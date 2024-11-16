using Admin.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Admin.Helpers;
using Newtonsoft.Json;

namespace Admin.Controllers
{
    [Authorize]
    public class HSSEReportController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IHSSEReportRepository repository;
        private readonly ICommonRepository crepository;
        private readonly ApiHelper apiHelper;
        private readonly IConfiguration configuration;

        public HSSEReportController(IWebHostEnvironment environment, IHSSEReportRepository repo, ICommonRepository common, ApiHelper apiHelper, IConfiguration configuration)
        {
            _environment = environment;
            repository = repo;
            crepository = common;
            this.apiHelper = apiHelper;
            this.configuration = configuration;
        }

        public ViewResult Index()
        {
            ViewData["Title"] = "HSSE Reports";
            return View(repository.HSSEReports);
        }

        public async Task<ViewResult> Pending()
        {
            var json = await apiHelper.GetReportsAsync();
            var obj = JsonConvert.DeserializeObject<IEnumerable<HSSEReport>>(json);
            return View("Pending", obj);
        }

        public async Task<ViewResult> ViewPending(string id)
        {
            ViewBag.ImageSource = configuration["API:Host"] + "/HSSEReport/GetFile?FileName=";
            var json = await apiHelper.GetReportAsync(id);
            var obj = JsonConvert.DeserializeObject<HSSEReport>(json);
            return View("ViewPending", obj);
        }

        public ViewResult ViewAction(string id)
        {
            ViewBag.UploadPath = configuration["UploadPath:hsse"];
            var report = repository.HSSEReports.FirstOrDefault(a => a.HSSEReportID == id);
            return View("View", report);
        }

        public ViewResult Add()
        {
            ViewBag.Title = "Add";
            var report = new HSSEReport
            {
                Company = "NR",
                ReportingDate = DateTime.Now
            };
            return View("Edit", report);
        }

        public ViewResult Edit(string id)
        {
            ViewBag.Title = "Edit";
            ViewBag.UploadPath = configuration["UploadPath:hsse"];
            var report = repository.HSSEReports.FirstOrDefault(a => a.HSSEReportID == id);
            return View(report);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(HSSEReport report)
        {
            var response = await apiHelper.UpdateStatusAsync(report.HSSEReportID);
            if (response == "ok")
            {
                repository.Save(report, "add");
                await apiHelper.DownloadFileAsync(report.DokumentasiEmergencyDrill);
                await apiHelper.DownloadFileAsync(report.DokumentasiManagementVisit);
                await apiHelper.DownloadFileAsync(report.DokumentasiSafetyMeeting);
                await apiHelper.DownloadFileAsync(report.DokumentasiToolboxMeeting);

                TempData["message"] = $"{report.HSSEReportID} has been approved";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = $"{report.HSSEReportID} failed to be approved. System error.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Delete(HSSEReport report)
        {
            repository.Delete(report);
            TempData["message"] = $"{report.HSSEReportID} has been deleted.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ICollection<IFormFile> DokumentasiSafetyMeeting, ICollection<IFormFile> DokumentasiToolboxMeeting, ICollection<IFormFile> DokumentasiEmergencyDrill, ICollection<IFormFile> DokumentasiManagementVisit, HSSEReport report)
        {
            if (ModelState.IsValid)
            {
                string mode;
                if (report.HSSEReportID == null)
                {
                    report.HSSEReportID = repository.GetNextID();
                    report.Company = "NR";
                    mode = "add";
                }
                else
                {
                    mode = "edit";
                }

                var uploadPath = configuration["UploadPath:hsse"];
                foreach (var file in DokumentasiSafetyMeeting)
                {
                    if (file.Length > 0)
                    {
                        var fileName = report.HSSEReportID + "_dsm" + Path.GetExtension(file.FileName);
                        using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        report.DokumentasiSafetyMeeting = fileName;
                        break;
                    }
                }
                foreach (var file in DokumentasiToolboxMeeting)
                {
                    if (file.Length > 0)
                    {
                        var fileName = report.HSSEReportID + "_dtm" + Path.GetExtension(file.FileName);
                        using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        report.DokumentasiToolboxMeeting = fileName;
                        break;
                    }
                }
                foreach (var file in DokumentasiEmergencyDrill)
                {
                    if (file.Length > 0)
                    {
                        var fileName = report.HSSEReportID + "_ded" + Path.GetExtension(file.FileName);
                        using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        report.DokumentasiEmergencyDrill = fileName;
                        break;
                    }
                }
                foreach (var file in DokumentasiManagementVisit)
                {
                    if (file.Length > 0)
                    {
                        var fileName = report.HSSEReportID + "_dmv" + Path.GetExtension(file.FileName);
                        using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        report.DokumentasiManagementVisit = fileName;
                        break;
                    }
                }

                repository.Save(report, mode);
                TempData["message"] = $"{report.HSSEReportID} has been saved";
                return RedirectToAction("Index");
            }
            else
            {
                // there is something wrong with the data values
                return View(report);
            }
        }

        [AllowAnonymous]
        public FileResult DownloadFile(string fileName)
        {
            var filepath = Path.Combine(configuration["UploadPath:hsse"], fileName);
            var fileBytes = System.IO.File.ReadAllBytes(filepath);
            var contentType = "image/" + Path.GetExtension(fileName).TrimStart('.');
            return File(fileBytes, contentType, fileName);
        }

        public IActionResult PdfReport()
        {
            return View();
        }

        // Public Area
        [AllowAnonymous]
        public ViewResult pIndex()
        {
            return View(repository.HSSEReports);
        }

        [AllowAnonymous]
        public ViewResult pView(string id)
        {
            ViewBag.UploadPath = configuration["UploadPath:hsse"];
            var report = repository.HSSEReports.FirstOrDefault(a => a.HSSEReportID == id);
            return View("pView", report);
        }
    }
}

