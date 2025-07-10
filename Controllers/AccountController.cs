using Admin.Helpers;
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
using Admin.Services;

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
        private readonly IEmailService _emailService;
        private readonly IImmediateEmailService _immediateEmailService;

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
            AdSettings adSettings,
            IEmailService emailService,
            IImmediateEmailService immediateEmailService)
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
            _emailService = emailService;
            _immediateEmailService = immediateEmailService;
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
                // Don't reveal that the user does not exist or is not confirmed
                return View("ForgotPasswordConfirmation");
            }

            try
            {
                // Generate password reset token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                
                // Create reset link
                var resetLink = Url.Action("ResetPassword", "Account", 
                    new { userId = user.Id, code = token }, 
                    protocol: HttpContext.Request.Scheme);

                // Send email using ImmediateEmailService for instant delivery
                var emailSent = await _immediateEmailService.SendImmediateTemplatedEmailAsync(
                    "PASSWORD_RESET_LINK",
                    user.Email,
                    new
                    {
                        Name = user.Name ?? user.UserName,
                        ResetLink = resetLink
                    },
                    "en",
                    logToDatabase: true
                );

                if (!emailSent)
                {
                    _logger.LogError($"Failed to send immediate password reset email for {user.Email}");
                    // Still show confirmation to prevent user enumeration
                }
                else
                {
                    _logger.LogInformation($"Password reset email sent immediately to {user.Email}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during password reset process for {model.Email}");
                // Still show confirmation to prevent user enumeration
            }

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
        public async Task<IActionResult> ResetPassword(string userId = null, string code = null)
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

            // Verify that the token is valid
            var isTokenValid = await _userManager.VerifyUserTokenAsync(user, 
                _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", code);
            
            if (!isTokenValid)
            {
                return View("Error");
            }

            var model = new ResetPasswordViewModel
            {
                Code = code,
                Email = user.Email
            };

            return View(model);
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> TestEmailService(string email = null)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Email parameter is required" });
            }

            try
            {
                // Test with regular EmailService (queued)
                var result = await _emailService.SendTemplatedEmailAsync(
                    "PASSWORD_RESET_LINK",
                    email,
                    new
                    {
                        Name = "Test User",
                        ResetLink = "https://portal.nusantararegas.com/Account/ResetPassword?userId=test&code=test123"
                    },
                    "en",
                    EmailPriority.High,
                    "TEST"
                );

                var responseMessage = result 
                    ? "Email queued successfully! Check the Emails table in database." 
                    : "Failed to queue email. Check logs for details.";

                return Json(new { 
                    success = result, 
                    message = responseMessage,
                    smtpServer = _configuration["Email:SmtpServer"],
                    smtpPort = _configuration["Email:SmtpPort"],
                    backgroundServiceEnabled = _configuration["Email:EnableBackgroundProcessing"]
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error testing email service to {email}");
                return Json(new { 
                    success = false, 
                    message = ex.Message,
                    smtpServer = _configuration["Email:SmtpServer"],
                    smtpPort = _configuration["Email:SmtpPort"]
                });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> TestImmediateEmailService(string email = null)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Email parameter is required" });
            }

            try
            {
                // First test SMTP connection
                var smtpTestResult = await _immediateEmailService.TestSmtpConnectionAsync();
                
                // Test immediate email sending
                var result = await _immediateEmailService.SendImmediateTemplatedEmailAsync(
                    "PASSWORD_RESET",
                    email,
                    new
                    {
                        RecipientName = "Test User",
                        UserName = "testuser",
                        Password = "TestPassword123!",
                        LoginUrl = "http://portal.nusantararegas.com"
                    },
                    "en",
                    logToDatabase: true
                );

                var responseMessage = result 
                    ? "Immediate email sent successfully!" 
                    : "Failed to send immediate email. Check logs for details.";

                return Json(new { 
                    success = result, 
                    message = responseMessage,
                    smtpConnectionTest = smtpTestResult,
                    smtpServer = _configuration["Email:SmtpServer"],
                    smtpPort = _configuration["Email:SmtpPort"],
                    fromEmail = _configuration["Email:FromEmail"],
                    fromName = _configuration["Email:FromName"]
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error testing immediate email service to {email}");
                return Json(new { 
                    success = false, 
                    message = ex.Message,
                    smtpServer = _configuration["Email:SmtpServer"],
                    smtpPort = _configuration["Email:SmtpPort"],
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> TestPasswordResetEmail(string email = null, string userName = null)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Email parameter is required" });
            }

            try
            {
                _logger.LogInformation($"Testing password reset email functionality for {email}");
                
                // Check if email exists in database before sending
                var emailsBefore = _emailRepository.Emails.Count();
                var pendingEmailsBefore = _emailRepository.GetEmailsByStatus(EmailStatus.Pending).Count();
                var failedEmailsBefore = _emailRepository.GetEmailsByStatus(EmailStatus.Failed).Count();
                
                // Test the email sending with correct template data
                var testEmailSent = await _immediateEmailService.SendImmediateTemplatedEmailAsync(
                    "PASSWORD_RESET",
                    email,
                    new
                    {
                        Name = userName ?? "Test User",     // Template expects "Name"
                        Username = userName ?? "testuser",  // Template expects "Username"
                        Password = "TestPassword123!"
                    },
                    "id",
                    logToDatabase: true
                );
                
                // Check email counts after
                await Task.Delay(1000); // Give time for database operations
                var emailsAfter = _emailRepository.Emails.Count();
                var pendingEmailsAfter = _emailRepository.GetEmailsByStatus(EmailStatus.Pending).Count();
                var failedEmailsAfter = _emailRepository.GetEmailsByStatus(EmailStatus.Failed).Count();
                var sentEmailsAfter = _emailRepository.GetEmailsByStatus(EmailStatus.Sent).Count();
                
                // Get the latest email record for this recipient
                var latestEmail = _emailRepository.GetEmailsByCategory("IMMEDIATE")
                    .Where(e => e.Receiver == email)
                    .OrderByDescending(e => e.CreatedOn)
                    .FirstOrDefault();

                var responseMessage = testEmailSent 
                    ? "Password reset email sent successfully!" 
                    : "Failed to send password reset email. Check logs for details.";

                return Json(new { 
                    success = testEmailSent, 
                    message = responseMessage,
                    emailCounts = new {
                        before = new { total = emailsBefore, pending = pendingEmailsBefore, failed = failedEmailsBefore },
                        after = new { total = emailsAfter, pending = pendingEmailsAfter, failed = failedEmailsAfter, sent = sentEmailsAfter }
                    },
                    latestEmailRecord = latestEmail != null ? new {
                        id = latestEmail.EmailID,
                        status = latestEmail.Status,
                        statusText = ((EmailStatus)latestEmail.Status).ToString(),
                        createdOn = latestEmail.CreatedOn,
                        sentOn = latestEmail.SentOn,
                        errorMessage = latestEmail.ErrorMessage,
                        templateType = latestEmail.TemplateType
                    } : null,
                    smtpConfig = new {
                        server = _configuration["Email:SmtpServer"],
                        port = _configuration["Email:SmtpPort"],
                        fromEmail = _configuration["Email:FromEmail"],
                        fromName = _configuration["Email:FromName"],
                        user = !string.IsNullOrEmpty(_configuration["Email:SmtpUser"]) ? "Configured" : "Not configured"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error testing password reset email to {email}");
                return Json(new { 
                    success = false, 
                    message = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
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
            await _emailService.SendTemplatedEmailAsync(
                "ACCOUNT_REGISTRATION",
                user.Email,
                new
                {
                    RecipientName = user.Name,
                    UserName = user.UserName,
                    Password = password,
                    LoginUrl = "http://portal.nusantararegas.com"
                },
                "id", // Indonesian language
                EmailPriority.High,
                "ACCOUNT"
            );
        }

        private async Task ResetUserPassword(ApplicationUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var password = "Rn" + new Random().Next(1000000, 9999999) + "!";
            var result = await _userManager.ResetPasswordAsync(user, token, password);
            if (result.Succeeded)
            {
                _logger.LogInformation($"Password reset succeeded for user {user.UserName}. Attempting to send email...");
                
                // Send email notification using ImmediateEmailService for instant delivery
                var emailSent = await _immediateEmailService.SendImmediateTemplatedEmailAsync(
                    "PASSWORD_RESET",
                    user.Email,
                    new
                    {
                        Name = user.Name ?? user.UserName,        // Template expects "Name"
                        Username = user.UserName,                 // Template expects "Username" 
                        Password = password
                    },
                    "id", // Indonesian language
                    logToDatabase: true
                );

                if (emailSent)
                {
                    TempData["message"] = $"Password {user.UserName} has been reset and email sent immediately.";
                    _logger.LogInformation($"Password reset email sent immediately to {user.Email}");
                }
                else
                {
                    TempData["message"] = $"Password {user.UserName} has been reset, but email sending failed. Please check logs and email configuration.";
                    _logger.LogError($"Failed to send immediate password reset email to {user.Email}. Email record should be in database with Failed status.");
                }
            }
            else
            {
                _logger.LogError($"Password reset failed for user {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
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
            // Dispose of any other resources if needed
        }

        #endregion
    }
}

