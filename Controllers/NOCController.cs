using Admin.Helpers;
using Admin.Models;
using Admin.Services;
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

        public NOCController(UserManager<ApplicationUser> userManager, IWebHostEnvironment environment, INOCRepository repo, ICommonRepository common, IEmailRepository emailRepo, INOCService nocService, ApiHelper apiHelper, IConfiguration configuration)
        {
            _environment = environment;
            _repository = repo;
            _crepository = common;
            _userManager = userManager;
            _emailRepository = emailRepo;
            _nocService = nocService;
            _apiHelper = apiHelper;
            _configuration = configuration;
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
                    var text = new StringBuilder();
                    text.Append("<table>");
                    text.Append($"<tr><td>Waktu</td><td>:</td><td>{noc.EntryDate:dd MMMM yyyy HH:mm}</td></tr>");
                    text.Append($"<tr><td>Lokasi</td><td>:</td><td>{_crepository.GetLocations().FirstOrDefault(x => x.LocationID == noc.Lokasi)?.Deskripsi}</td></tr>");
                    text.Append($"<tr><td>Daftar Pengamatan</td><td>:</td><td>{(await _repository.GetObservationListsAsync()).FirstOrDefault(x => x.ObservationListID == noc.DaftarPengamatan)?.Deskripsi}</td></tr>");
                    text.Append($"<tr><td>CLSR Category</td><td>:</td><td>{(await _repository.GetClsrListsAsync()).FirstOrDefault(x => x.ClsrID == noc.Clsr)?.Deskripsi}</td></tr>");
                    text.Append($"<tr><td>Deskripsi</td><td>:</td><td>{noc.Deskripsi}</td></tr>");
                    text.Append($"<tr><td>Tindakan</td><td>:</td><td>{noc.Tindakan}</td></tr>");
                    text.Append($"<tr><td>Rekomendasi</td><td>:</td><td>{noc.Rekomendasi}</td></tr>");
                    text.Append($"<tr><td>Prioritas</td><td>:</td><td>{_crepository.GetPriorities().FirstOrDefault(x => x.PriorityID == noc.Prioritas)?.Deskripsi.Split('.')[1]}</td></tr>");
                    text.Append($"<tr><td>Status</td><td>:</td><td>{_repository.GetNOCStatuses().FirstOrDefault(x => x.NOCStatusID == noc.Status)?.Deskripsi}</td></tr>");
                    text.Append($"<tr><td>Due Date</td><td>:</td><td>{noc.DueDate:dd MMMM yyyy}</td></tr>");
                    text.Append("</table>");

                    var emailIncident1 = new Email
                    {
                        Receiver = _configuration["Hsse:Receiver1"],
                        Subject = "New Incident Alert",
                        Message = $"Terdapat Incident Baru {text}",
                        Schedule = DateTime.Now,
                        CreatedOn = DateTime.Now
                    };
                    _emailRepository.Save(emailIncident1);

                    var emailIncident2 = new Email
                    {
                        Receiver = _configuration["Hsse:Receiver2"],
                        Subject = "New Incident Alert",
                        Message = $"Terdapat Incident Baru {text}",
                        Schedule = DateTime.Now,
                        CreatedOn = DateTime.Now
                    };
                    _emailRepository.Save(emailIncident2);

                    await _apiHelper.SendEmailAsync();
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
