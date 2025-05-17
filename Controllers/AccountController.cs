﻿using Admin.Helpers;
using Admin.Interfaces.Repositories;
using Admin.Interfaces.Services;
using Admin.Models.AccountViewModels;
using Admin.Models.AdSettings;
using Admin.Models.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    [Authorize]
    public class AccountController : Controller, IDisposable
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger<AccountController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICommonRepository _repository;
        private readonly IEmailRepository _emailRepository;
        private readonly ApiHelper _apiHelper;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly AdSettings _adSettings;
        private readonly LdapConnection _ldapConnection;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILogger<AccountController> logger,
            RoleManager<IdentityRole> roleManager,
            IEmailRepository emailRepository,
            ICommonRepository repository,
            ApiHelper apiHelper,
            IConfiguration configuration,
            HttpClient httpClient,
            AdSettings adSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = logger;
            _roleManager = roleManager;
            _repository = repository;
            _emailRepository = emailRepository;
            _apiHelper = apiHelper;
            _configuration = configuration;
            _httpClient = httpClient;
            _adSettings = adSettings;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                if (await AuthenticateUserAsync(model))
                {
                    return RedirectToLocal(returnUrl);
                }

                ModelState.AddModelError(string.Empty, "Authentication failed.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(0, "LoginError"), ex, "Error occurred during login.");
                ModelState.AddModelError(string.Empty, "An error occurred during login.");
                return View(model);
            }
        }

        private async Task<bool> AuthenticateUserAsync(LoginViewModel model)
        {
            if (VerifyPassAd(model.UserName, model.Password))
            {
                var formattedUserName = FormatUserName(model.UserName);
                var user = await _userManager.FindByNameAsync(formattedUserName);
                if (user != null)
                {
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    _logger.LogInformation("User logged in via ADAuth.");
                    return true;
                }

                ModelState.AddModelError(string.Empty, "User not found.");
                return false;
            }

            var localSignInResult = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (localSignInResult.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return true;
            }

            return false;
        }

        private string FormatUserName(string userName)
        {
            return userName.StartsWith("mk.", StringComparison.OrdinalIgnoreCase)
                ? userName.Substring(3)
                : userName;
        }

        public bool VerifyPassAd(string userName, string password)
        {
            string ldapHost = _adSettings.Server;
            int ldapPort = _adSettings.Port;
            string userDn = $"{userName}@{_adSettings.Domain}";

            try
            {
                using var ldapConnection = new LdapConnection(new LdapDirectoryIdentifier(ldapHost, ldapPort));
                var networkCredential = new System.Net.NetworkCredential(userDn, password);
                ldapConnection.Credential = networkCredential;
                ldapConnection.Bind();

                System.Diagnostics.Debug.WriteLine("LDAP bind successful.");
                return true;
            }
            catch (LdapException ldapException)
            {
                System.Diagnostics.Debug.WriteLine($"LDAP Exception: {ldapException}");
            }

            return false;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
            }

            if (result.IsLockedOut)
            {
                return View("Lockout");
            }

            ViewData["ReturnUrl"] = returnUrl;
            ViewData["LoginProvider"] = info.LoginProvider;
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return View("ExternalLoginFailure");
            }

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                result = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                    return RedirectToLocal(returnUrl);
                }
            }

            AddErrors(result);
            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return View("ForgotPasswordConfirmation");
            }

            // Send email with reset link (implementation not shown)
            return View("ForgotPasswordConfirmation");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            var message = "Your security code is: " + code;
            if (model.SelectedProvider == "Email")
            {
                await _emailSender.SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code", message);
            }
            else if (model.SelectedProvider == "Phone")
            {
                await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
            }

            return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return View("Lockout");
            }

            ModelState.AddModelError(string.Empty, "Invalid code.");
            return View(model);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        public ViewResult Index()
        {
            var users = _userManager.Users.ToList();
            var models = users.Select(user => new AccountViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name,
                Department = GetDepartmentDescription(user.Department, _repository),
                Role = GetUserRole(user, _userManager).Result
            }).ToList();

            return View(models);
        }

        public ViewResult Add()
        {
            ViewBag.Roles = _roleManager.Roles;
            ViewBag.Department = _repository.GetAllDepartments();
            ViewBag.Jabatan = _repository.GetJabatan();
            var model = new AccountViewModel();
            return View("Edit", model);
        }

        public ViewResult Edit(string id)
        {
            ViewBag.Roles = _roleManager.Roles;
            ViewBag.Department = _repository.GetAllDepartments();
            ViewBag.Jabatan = _repository.GetJabatan();
            var user = _userManager.Users.FirstOrDefault(x => x.Id == id);
            var model = new AccountViewModel
            {
                Id = user.Id,
                Name = user.Name,
                UserName = user.UserName,
                Email = user.Email,
                Department = user.Department,
                NIP = user.NIP,
                IsTkjp = user.IsTkjp,
                Jabatan = user.Jabatan,
                GCG = user.GCG,
                GCGAdmin = user.GCGAdmin,
                Role = GetUserRole(user, _userManager).Result
            };

            return View("Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = _roleManager.Roles;
                ViewBag.Department = _repository.GetDepartments();
                ViewBag.Jabatan = _repository.GetJabatan();
                return View(model);
            }

            // Check for duplicate email
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null && existingUser.Id != model.Id)
            {
                TempData["error"] = $"A user with the email '{model.Email}' is already registered.";
                ViewBag.Roles = _roleManager.Roles;
                ViewBag.Department = _repository.GetDepartments();
                ViewBag.Jabatan = _repository.GetJabatan();
                return View(model); // Redirect back to the Edit page with the model
            }

            IdentityResult result;
            if (string.IsNullOrEmpty(model.Id))
            {
                result = await CreateUser(model);
            }
            else
            {
                result = await UpdateUser(model);
            }

            if (result.Succeeded)
            {
                TempData["message"] = $"User {model.UserName} has been saved successfully.";
                return RedirectToAction("Index");
            }

            AddErrors(result);
            ViewBag.Roles = _roleManager.Roles;
            ViewBag.Department = _repository.GetDepartments();
            ViewBag.Jabatan = _repository.GetJabatan();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccountAction(string Id, string Action)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return RedirectToAction("Index");
            }

            if (Action == "Delete")
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["message"] = $"User {user.UserName} has been deleted.";
                }
            }
            else if (Action == "ResetPassword")
            {
                await ResetUserPassword(user);
            }

            return RedirectToAction("Index");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string NewPassword)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var result = await _userManager.ChangePasswordAsync(user, "123", NewPassword);

            if (result.Succeeded)
            {
                TempData["message"] = "Your password has been changed";
            }
            else
            {
                TempData["error"] = string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction("ChangePassword");
        }

        [AllowAnonymous]
        public string IsSignedIn()
        {
            return _signInManager.IsSignedIn(User) ? "Y" : "N";
        }

        #region Helpers

        private async Task<IdentityResult> CreateUser(AccountViewModel model)
        {
            var password = "Rn" + new Random().Next(1000000, 9999999) + "!";
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Name = model.Name,
                Email = model.Email,
                Department = model.Department
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var role = await _roleManager.FindByIdAsync(model.Role);
                result = await _userManager.AddToRoleAsync(user, role.Name);
                if (result.Succeeded)
                {
                    await SendConfirmationEmail(user, password);
                }
            }

            return result;
        }

        private async Task<IdentityResult> UpdateUser(AccountViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            user.Name = model.Name;
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.Department = model.Department;
            user.NIP = model.NIP;
            user.Jabatan = model.Jabatan;
            user.GCG = model.GCG;
            user.IsTkjp = model.IsTkjp;
            user.GCGAdmin = model.GCGAdmin;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                var role = await _roleManager.FindByIdAsync(model.Role);
                var currentRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                if (currentRole != null)
                {
                    await _userManager.RemoveFromRoleAsync(user, currentRole);
                }
                result = await _userManager.AddToRoleAsync(user, role.Name);
            }

            return result;
        }

        private async Task SendConfirmationEmail(ApplicationUser user, string password)
        {
            var email = new Email
            {
                Subject = "Konfirmasi Pendaftaran User",
                Receiver = user.Email,
                Message = $"Dear Bpk/Ibu {user.Name},<p/>Anda telah terdaftar di Portal Nusantara Regas.<br/>Silahkan login ke http://portal.nusantararegas.com menggunakan username dan password berikut ini.<br>Username: {user.UserName}<br/>Password baru: {password}",
                Schedule = DateTime.Now,
                CreatedOn = DateTime.Now
            };
            _emailRepository.Save(email);
            await _apiHelper.SendEmailAsync();
        }

        private async Task ResetUserPassword(ApplicationUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var password = "Rn" + new Random().Next(1000000, 9999999) + "!";
            var result = await _userManager.ResetPasswordAsync(user, token, password);
            if (result.Succeeded)
            {
                var email = new Email
                {
                    Subject = "Konfirmasi Reset Password",
                    Receiver = user.Email,
                    Message = $"Dear Bpk/Ibu {user.Name},<p/>Password Anda telah direset.<br/>Silahkan login ke http://portal.nusantararegas.com menggunakan username dan password berikut ini.<br>Username: {user.UserName}<br/>Password baru: {password}",
                    Schedule = DateTime.Now,
                    CreatedOn = DateTime.Now
                };
                _emailRepository.Save(email);
                await _apiHelper.SendEmailAsync();
                TempData["message"] = $"Password {user.UserName} has been reset.";
            }
        }

        public static string GetDepartmentDescription(string departmentId, ICommonRepository repository)
        {
            return repository.GetAllDepartments().FirstOrDefault(x => x.DepartmentID == departmentId)?.Deskripsi ?? "";
        }

        public static async Task<string> GetUserRole(ApplicationUser user, UserManager<ApplicationUser> userManager)
        {
            return (await userManager.GetRolesAsync(user)).FirstOrDefault() ?? "";
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        public new void Dispose()
        {
            _ldapConnection?.Dispose();
        }

        #endregion
    }
}

