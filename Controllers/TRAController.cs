using Admin.Helpers;
using Admin.Interfaces.Repositories;
using Admin.Interfaces.Services;
using Admin.Models.Tra;
using Admin.Models.User;
using Admin.Models.Vendors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    [Authorize]
    public class TraController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly ITraRepository repository;
        private readonly ICommonRepository crepository;
        private readonly IProjectRepository prepository;
        private readonly ApiHelper apiHelper;
        private readonly IConfiguration configuration;

        public TraController(UserManager<ApplicationUser> userManager, IWebHostEnvironment environment, ITraRepository repository, ICommonRepository crepository, IProjectRepository prepository, ApiHelper apiHelper, IConfiguration configuration)
        {
            this.userManager = userManager;
            _environment = environment;
            this.repository = repository;
            this.crepository = crepository;
            this.prepository = prepository;
            this.apiHelper = apiHelper;
            this.configuration = configuration;
        }

        public ViewResult Index()
        {
            ViewBag.repository = repository;
            return View(repository.Tras);
        }

        public async Task<ViewResult> Pending()
        {
            ViewBag.repository = repository;
            var json = await apiHelper.GetTrasAsync();
            var obj = JsonConvert.DeserializeObject<IEnumerable<Tra>>(json);
            return View("Pending", obj.Where(x => x.SponsorPekerjaan == userManager.GetUserName(User)));
        }

        public ViewResult ViewAction(string id)
        {
            ViewBag.Title = "View";
            var tra = repository.Tras.FirstOrDefault(a => a.TraID == id);
            ViewBag.ProjectName = repository.GetProjectName(tra.Project);
            ViewBag.VendorName = repository.GetVendorName(tra.Company);
            ViewBag.Hazards = crepository.GetHazards();
            ViewBag.Workers = repository.GetWorkers(tra.TraID);
            ViewBag.ProjectTasks = repository.GetProjectTasks(tra.TraID);
            ViewBag.Status = repository.GetTraStatuses().FirstOrDefault(x => x.TraStatusID == tra.Status).Deskripsi;
            return View("Edit", tra);
        }

        public ViewResult Add()
        {
            const string VendorID = "V000";
            ViewBag.Title = "Add";
            ViewBag.CompanyName = "Nusantara Regas";
            ViewBag.Projects = repository.GetProjects(VendorID);
            var tra = new Tra { Company = VendorID };
            return View(tra);
        }

        public ViewResult Edit(string id)
        {
            ViewBag.Title = "Edit";
            var tra = repository.Tras.FirstOrDefault(a => a.TraID == id);
            ViewBag.ProjectName = repository.GetProjectName(tra.Project);
            ViewBag.VendorName = repository.GetVendorName(tra.Company);
            ViewBag.Hazards = crepository.GetHazards();
            ViewBag.Workers = repository.GetWorkers(tra.TraID);
            ViewBag.ProjectTasks = repository.GetProjectTasks(tra.TraID);
            ViewBag.Status = repository.GetTraStatuses().FirstOrDefault(x => x.TraStatusID == tra.Status).Deskripsi;
            return View(tra);
        }

        public async Task<IActionResult> ViewPending(string id)
        {
            try
            {
                ViewBag.Title = "View Pending Report";
                var jsonTra = await apiHelper.GetTraAsync(id);
                var tra = JsonConvert.DeserializeObject<Tra>(jsonTra);

                var jsonProjectTask = await apiHelper.GetProjectTasksAsync(id);
                var projectTasks = JsonConvert.DeserializeObject<IEnumerable<ProjectTask>>(jsonProjectTask);

                var jsonWorker = await apiHelper.GetWorkersAsync(id);
                var workers = JsonConvert.DeserializeObject<IEnumerable<Worker>>(jsonWorker);

                ViewBag.ProjectName = repository.GetProjectName(tra.Project);
                ViewBag.VendorName = repository.GetVendorName(tra.Company);
                ViewBag.Workers = workers;
                ViewBag.ProjectTasks = projectTasks;
                ViewBag.Status = repository.GetTraStatuses().FirstOrDefault(x => x.TraStatusID == tra.Status).Deskripsi;
                return View(tra);
            }
            catch (Exception)
            {
                TempData["message"] = "System error.";
                return RedirectToAction("Pending");
            }
        }

        [HttpPost]
        public IActionResult Edit(Tra tra)
        {
            if (ModelState.IsValid)
            {
                var project = prepository.Projects.FirstOrDefault(x => x.ProjectID == tra.Project);
                tra.SponsorPekerjaan = project.SponsorPekerjaan;
                tra.HSSE = project.HSSE;
                tra.PimpinanPemilikWilayah = project.PemilikWilayah;
                repository.Save(tra);
                TempData["message"] = $"{tra.TraID} has been saved";
                return RedirectToAction("Index");
            }
            else
            {
                // there is something wrong with the data values
                if (tra.TraID == null)
                {
                    ViewBag.Title = "Add";
                    ViewBag.CompanyName = "Nusantara Regas";
                    ViewBag.Projects = repository.GetProjects(tra.Company);
                    return View("Add", tra);
                }
                else
                {
                    ViewBag.ProjectName = repository.GetProjectName(tra.Project);
                    ViewBag.VendorName = repository.GetVendorName(tra.Company);
                    ViewBag.Workers = repository.GetWorkers(tra.TraID);
                    ViewBag.ProjectTasks = repository.GetProjectTasks(tra.TraID);
                    ViewBag.Status = repository.GetTraStatuses().FirstOrDefault(x => x.TraStatusID == tra.Status).Deskripsi;
                    return View("Edit", tra);
                }
            }
        }

        [HttpPost]
        public IActionResult EditTask(ProjectTask task)
        {
            if (ModelState.IsValid)
            {
                repository.SaveTask(task);
                TempData["message"] = $"{task.ProjectTaskID} has been saved";
                return RedirectToAction("Edit", new { ID = task.TraID });
            }
            else
            {
                // there is something wrong with the data values
                return View(task);
            }
        }

        [HttpPost]
        public IActionResult EditWorker(Worker worker)
        {
            if (ModelState.IsValid)
            {
                repository.SaveWorker(worker);
                TempData["message"] = $"WorkerID {worker.WorkerID} has been saved";
                return RedirectToAction("Edit", new { ID = worker.TraID });
            }
            else
            {
                // there is something wrong with the data values
                return View(worker);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetWorker(string WorkerID)
        {
            var worker = repository.GetWorker(WorkerID);
            return Json(worker);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetProjectTask(string ProjectTaskID)
        {
            var task = repository.GetProjectTask(ProjectTaskID);
            return Json(task);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(string TraID, string action)
        {
            if (action == "SubmitVendor")
            {
                try
                {
                    var jsonTra = await apiHelper.GetTraAsync(TraID);
                    var tra = JsonConvert.DeserializeObject<Tra>(jsonTra);

                    var jsonProjectTask = await apiHelper.GetProjectTasksAsync(TraID);
                    var projectTasks = JsonConvert.DeserializeObject<IEnumerable<ProjectTask>>(jsonProjectTask);

                    var jsonWorker = await apiHelper.GetWorkersAsync(TraID);
                    var workers = JsonConvert.DeserializeObject<IEnumerable<Worker>>(jsonWorker);
                    tra.Status = 2;
                    repository.Save(tra);

                    foreach (var projectTask in projectTasks)
                    {
                        repository.SaveTask(projectTask);
                    }

                    foreach (var worker in workers)
                    {
                        repository.SaveWorker(worker);
                    }

                    await apiHelper.UpdateStatusTraAsync(TraID);

                    TempData["message"] = $"{TraID} has been submitted.";
                }
                catch (Exception)
                {
                    TempData["message"] = $"{TraID} failed to be submitted.";
                }
            }
            else if (action == "Submit")
            {
                repository.UpdateStatus(TraID);
                TempData["message"] = $"{TraID} has been submitted.";
            }
            else if (action == "ReviewSponsor")
            {
                repository.UpdateStatus(TraID);
                TempData["message"] = $"{TraID} has been reviewed by Sponsor.";
            }
            else if (action == "ReviewHSSE")
            {
                repository.UpdateStatus(TraID);
                TempData["message"] = $"{TraID} has been reviewed by HSSE.";
            }
            else if (action == "ApprovePemilikWilayah")
            {
                repository.UpdateStatus(TraID);
                TempData["message"] = $"{TraID} has been approved by Pemilik Wilayah.";
            }
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public IActionResult Report(string ID)
        {
            var tra = repository.Tras.FirstOrDefault(a => a.TraID == ID);
            ViewBag.ProjectName = repository.GetProjectName(tra.Project);
            ViewBag.VendorName = repository.GetVendorName(tra.Company);
            ViewBag.Workers = repository.GetWorkers(tra.TraID);
            ViewBag.ProjectTasks = repository.GetProjectTasks(tra.TraID);
            ViewBag.Status = repository.GetTraStatuses().FirstOrDefault(x => x.TraStatusID == tra.Status).Deskripsi;

            return View(tra);
        }

        public async Task<IActionResult> ExportToPdf(string ID)
        {
            var hc = new HttpClient();
            var htmlContent = await hc.GetStringAsync($"http://{Request.Host}/Tra/Report/" + ID);
            var result = await hc.PostAsync("http://localhost:5000/api/pdf", new StringContent(htmlContent));

            HttpContext.Response.ContentType = "application/pdf";
            HttpContext.Response.Headers.Append("x-filename", "report.pdf");
            HttpContext.Response.Headers.Append("Access-Control-Expose-Headers", "x-filename");
            await result.Content.CopyToAsync(HttpContext.Response.Body);
            return new ContentResult();
        }

        // Public Area
        [AllowAnonymous]
        public ViewResult pIndex()
        {
            ViewBag.repository = repository;
            return View(repository.Tras);
        }

        [AllowAnonymous]
        public ViewResult pView(string ID)
        {
            var tra = repository.Tras.FirstOrDefault(a => a.TraID == ID);
            ViewBag.ProjectName = repository.GetProjectName(tra.Project);
            ViewBag.VendorName = repository.GetVendorName(tra.Company);
            ViewBag.Workers = repository.GetWorkers(tra.TraID);
            ViewBag.ProjectTasks = repository.GetProjectTasks(tra.TraID);
            ViewBag.Status = repository.GetTraStatuses().FirstOrDefault(x => x.TraStatusID == tra.Status).Deskripsi;
            return View("pView", tra);
        }
    }
}

