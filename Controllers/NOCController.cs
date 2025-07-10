using Admin.Helpers;
using Admin.Interfaces.Repositories;
using Admin.Interfaces.Services;
using Admin.Models;
using Admin.Models.NOC;
using Admin.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Admin.Services;

namespace Admin.Controllers
{
    [Authorize]
    public class NOCController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly INOCRepository _repository;
        private readonly ICommonRepository _crepository;
        private readonly IEmailRepository _emailRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INOCService _nocService;
        private readonly ApiHelper _apiHelper;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public NOCController(UserManager<ApplicationUser> userManager, IWebHostEnvironment environment, INOCRepository repo, ICommonRepository common, IEmailRepository emailRepo, INOCService nocService, ApiHelper apiHelper, IConfiguration configuration, IEmailService emailService)
        {
            _environment = environment;
            _repository = repo;
            _crepository = common;
            _userManager = userManager;
            _emailRepository = emailRepo;
            _nocService = nocService;
            _apiHelper = apiHelper;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<ViewResult> Index()
        {
            ViewData["Title"] = "Nusantara Regas Observation Card (NOC)";

            ViewBag.Departments = _crepository.GetAllDepartments();
            ViewBag.Locations = _crepository.GetLocations();
            ViewBag.Priorities = _crepository.GetPriorities();
            ViewBag.Statuses = _repository.GetNOCStatuses();
            ViewBag.ObservationLists = await _repository.GetObservationListsAsync();

            var onlineUser = _userManager.GetUserName(User);
            if (User.IsInRole("AdminNOC"))
            {
                return View(_repository.NOCs.Where(x => x.EntryDate.Month == DateTime.Now.Month && x.EntryDate.Year == DateTime.Now.Year).ToList());
            }
            return View(_repository.NOCs.Where(x => x.NamaObserver == onlineUser).ToList());
        }

        public async Task<ViewResult> ViewAction(string nocId)
        {
            ViewBag.Departments = _crepository.GetDepartments();
            ViewBag.Locations = _crepository.GetLocations();
            ViewBag.Priorities = _crepository.GetPriorities();
            ViewBag.Statuses = _repository.GetNOCStatuses();
            ViewBag.ObservationLists = await _repository.GetObservationListsAsync();
            ViewBag.ClsrLists = await _repository.GetClsrListsAsync();
            var noc = _repository.NOCs.FirstOrDefault(a => a.NOCID == nocId);
            return View("View", noc);
        }

        public async Task<ViewResult> Add()
        {
            ViewBag.Title = "Add";
            ViewBag.Departments = _crepository.GetDepartments();
            ViewBag.Locations = _crepository.GetLocations();
            ViewBag.Priorities = _crepository.GetPriorities();
            ViewBag.Statuses = _repository.GetNOCStatuses();
            ViewBag.ObservationLists = await _repository.GetObservationListsAsync();
            ViewBag.ClsrLists = await _repository.GetClsrListsAsync();
            var noc = new NOC
            {
                EntryDate = DateTime.Now,
                DueDate = DateTime.Now.Date
            };
            return View("Edit", noc);
        }

        public async Task<IActionResult> Edit(string id)
        {
            ViewBag.Title = "Edit";
            ViewBag.Departments = _crepository.GetDepartments();
            ViewBag.Locations = _crepository.GetLocations();
            ViewBag.Priorities = _crepository.GetPriorities();
            ViewBag.Statuses = _repository.GetNOCStatuses();
            ViewBag.ObservationLists = await _repository.GetObservationListsAsync();
            ViewBag.ClsrLists = await _repository.GetClsrListsAsync();
            var onlineUser = _userManager.GetUserName(User);

            NOC noc;
            if (User.IsInRole("AdminNOC"))
            {
                noc = _repository.NOCs.FirstOrDefault(a => a.NOCID == id);
            }
            else
            {
                noc = _repository.NOCs.FirstOrDefault(a => a.NOCID == id && a.NamaObserver == onlineUser);
            }

            if (noc == null)
            {
                return RedirectToAction("Index");
            }
            return View(noc);
        }

        public async Task<ViewResult> GetNOCs(string startDate, string endDate)
        {
            ViewBag.Departments = _crepository.GetDepartments();
            ViewBag.Locations = _crepository.GetLocations();
            ViewBag.Priorities = _crepository.GetPriorities();
            ViewBag.Statuses = _repository.GetNOCStatuses();
            ViewBag.ObservationLists = await _repository.GetObservationListsAsync();
            ViewBag.ClsrLists = await _repository.GetClsrListsAsync();
            var provider = new CultureInfo("id-ID");
            var dtStart = DateTime.ParseExact(startDate, "dd/MM/yyyy", provider);
            var dtEnd = DateTime.ParseExact(endDate, "dd/MM/yyyy", provider);

            var list = _nocService.NOC.Where(x => x.EntryDate >= dtStart && x.EntryDate <= dtEnd.AddDays(1)).ToList();

            return View("List", list);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(NOC noc)
        {
            await _repository.DeleteAsync(noc);
            TempData["message"] = $"{noc.NOCID} has been deleted.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ICollection<IFormFile> files, NOC noc)
        {
            if (ModelState.IsValid)
            {
                var mode = noc.NOCID == null ? "add" : "edit";
                if (noc.NOCID == null)
                {
                    noc.NOCID = await _repository.GetNextIDAsync();
                }

                var uploadPath = _configuration["UploadPath:noc"];
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var fileName = noc.NOCID.ToString().PadLeft(4, '0') + Path.GetExtension(file.FileName);
                        using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        noc.ContentType = file.ContentType;
                        noc.Photo = fileName;
                    }
                }

                noc.NamaObserver = _userManager.GetUserName(User);
                noc.DivisiObserver = (await _userManager.FindByIdAsync(_userManager.GetUserId(User))).Department;

                await _repository.SaveAsync(noc, mode);

                int[] incidentIds = { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
                if (incidentIds.Contains(noc.DaftarPengamatan))
                {
                    await _emailService.SendTemplatedEmailAsync(
                        "NOC_INCIDENT",
                        _configuration["Hsse:Receiver1"],
                        new
                        {
                            NOCID = noc.NOCID,
                            RecipientName = "HSSE Team",
                            EntryDate = noc.EntryDate.ToString("dd MMMM yyyy HH:mm"),
                            Location = _crepository.GetLocations().FirstOrDefault(x => x.LocationID == noc.Lokasi)?.Deskripsi,
                            ObservationType = (await _repository.GetObservationListsAsync()).FirstOrDefault(x => x.ObservationListID == noc.DaftarPengamatan)?.Deskripsi,
                            ClsrCategory = (await _repository.GetClsrListsAsync()).FirstOrDefault(x => x.ClsrID == noc.Clsr)?.Deskripsi,
                            Description = noc.Deskripsi,
                            Action = noc.Tindakan,
                            Recommendation = noc.Rekomendasi,
                            Priority = _crepository.GetPriorities().FirstOrDefault(x => x.PriorityID == noc.Prioritas)?.Deskripsi.Split('.')[1],
                            Status = _repository.GetNOCStatuses().FirstOrDefault(x => x.NOCStatusID == noc.Status)?.Deskripsi,
                            DueDate = noc.DueDate.ToString("dd MMMM yyyy")
                        },
                        "id",
                        EmailPriority.High,
                        "NOC"
                    );

                    await _emailService.SendTemplatedEmailAsync(
                        "NOC_INCIDENT",
                        _configuration["Hsse:Receiver2"],
                        new
                        {
                            NOCID = noc.NOCID,
                            RecipientName = "HSSE Team",
                            EntryDate = noc.EntryDate.ToString("dd MMMM yyyy HH:mm"),
                            Location = _crepository.GetLocations().FirstOrDefault(x => x.LocationID == noc.Lokasi)?.Deskripsi,
                            ObservationType = (await _repository.GetObservationListsAsync()).FirstOrDefault(x => x.ObservationListID == noc.DaftarPengamatan)?.Deskripsi,
                            ClsrCategory = (await _repository.GetClsrListsAsync()).FirstOrDefault(x => x.ClsrID == noc.Clsr)?.Deskripsi,
                            Description = noc.Deskripsi,
                            Action = noc.Tindakan,
                            Recommendation = noc.Rekomendasi,
                            Priority = _crepository.GetPriorities().FirstOrDefault(x => x.PriorityID == noc.Prioritas)?.Deskripsi.Split('.')[1],
                            Status = _repository.GetNOCStatuses().FirstOrDefault(x => x.NOCStatusID == noc.Status)?.Deskripsi,
                            DueDate = noc.DueDate.ToString("dd MMMM yyyy")
                        },
                        "id",
                        EmailPriority.High,
                        "NOC"
                    );
                }

                TempData["message"] = $"{noc.NOCID} has been saved";
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Departments = _crepository.GetDepartments();
                ViewBag.Locations = _crepository.GetLocations();
                ViewBag.Priorities = _crepository.GetPriorities();
                ViewBag.Statuses = _repository.GetNOCStatuses();
                ViewBag.ObservationLists = await _repository.GetObservationListsAsync();
                ViewBag.ClsrLists = await _repository.GetClsrListsAsync();

                return View(noc);
            }
        }

        [AllowAnonymous]
        public async Task<FileResult> DownloadFile(string id)
        {
            var noc = _repository.NOCs.FirstOrDefault(x => x.NOCID == id);
            var filepath = Path.Combine(_configuration["UploadPath:noc"], noc.Photo);
            if (!System.IO.File.Exists(filepath))
            {
                var emptyFileBytes = Encoding.UTF8.GetBytes("NOC file not found on the server.");
                return File(emptyFileBytes, "text/plain", "NotFound.txt");
            }
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filepath);
            return File(fileBytes, noc.ContentType, noc.Photo);
        }

        [AllowAnonymous]
        public async Task<ViewResult> pIndex()
        {
            ViewData["Title"] = "Nusantara Regas Observation Card (NOC)";

            ViewBag.Departments = _crepository.GetDepartments().ToList();
            ViewBag.Locations = _crepository.GetLocations();
            ViewBag.Priorities = _crepository.GetPriorities();
            ViewBag.Statuses = _repository.GetNOCStatuses().ToList();
            ViewBag.ObservationLists = await _repository.GetObservationListsAsync();
            return View(_repository.NOCs.ToList());
        }

        [AllowAnonymous]
        public async Task<ViewResult> pView(string id)
        {
            ViewBag.Departments = _crepository.GetDepartments();
            ViewBag.Locations = _crepository.GetLocations();
            ViewBag.Priorities = _crepository.GetPriorities();
            ViewBag.Statuses = _repository.GetNOCStatuses();
            ViewBag.ObservationLists = await _repository.GetObservationListsAsync();
            var noc = _repository.NOCs.FirstOrDefault(a => a.NOCID == id);
            return View(noc);
        }
    }
}
