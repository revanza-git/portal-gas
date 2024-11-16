using Admin.Helpers;
using Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    [Authorize]
    public class AmanController : Controller
    {
        private readonly IAmanRepository repository;
        private readonly ICommonRepository crepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailRepository emailRepository;
        private readonly ApiHelper apiHelper;
        private readonly IConfiguration configuration;

        public AmanController(UserManager<ApplicationUser> userManager, IAmanRepository repo, ICommonRepository common, IEmailRepository emailRepo, ApiHelper apiHelper, IConfiguration configuration)
        {
            this.repository = repo;
            this.crepository = common;
            this.emailRepository = emailRepo;
            this.userManager = userManager;
            this.apiHelper = apiHelper;
            this.configuration = configuration;
        }

        public ViewResult Index()
        {
            ViewData["Title"] = "Action Management (AMAN)";
            ViewBag.Classifications = crepository.GetClassifications();
            ViewBag.Priorities = crepository.GetPriorities();
            ViewBag.Statuses = crepository.GetAmanStatuses();
            ViewBag.CorrectionTypes = crepository.GetAmanCorrectionTypes();
            ViewBag.Users = userManager.Users;

            if (User.IsInRole("AdminQM"))
            {
                return View(repository.Amans.ToList());
            }
            var amans = repository.Amans.Where(x => x.Creator == User.Identity.Name || x.Responsible == User.Identity.Name || x.Verifier == User.Identity.Name);
            return View(amans);
        }

        public ViewResult ViewAction(string id)
        {
            ViewBag.Locations = crepository.GetLocations();
            ViewBag.Classifications = crepository.GetClassifications();
            ViewBag.Priorities = crepository.GetPriorities();
            ViewBag.Responsibles = crepository.GetResponsibles();
            ViewBag.AmanSources = crepository.GetAmanSources();
            ViewBag.AmanStatuses = crepository.GetAmanStatuses();
            ViewBag.AmanStatuses2 = crepository.GetAmanStatuses();
            ViewBag.Reschedules = repository.GetReschedules(id);
            ViewBag.CorrectionTypes = crepository.GetAmanCorrectionTypes();
            ViewBag.Users = userManager.Users;
            var aman = repository.Amans.FirstOrDefault(a => a.AmanID == id);
            ViewBag.Auditors = aman.Auditors;
            return View("View", aman);
        }

        public ViewResult Edit(string id)
        {
            ViewBag.Title = "Edit";
            ViewBag.Locations = crepository.GetLocations();
            ViewBag.Classifications = crepository.GetClassifications();
            ViewBag.Priorities = crepository.GetPriorities();
            ViewBag.Responsibles = crepository.GetResponsibles();
            ViewBag.AmanSources = crepository.GetAmanSources();
            ViewBag.CorrectionTypes = crepository.GetAmanCorrectionTypes();
            ViewBag.Users = userManager.Users;
            var aman = repository.Amans.FirstOrDefault(a => a.AmanID == id);
            ViewBag.Auditors = aman.Auditors;
            return View(aman);
        }

        [HttpPost]
        public IActionResult Delete(Aman aman)
        {
            repository.Delete(aman);
            TempData["message"] = $"{aman.AmanID} has been deleted.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Aman aman)
        {
            if (ModelState.IsValid)
            {
                var verifier = await userManager.FindByNameAsync(aman.Verifier);
                var responsible = await userManager.FindByNameAsync(aman.Responsible);

                aman.Creator = userManager.GetUserName(User);
                aman.Department = responsible.Department;
                repository.Save(aman);
                aman = repository.Amans.FirstOrDefault(x => x.AmanID == aman.AmanID);

                var message = new StringBuilder();
                message.Append("<table>");
                message.Append($"<tr><td>No.</td><td>:</td><td>{aman.AmanID}</td></tr>");
                message.Append($"<tr><td>Start Date</td><td>:</td><td>{aman.StartDate:dd MMMM yyyy}</td></tr>");
                message.Append($"<tr><td>End Date</td><td>:</td><td>{aman.EndDate:dd MMMM yyyy}</td></tr>");
                message.Append($"<tr><td>Source</td><td>:</td><td>{crepository.GetAmanSources().FirstOrDefault(x => x.AmanSourceID == aman.Source)?.Deskripsi}</td></tr>");
                message.Append($"<tr><td>Location</td><td>:</td><td>{crepository.GetLocations().FirstOrDefault(x => x.LocationID == aman.Location)?.Deskripsi}</td></tr>");
                message.Append($"<tr><td>Findings / Opportunities</td><td>:</td><td>{aman.Findings}</td></tr>");
                message.Append($"<tr><td>Recommendation</td><td>:</td><td>{aman.Recommendation}</td></tr>");
                message.Append($"<tr><td>Creator</td><td>:</td><td>{(await userManager.FindByNameAsync(aman.Creator)).Name}</td></tr>");
                message.Append($"<tr><td>Responsible</td><td>:</td><td>{responsible.Name}</td></tr>");
                message.Append($"<tr><td>Email</td><td>:</td><td>{responsible.Email}</td></tr>");
                message.Append($"<tr><td>Verifier</td><td>:</td><td>{verifier.Name}</td></tr>");
                message.Append($"<tr><td>Auditor(s)</td><td>:</td><td>{aman.Auditors}</td></tr>");
                message.Append($"<tr><td>Status</td><td>:</td><td>{crepository.GetAmanStatuses().FirstOrDefault(x => x.AmanStatusID == aman.Status)?.Deskripsi}</td></tr>");
                message.Append("</table>");

                var email = new Email
                {
                    Receiver = responsible.Email,
                    Subject = "New AMAN Notification",
                    Message = $"Dear {responsible.Name},<br/><p>There is a new AMAN with detail:</p>{message}",
                    Schedule = DateTime.Now,
                    CreatedOn = DateTime.Now
                };
                emailRepository.Save(email);

                var email2 = new Email
                {
                    Receiver = verifier.Email,
                    Subject = "New AMAN Notification",
                    Message = $"Dear {verifier.Name},<br/><p>There is a new AMAN with detail:</p>{message}",
                    Schedule = DateTime.Now,
                    CreatedOn = DateTime.Now
                };
                emailRepository.Save(email2);

                await apiHelper.SendEmailAsync();

                TempData["message"] = $"{aman.AmanID} has been saved";
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Locations = crepository.GetLocations();
                ViewBag.Classifications = crepository.GetClassifications();
                ViewBag.Priorities = crepository.GetPriorities();
                ViewBag.Responsibles = crepository.GetResponsibles();
                ViewBag.AmanSources = crepository.GetAmanSources();
                ViewBag.CorrectionTypes = crepository.GetAmanCorrectionTypes();
                ViewBag.Users = userManager.Users;
                return View(aman);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProgress(ICollection<IFormFile> files, Aman aman)
        {
            var uploadPath = configuration["UploadPath:aman"];
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = aman.AmanID + Path.GetExtension(file.FileName);
                    using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    aman.ContentType = file.ContentType;
                    aman.FileName = fileName;
                }
            }

            repository.SaveProgress(aman);
            var search = repository.Amans.FirstOrDefault(x => x.AmanID == aman.AmanID);
            if (search.Verifier != null)
            {
                var message = new StringBuilder();
                message.Append("<br><b>Details of AMAN:</b><table>");
                message.Append($"<tr><td>No.</td><td>:</td><td>{search.AmanID}</td></tr>");
                message.Append($"<tr><td>Start Date</td><td>:</td><td>{search.StartDate:dd MMMM yyyy}</td></tr>");
                message.Append($"<tr><td>End Date</td><td>:</td><td>{search.EndDate:dd MMMM yyyy}</td></tr>");
                message.Append($"<tr><td>Source</td><td>:</td><td>{crepository.GetAmanSources().FirstOrDefault(x => x.AmanSourceID == search.Source)?.Deskripsi}</td></tr>");
                message.Append($"<tr><td>Location</td><td>:</td><td>{crepository.GetLocations().FirstOrDefault(x => x.LocationID == search.Location)?.Deskripsi}</td></tr>");
                message.Append($"<tr><td>Findings / Opportunities</td><td>:</td><td>{search.Findings}</td></tr>");
                message.Append($"<tr><td>Recommendation</td><td>:</td><td>{search.Recommendation}</td></tr>");
                message.Append($"<tr><td>Creator</td><td>:</td><td>{(await userManager.FindByNameAsync(search.Creator)).Name}</td></tr>");
                message.Append($"<tr><td>Responsible</td><td>:</td><td>{(await userManager.FindByNameAsync(search.Responsible)).Name}</td></tr>");
                message.Append($"<tr><td>Email</td><td>:</td><td>{(await userManager.FindByNameAsync(search.Responsible)).Email}</td></tr>");
                message.Append($"<tr><td>Verifier</td><td>:</td><td>{(await userManager.FindByNameAsync(search.Verifier)).Name}</td></tr>");
                message.Append($"<tr><td>Status</td><td>:</td><td>{crepository.GetAmanStatuses().FirstOrDefault(x => x.AmanStatusID == search.Status)?.Deskripsi}</td></tr>");
                message.Append("</table>");

                var verifier = await userManager.FindByNameAsync(search.Verifier);
                var message2 = aman.Progress == 100 ? "<br/>Please log in to the Nusantara Regas Internal Portal to close it." : "";
                var email = new Email
                {
                    Receiver = verifier.Email,
                    Subject = "AMAN Status Update Notification",
                    Message = $"Dear {verifier.Name},<br/><p>The progress of AMAN with ID {aman.AmanID} has been updated to {aman.Progress}%.</p>{message}{message2}",
                    Schedule = DateTime.Now,
                    CreatedOn = DateTime.Now
                };
                emailRepository.Save(email);

                await apiHelper.SendEmailAsync();
            }
            TempData["message"] = $"Progress {aman.AmanID} has been updated";
            return RedirectToAction("ViewAction", new { ID = aman.AmanID });
        }

        [HttpPost]
        public async Task<IActionResult> Reschedule(Reschedule reschedule)
        {
            var aman = repository.Amans.FirstOrDefault(x => x.AmanID == reschedule.AmanID);
            var verifier = await userManager.FindByNameAsync(aman.Verifier);

            var message = new StringBuilder();
            message.Append("<b>Details of AMAN:</b><br><table>");
            message.Append($"<tr><td>No.</td><td>:</td><td>{aman.AmanID}</td></tr>");
            message.Append($"<tr><td>Start Date</td><td>:</td><td>{aman.StartDate:dd MMMM yyyy}</td></tr>");
            message.Append($"<tr><td>End Date</td><td>:</td><td>{aman.EndDate:dd MMMM yyyy}</td></tr>");
            message.Append($"<tr><td>Source</td><td>:</td><td>{crepository.GetAmanSources().FirstOrDefault(x => x.AmanSourceID == aman.Source)?.Deskripsi}</td></tr>");
            message.Append($"<tr><td>Location</td><td>:</td><td>{crepository.GetLocations().FirstOrDefault(x => x.LocationID == aman.Location)?.Deskripsi}</td></tr>");
            message.Append($"<tr><td>Findings / Opportunities</td><td>:</td><td>{aman.Findings}</td></tr>");
            message.Append($"<tr><td>Recommendation</td><td>:</td><td>{aman.Recommendation}</td></tr>");
            message.Append($"<tr><td>Creator</td><td>:</td><td>{(await userManager.FindByNameAsync(aman.Creator)).Name}</td></tr>");
            message.Append($"<tr><td>Responsible</td><td>:</td><td>{(await userManager.FindByNameAsync(aman.Responsible)).Name}</td></tr>");
            message.Append($"<tr><td>Email</td><td>:</td><td>{(await userManager.FindByNameAsync(aman.Responsible)).Email}</td></tr>");
            message.Append($"<tr><td>Verifier</td><td>:</td><td>{(await userManager.FindByNameAsync(aman.Verifier)).Name}</td></tr>");
            message.Append($"<tr><td>Status</td><td>:</td><td>{crepository.GetAmanStatuses().FirstOrDefault(x => x.AmanStatusID == aman.Status)?.Deskripsi}</td></tr>");
            message.Append("</table>");

            message.Append("<br><b>Details of Reschedule Data:</b><br><table>");
            message.Append($"<tr><td>Old End Date</td><td>:</td><td>{reschedule.OldEndDate:dd MMMM yyyy}</td></tr>");
            message.Append($"<tr><td>New End Date</td><td>:</td><td>{reschedule.NewEndDate:dd MMMM yyyy}</td></tr>");
            message.Append($"<tr><td>Reason</td><td>:</td><td>{reschedule.Reason}</td></tr>");
            message.Append("</table>");

            var email = new Email
            {
                Receiver = verifier.Email,
                Subject = "AMAN Reschedule Notification",
                Message = $"Dear {verifier.Name},<br/><p>AMAN with ID {aman.AmanID} has been requested for reschedule.</p>{message}<br/>Please log in to the Nusantara Regas Internal Portal to approve or reject it.",
                Schedule = DateTime.Now,
                CreatedOn = DateTime.Now
            };
            emailRepository.Save(email);

            await apiHelper.SendEmailAsync();

            repository.SaveReschedule(reschedule);
            TempData["message"] = $"New schedule for {reschedule.AmanID} has been saved";
            return RedirectToAction("ViewAction", new { ID = reschedule.AmanID });
        }

        [HttpPost]
        public async Task<IActionResult> ApproveReschedule(Reschedule reschedule, string action)
        {
            var aman = repository.Amans.FirstOrDefault(x => x.AmanID == reschedule.AmanID);
            var responsible = await userManager.FindByNameAsync(aman.Responsible);

            if (action == "Approve")
            {
                var email = new Email
                {
                    Receiver = responsible.Email,
                    Subject = "AMAN Reschedule Approve Notification",
                    Message = $"Dear {responsible.Name},<br/><p>Reschedule request for {aman.AmanID} has been approved.",
                    Schedule = DateTime.Now,
                    CreatedOn = DateTime.Now
                };
                emailRepository.Save(email);
                await apiHelper.SendEmailAsync();
                repository.ApproveReschedule(reschedule);
                TempData["message"] = $"New schedule for {reschedule.AmanID} has been approved";
            }
            else if (action == "Reject")
            {
                var email = new Email
                {
                    Receiver = responsible.Email,
                    Subject = "AMAN Reschedule Reject Notification",
                    Message = $"Dear {responsible.Name},<br/><p>Reschedule request for {aman.AmanID} has been rejected by Verifier.",
                    Schedule = DateTime.Now,
                    CreatedOn = DateTime.Now
                };
                emailRepository.Save(email);
                await apiHelper.SendEmailAsync();
                repository.RejectReschedule(reschedule);
                TempData["message"] = $"New schedule for {reschedule.AmanID} has been rejected";
            }
            return RedirectToAction("ViewAction", new { ID = reschedule.AmanID });
        }

        public ViewResult Add()
        {
            ViewBag.Title = "Add";
            ViewBag.Locations = crepository.GetLocations();
            ViewBag.Classifications = crepository.GetClassifications();
            ViewBag.Priorities = crepository.GetPriorities();
            ViewBag.Responsibles = crepository.GetResponsibles();
            ViewBag.AmanSources = crepository.GetAmanSources();
            ViewBag.CorrectionTypes = crepository.GetAmanCorrectionTypes();
            ViewBag.Users = userManager.Users;
            var aman = new Aman
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                Status = 2
            };
            return View("Edit", aman);
        }

        public IActionResult Close(string amanId)
        {
            TempData["message"] = $"{amanId} has been closed";
            repository.Close(amanId);
            return RedirectToAction("Index");
        }

        public IActionResult Approve(string amanId)
        {
            TempData["message"] = $"{amanId} has been approved";
            repository.Approve(amanId);
            return RedirectToAction("Index");
        }

        public async Task<FileResult> DownloadFile(string id)
        {
            var aman = repository.Amans.FirstOrDefault(x => x.AmanID == id);
            var filePath = Path.Combine(configuration["UploadPath:aman"], aman.FileName);
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, aman.ContentType, aman.FileName);
        }

        [AllowAnonymous]
        public ViewResult pIndex(string daterange)
        {
            ViewData["Title"] = "Action Management (AMAN)";
            string startdate, enddate;
            if (string.IsNullOrEmpty(daterange))
            {
                startdate = DateTime.Now.ToString("01/MM/yyyy");
                enddate = DateTime.Now.ToString("dd/MM/yyyy");
            }
            else
            {
                var dates = daterange.Split('-');
                if (dates.Length == 2)
                {
                    startdate = dates[0].Trim();
                    enddate = dates[1].Trim();
                }
                else

                {
                    startdate = DateTime.Now.ToString("01/MM/yyyy");
                    enddate = DateTime.Now.ToString("dd/MM/yyyy");
                }
            }
            ViewBag.startdate = startdate;
            ViewBag.enddate = enddate;
            var cultureInfo = new CultureInfo("id-ID");
            var dtStart = DateTime.ParseExact(startdate, "dd/MM/yyyy", cultureInfo);
            var dtEnd = DateTime.ParseExact(enddate, "dd/MM/yyyy", cultureInfo);

            ViewBag.Classifications = crepository.GetClassifications();
            ViewBag.Priorities = crepository.GetPriorities();
            ViewBag.Statuses = crepository.GetAmanStatuses();
            ViewBag.Locations = crepository.GetLocations();
            ViewBag.Responsibles = crepository.GetResponsibles();
            ViewBag.AmanSources = crepository.GetAmanSources();
            ViewBag.Users = userManager.Users;

            ViewBag.Open = repository.Amans.Count(x => x.Status == 2 && x.StartDate >= dtStart && x.StartDate <= dtEnd);
            ViewBag.Closed = repository.Amans.Count(x => x.Status == 3 && x.StartDate >= dtStart && x.StartDate <= dtEnd);
            ViewBag.ClosedOntime = repository.Amans.Count(x => x.Status == 3 && x.StartDate >= dtStart && x.StartDate <= dtEnd && x.ClosingDate <= x.EndDate);
            ViewBag.ClosedOverdue = repository.Amans.Count(x => x.Status == 3 && x.StartDate >= dtStart && x.StartDate <= dtEnd && x.ClosingDate > x.EndDate);

            var graphs = new List<AmanGraph3>();
            var departments = crepository.GetAllDepartments();
            foreach (var department in departments)
            {
                var graph = new AmanGraph3
                {
                    department = department.Deskripsi,
                    open = repository.Amans.Count(x => x.Status == 2 && x.Department == department.DepartmentID && x.StartDate >= dtStart && x.StartDate <= dtEnd),
                    closed = repository.Amans.Count(x => x.Status == 3 && x.Department == department.DepartmentID && x.StartDate >= dtStart && x.StartDate <= dtEnd)
                };
                graphs.Add(graph);
            }
            ViewBag.Graph3Json = JsonConvert.SerializeObject(graphs);

            return View(repository.Amans.Where(x => x.Classification == 2 && x.Status >= 2 && x.StartDate >= dtStart && x.StartDate <= dtEnd).OrderByDescending(x => x.AmanID));
        }

        [AllowAnonymous]
        public ViewResult pView(string id)
        {
            ViewBag.Locations = crepository.GetLocations();
            ViewBag.Classifications = crepository.GetClassifications();
            ViewBag.Priorities = crepository.GetPriorities();
            ViewBag.Responsibles = crepository.GetResponsibles();
            ViewBag.AmanSources = crepository.GetAmanSources();
            ViewBag.AmanStatuses = crepository.GetAmanStatuses();
            ViewBag.AmanStatuses2 = crepository.GetAmanStatuses();
            ViewBag.Reschedules = repository.GetReschedules(id);
            var aman = repository.Amans.FirstOrDefault(a => a.AmanID == id && a.Classification == 2 && a.Status >= 2);
            return View("pView", aman);
        }

        [AllowAnonymous]
        public ViewResult LFE()
        {
            ViewBag.Classifications = crepository.GetClassifications();
            ViewBag.Priorities = crepository.GetPriorities();
            ViewBag.Statuses = crepository.GetAmanStatuses();
            return View("LFE", repository.Amans.Where(a => a.Classification == 2 && a.Status == 3 && a.Priority == 1));
        }

        [AllowAnonymous]
        public ViewResult ViewLFE(string id)
        {
            ViewBag.Locations = crepository.GetLocations();
            ViewBag.Classifications = crepository.GetClassifications();
            ViewBag.Priorities = crepository.GetPriorities();
            ViewBag.Responsibles = crepository.GetResponsibles();
            ViewBag.AmanSources = crepository.GetAmanSources();
            ViewBag.AmanStatuses = crepository.GetAmanStatuses();
            ViewBag.AmanStatuses2 = crepository.GetAmanStatuses();
            ViewBag.Reschedules = repository.GetReschedules(id);
            var aman = repository.Amans.FirstOrDefault(a => a.AmanID == id && a.Classification == 2 && a.Status == 3 && a.Priority == 1);
            return View("ViewLFE", aman);
        }

        [AllowAnonymous]
        public IActionResult Report(string id)
        {
            ViewBag.Title = "Edit";
            ViewBag.Locations = crepository.GetLocations();
            ViewBag.Classifications = crepository.GetClassifications();
            ViewBag.Priorities = crepository.GetPriorities();
            ViewBag.Responsibles = crepository.GetResponsibles();
            ViewBag.AmanSources = crepository.GetAmanSources();
            ViewBag.AmanStatuses = crepository.GetAmanStatuses();
            ViewBag.Reschedules = repository.GetReschedules(id);
            ViewBag.ReschedulesCount = repository.GetReschedules(id).Count();
            ViewBag.Users = userManager.Users;
            var aman = repository.Amans.FirstOrDefault(a => a.AmanID == id);
            return View(aman);
        }

        public async Task<IActionResult> ExportToPdf(string id)
        {
            var scheme = HttpContext.Request.Scheme;
            var pathBase = HttpContext.Request.PathBase.Value;
            var url = $"{scheme}://{Request.Host}{pathBase}/Aman/Report/{id}";

            var text = await new HttpClient().GetStringAsync(url);
            var pdfContent = await GeneratePdfAsync(text);
            var fileName = $"{id}.pdf";

            return File(pdfContent, "application/pdf", fileName);
        }

        private async Task<byte[]> GeneratePdfAsync(string htmlContent)
        {
            using var client = new HttpClient();
            var response = await client.PostAsync("https://api.html2pdf.app/v1/generate", new StringContent(JsonConvert.SerializeObject(new
            {
                html = htmlContent,
                apiKey = "your_api_key"
            }), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
