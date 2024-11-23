using Admin.Helpers;
using Admin.Interfaces.Repositories;
using Admin.Interfaces.Services;
using Admin.Models;
using Admin.Models.Semar;
using Admin.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    [Authorize]
    public class SemarController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly ISemarRepository _repository;
        private readonly ICommonRepository _crepository;
        private readonly IEmailRepository _emailRepository;
        private readonly ISemarService _semarService;
        private readonly ApiHelper _apiHelper;
        private readonly IConfiguration _configuration;

        public SemarController(UserManager<ApplicationUser> userManager, IWebHostEnvironment environment, ISemarRepository repo, ICommonRepository common, IEmailRepository email, ISemarService semarService, ApiHelper apiHelper, IConfiguration configuration)
        {
            _environment = environment;
            _repository = repo;
            _crepository = common;
            _emailRepository = email;
            _userManager = userManager;
            _semarService = semarService;
            _apiHelper = apiHelper;
            _configuration = configuration;
        }

        [HttpGet]
        public string GetCurrentCultureDate()
        {
            return DateTime.Now.ToString();
        }

        public ViewResult Index()
        {
            ViewBag.Departments = _crepository.GetDepartments();
            ViewBag.SemarTypes = _crepository.GetSemarTypes();
            ViewBag.Classifications = _crepository.GetClassifications();
            ViewBag.Priorities = _crepository.GetPriorities();
            ViewBag.Statuses = _crepository.GetSemarStatuses();
            ViewBag.SemarLevel = _crepository.GetSemarLevels();
            ViewBag.SemarProducts = _crepository.GetSemarProducts();
            GetSemars(0);
            return View();
        }

        public ViewResult GetSemars(int type = 0)
        {
            ViewBag.Departments = _crepository.GetDepartments();
            ViewBag.SemarTypes = _crepository.GetSemarTypes();
            ViewBag.Classifications = _crepository.GetClassifications();
            ViewBag.Priorities = _crepository.GetPriorities();
            ViewBag.Statuses = _crepository.GetSemarStatuses();
            ViewBag.SemarLevel = _crepository.GetSemarLevels();
            var department = _userManager.FindByIdAsync(_userManager.GetUserId(User)).Result.Department;
            List<Semar> list = new List<Semar>();
            if (type == 0)
            {
                var stk = _crepository.GetSemarTypes().Where(item => item.Type == "STK").Select(item => item.SemarTypeID).ToList();
                var list1 = _semarService.Semars.Where(x => stk.Contains(x.Type)).ToList();
                var list2 = _semarService.Semars.Where(x => (x.Type == 5 || x.Type == 6) && x.Classification == 2).ToList();
                var list3 = _semarService.Semars.Where(x => (x.Type == 5 || x.Type == 6) && x.Owner == department && x.Classification == 1).ToList();
                list = list1.Concat(list2).Concat(list3).OrderByDescending(x => x.SemarID).ToList();
            }
            else
            {
                if (type == 99)
                {
                    var stk = _crepository.GetSemarTypes().Where(item => item.Type == "STK").Select(item => item.SemarTypeID).ToList();
                    list = _semarService.Semars.Where(x => stk.Contains(x.Type)).OrderByDescending(x => x.SemarID).ToList();
                }
                else
                {
                    if (type == 5 || type == 6)
                    {
                        var list1 = _semarService.Semars.Where(x => x.Type == type && x.Classification == 2).OrderByDescending(x => x.SemarID).ToList();
                        var list2 = _semarService.Semars.Where(x => x.Type == type && x.Owner == department && x.Classification == 1).ToList();
                        list = list1.Concat(list2).OrderByDescending(x => x.SemarID).ToList();
                    }
                    else
                    {
                        list = _semarService.Semars.Where(x => x.Type == type && x.Classification == 2).OrderByDescending(x => x.SemarID).ToList();
                    }
                }
            }
            return View("List", list);
        }

        public ViewResult ViewAction(string id)
        {
            ViewBag.Departments = _crepository.GetDepartments();
            ViewBag.SemarTypes = _crepository.GetSemarTypes();
            ViewBag.SemarLevels = _crepository.GetSemarLevels();
            ViewBag.Classifications = _crepository.GetClassifications();
            ViewBag.Priorities = _crepository.GetPriorities();
            ViewBag.Statuses = _crepository.GetSemarStatuses();
            var semar = _repository.Semars.FirstOrDefault(a => a.SemarID == id);
            return View("View", semar);
        }

        public ViewResult Add()
        {
            ViewBag.Title = "Add";
            ViewBag.Departments = _crepository.GetDepartments();
            ViewBag.SemarTypes = _crepository.GetSemarTypes();
            ViewBag.SemarLevels = _crepository.GetSemarLevels();
            ViewBag.Classifications = _crepository.GetClassifications();
            ViewBag.Priorities = _crepository.GetPriorities();
            ViewBag.Statuses = _crepository.GetSemarStatuses();
            var semar = new Semar
            {
                PublishDate = DateTime.Today,
                ExpiredDate = DateTime.Today.AddYears(1)
            };
            return View("Edit", semar);
        }

        public async Task<IActionResult> Edit(string id)
        {
            ViewBag.Title = "Edit";
            ViewBag.Departments = _crepository.GetDepartments();
            ViewBag.SemarTypes = _crepository.GetSemarTypes();
            ViewBag.SemarLevels = _crepository.GetSemarLevels();
            ViewBag.Classifications = _crepository.GetClassifications();
            ViewBag.Priorities = _crepository.GetPriorities();
            ViewBag.Statuses = _crepository.GetSemarStatuses();
            var department = (await _userManager.FindByIdAsync(_userManager.GetUserId(User))).Department;
            var semar = _repository.Semars.FirstOrDefault(x => x.SemarID == id);
            if (semar == null)
            {
                return RedirectToAction("Index");
            }
            return View(semar);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ICollection<IFormFile> files, Semar semar)
        {
            var mode = "edit";

            if (ModelState.IsValid)
            {
                if (semar.SemarID == null)
                {
                    mode = "add";
                    semar.SemarID = _repository.GetNextID();
                }

                var uploadPath = _configuration["UploadPath:semar"];
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var fileName = semar.SemarID + Path.GetExtension(file.FileName);
                        using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        semar.ContentType = file.ContentType;
                        semar.FileName = fileName;
                    }
                }
                var user = _userManager.GetUserName(User);
                var role = (await _userManager.GetRolesAsync(await _userManager.FindByNameAsync(user))).FirstOrDefault();
                semar.Creator = user;
                if (role == "AtasanAdmin")
                {
                    semar.Status = 2;
                }
                else
                {
                    if (semar.Type == 1)
                    {
                        var message = new StringBuilder();
                        message.Append("<table>");
                        message.Append($"<tr><td>Id.</td><td>:</td><td>{semar.SemarID}</td></tr>");
                        message.Append($"<tr><td>Type</td><td>:</td><td>{_crepository.GetSemarTypes().FirstOrDefault(x => x.SemarTypeID == semar.Type)?.Deskripsi}</td></tr>");
                        message.Append($"<tr><td>No. Dokumen</td><td>:</td><td>{semar.NoDocument}</td></tr>");
                        message.Append($"<tr><td>Title</td><td>:</td><td>{semar.Title}</td></tr>");
                        message.Append($"<tr><td>Level</td><td>:</td><td>{_crepository.GetSemarLevels().FirstOrDefault(x => x.SemarLevelID == semar.SemarLevel)?.Deskripsi}</td></tr>");
                        message.Append($"<tr><td>Owner</td><td>:</td><td>{_crepository.GetAllDepartments().FirstOrDefault(x => x.DepartmentID == semar.Owner)?.Deskripsi}</td></tr>");
                        message.Append($"<tr><td>Description</td><td>:</td><td>{semar.Description}</td></tr>");
                        message.Append($"<tr><td>Revision</td><td>:</td><td>{semar.Revision}</td></tr>");
                        message.Append($"<tr><td>Published Date</td><td>:</td><td>{semar.PublishDate:dd MMMM yyyy}</td></tr>");
                        message.Append($"<tr><td>Expired Date</td><td>:</td><td>{semar.ExpiredDate:dd MMMM yyyy}</td></tr>");
                        message.Append("</table>");

                        var admins = _userManager.Users.Where(x => x.Department == semar.Owner && x.UserName != semar.Creator).ToList();
                        foreach (var admin in admins)
                        {
                            if ((await _userManager.GetRolesAsync(await _userManager.FindByNameAsync(admin.UserName))).FirstOrDefault() == "AtasanAdmin")
                            {
                                var email = new Email
                                {
                                    Receiver = admin.Email,
                                    Subject = "SEMAR Approval Notification",
                                    Message = $"Dear {admin.Name},<br/><p>SEMAR berikut ini membutuhkan approval dari Anda:</p>{message}",
                                    Schedule = DateTime.Now,
                                    CreatedOn = DateTime.Now
                                };
                                _emailRepository.Save(email);
                            }
                        }
                    }
                }
                await _apiHelper.SendEmailAsync();
                _repository.Save(semar, mode);
                TempData["message"] = $"{semar.SemarID} has been saved";
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Title = "Edit";
                ViewBag.Departments = _crepository.GetDepartments();
                ViewBag.SemarTypes = _crepository.GetSemarTypes();
                ViewBag.SemarLevels = _crepository.GetSemarLevels();
                ViewBag.Classifications = _crepository.GetClassifications();
                ViewBag.Priorities = _crepository.GetPriorities();
                ViewBag.Statuses = _crepository.GetSemarStatuses();

                return View(semar);
            }
        }

        public async Task<FileResult> DownloadFile(string semarID)
        {
            var semar = _repository.Semars.FirstOrDefault(x => x.SemarID == semarID);
            var filepath = Path.Combine(_configuration["UploadPath:semar"], semar.FileName);
            if (!System.IO.File.Exists(filepath))
            {
                var emptyFileBytes = Encoding.UTF8.GetBytes("Semar file not found on the server.");
                return File(emptyFileBytes, "text/plain", "NotFound.txt");
            }
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filepath);
            return File(fileBytes, semar.ContentType, semar.FileName);
        }

        public IActionResult Approve(string semarId)
        {
            TempData["message"] = $"{semarId} has been approved";
            var semar = _repository.Approve(semarId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(Semar semar)
        {
            _repository.Delete(semar);
            TempData["message"] = $"{semar.SemarID} has been deleted.";
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public ViewResult pIndex()
        {
            ViewBag.Departments = _crepository.GetDepartments();
            ViewBag.SemarTypes = _crepository.GetSemarTypes();
            ViewBag.Classifications = _crepository.GetClassifications();
            ViewBag.Priorities = _crepository.GetPriorities();
            ViewBag.Statuses = _crepository.GetSemarStatuses();
            return View(_repository.Semars.Where(x => x.Classification == 2 && x.Status == 2 && x.ExpiredDate >= DateTime.Now).OrderByDescending(x => x.SemarID));
        }

        [AllowAnonymous]
        public ViewResult pView(string id)
        {
            ViewBag.Departments = _crepository.GetDepartments();
            ViewBag.SemarTypes = _crepository.GetSemarTypes();
            ViewBag.SemarLevels = _crepository.GetSemarLevels();
            ViewBag.Classifications = _crepository.GetClassifications();
            ViewBag.Priorities = _crepository.GetPriorities();
            ViewBag.Statuses = _crepository.GetSemarStatuses();
            var semar = _repository.Semars.FirstOrDefault(x => x.SemarID == id && x.Classification == 2);
            return View("pView", semar);
        }
    }
}

