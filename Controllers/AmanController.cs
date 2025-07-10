using Admin.Helpers;
using Admin.Interfaces.Repositories;
using Admin.Models;
using Admin.Models.Aman;
using Admin.Models.User;
using Admin.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Admin.Services;

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
        private readonly ViewRenderService _viewRenderService;
        private readonly IConverter _pdfConverter;
        private readonly IMemoryCache _cache;
        private readonly ICacheService _cacheService;
        private readonly IEmailService _emailService;

        public AmanController(UserManager<ApplicationUser> userManager, IAmanRepository repo, ICommonRepository common, IEmailRepository emailRepo, ApiHelper apiHelper, IConfiguration configuration, ViewRenderService viewRenderService, IConverter pdfConverter, IMemoryCache cache, ICacheService cacheService, IEmailService emailService)
        {
            this.repository = repo;
            this.crepository = common;
            this.emailRepository = emailRepo;
            this.userManager = userManager;
            this.apiHelper = apiHelper;
            this.configuration = configuration;
            this._viewRenderService = viewRenderService;
            this._pdfConverter = pdfConverter;
            this._cache = cache;
            this._cacheService = cacheService;
            this._emailService = emailService;
        }

        [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "page", "size" })]
        public async Task<ViewResult> Index(int page = 1, int size = 20)
        {
            ViewData["Title"] = "Action Management (AMAN)";
            
            // Cache lookup keys
            var userRole = User.IsInRole("AdminQM") ? "AdminQM" : "User";
            var userName = User.Identity.Name;
            var cacheKey = $"amans_page_{page}_size_{size}_role_{userRole}_user_{userName}";
            var staticDataCacheKey = "aman_static_data";

            // Cache static data (classifications, priorities, etc.)
            if (!_cache.TryGetValue(staticDataCacheKey, out var staticData))
            {
                staticData = new
                {
                    Classifications = crepository.GetClassifications(),
                    Priorities = crepository.GetPriorities(),
                    Statuses = crepository.GetAmanStatuses(),
                    CorrectionTypes = crepository.GetAmanCorrectionTypes(),
                    Users = userManager.Users.ToList(),
                    AllDepartments = crepository.GetAllDepartments()
                };
                
                _cache.Set(staticDataCacheKey, staticData, TimeSpan.FromMinutes(15));
            }

            // Set ViewBag from cached data
            var cachedStaticData = staticData as dynamic;
            ViewBag.Classifications = cachedStaticData.Classifications;
            ViewBag.Priorities = cachedStaticData.Priorities;
            ViewBag.Statuses = cachedStaticData.Statuses;
            ViewBag.CorrectionTypes = cachedStaticData.CorrectionTypes;
            ViewBag.Users = cachedStaticData.Users;
            ViewBag.AllDepartments = cachedStaticData.AllDepartments;

            // Try to get cached paged results
            if (!_cache.TryGetValue(cacheKey, out var pagedResult))
            {
                // If repository has the new async method, use it
                if (repository is EFAmanRepository efRepo)
                {
                    pagedResult = await efRepo.GetPagedAmansAsync(page, size, userRole, userName);
                }
                else
                {
                    // Fallback to original logic for compatibility
                    IEnumerable<Aman> amans;
                    if (User.IsInRole("AdminQM"))
                    {
                        amans = repository.Amans.Skip((page - 1) * size).Take(size).ToList();
                    }
                    else
                    {
                        amans = repository.Amans
                            .Where(x => x.Creator == User.Identity.Name || x.Responsible == User.Identity.Name || x.Verifier == User.Identity.Name)
                            .Skip((page - 1) * size).Take(size);
                    }
                    pagedResult = (amans, amans.Count());
                }
                
                // Cache for 10 minutes
                _cache.Set(cacheKey, pagedResult, TimeSpan.FromMinutes(10));
            }

            var (items, totalCount) = ((IEnumerable<Aman>, int))pagedResult;
            
            // Set pagination data
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / size);

            return View(items);
        }

        [ResponseCache(Duration = 600, VaryByQueryKeys = new[] { "id" })]
        public async Task<ViewResult> ViewAction(string id)
        {
            var cacheKey = $"aman_view_{id}";
            var staticDataCacheKey = "aman_view_static_data";

            // Cache static data
            if (!_cache.TryGetValue(staticDataCacheKey, out var staticData))
            {
                staticData = new
                {
                    Locations = crepository.GetLocations(),
                    Classifications = crepository.GetClassifications(),
                    Priorities = crepository.GetPriorities(),
                    Responsibles = crepository.GetResponsibles(),
                    AmanSources = crepository.GetAmanSources(),
                    AmanStatuses = crepository.GetAmanStatuses(),
                    CorrectionTypes = crepository.GetAmanCorrectionTypes(),
                    Users = userManager.Users.ToList()
                };
                
                _cache.Set(staticDataCacheKey, staticData, TimeSpan.FromMinutes(30));
            }

            // Set ViewBag from cached data
            var cachedStaticData = staticData as dynamic;
            ViewBag.Locations = cachedStaticData.Locations;
            ViewBag.Classifications = cachedStaticData.Classifications;
            ViewBag.Priorities = cachedStaticData.Priorities;
            ViewBag.Responsibles = cachedStaticData.Responsibles;
            ViewBag.AmanSources = cachedStaticData.AmanSources;
            ViewBag.AmanStatuses = cachedStaticData.AmanStatuses;
            ViewBag.AmanStatuses2 = cachedStaticData.AmanStatuses;
            ViewBag.CorrectionTypes = cachedStaticData.CorrectionTypes;
            ViewBag.Users = cachedStaticData.Users;

            // Try to get cached aman data
            if (!_cache.TryGetValue(cacheKey, out var amanData))
            {
                Aman aman;
                IEnumerable<Reschedule> reschedules;

                if (repository is EFAmanRepository efRepo)
                {
                    aman = await efRepo.GetAmanByIdAsync(id);
                    reschedules = await efRepo.GetReschedulesAsync(id);
                }
                else
                {
                    aman = repository.Amans.FirstOrDefault(a => a.AmanID == id);
                    reschedules = repository.GetReschedules(id);
                }

                amanData = new { Aman = aman, Reschedules = reschedules };
                
                // Cache for 5 minutes (shorter because this data changes more frequently)
                _cache.Set(cacheKey, amanData, TimeSpan.FromMinutes(5));
            }

            var cachedAmanData = amanData as dynamic;
            ViewBag.Reschedules = cachedAmanData.Reschedules;
            ViewBag.Auditors = cachedAmanData.Aman.Auditors;

            return View("View", cachedAmanData.Aman);
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
                var responsible = await userManager.FindByNameAsync(aman.Responsible);
                var verifier = await userManager.FindByNameAsync(aman.Verifier);

                aman.Creator = userManager.GetUserName(User);
                aman.Department = responsible.Department;
                
                // Use async save if available
                if (repository is EFAmanRepository efRepo)
                {
                    await efRepo.SaveAsync(aman);
                }
                else
                {
                    repository.Save(aman);
                }
                
                // Phase 2 Optimization: Invalidate cache after save
                _cacheService.InvalidateAmanCache(aman.AmanID);
                _cacheService.InvalidateAmanCache(); // Invalidate list caches
                
                aman = repository.Amans.FirstOrDefault(x => x.AmanID == aman.AmanID);

                // Use templated email service instead of manual email creation
                var amanData = new
                {
                    AmanID = aman.AmanID,
                    RecipientName = responsible.Name,
                    StartDate = aman.StartDate.ToString("dd MMMM yyyy"),
                    EndDate = aman.EndDate.ToString("dd MMMM yyyy"),
                    Findings = aman.Findings,
                    Recommendation = aman.Recommendation,
                    Responsible = responsible.Name,
                    ResponsibleEmail = responsible.Email,
                    Verifier = verifier.Name,
                    VerifierEmail = verifier.Email
                };

                // Send to responsible person
                await _emailService.SendTemplatedEmailAsync(
                    "AMAN_NEW",
                    responsible.Email,
                    amanData,
                    "en", // or "id" for Indonesian
                    EmailPriority.High,
                    "AMAN"
                );

                // Send to verifier
                var verifierData = new
                {
                    AmanID = aman.AmanID,
                    RecipientName = verifier.Name,
                    StartDate = aman.StartDate.ToString("dd MMMM yyyy"),
                    EndDate = aman.EndDate.ToString("dd MMMM yyyy"),
                    Findings = aman.Findings,
                    Recommendation = aman.Recommendation,
                    Responsible = responsible.Name,
                    ResponsibleEmail = responsible.Email,
                    Verifier = verifier.Name,
                    VerifierEmail = verifier.Email
                };

                await _emailService.SendTemplatedEmailAsync(
                    "AMAN_NEW",
                    verifier.Email,
                    verifierData,
                    "en",
                    EmailPriority.High,
                    "AMAN"
                );

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
                    var fileName = aman.AmanID + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(file.FileName);
                    using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    aman.ContentType = file.ContentType;
                    aman.FileName = fileName;
                }
            }
            repository.SaveProgress(aman);

            if (aman.Status == 3)
            {
                var verifier = await userManager.FindByNameAsync(aman.Verifier);
                await _emailService.SendTemplatedEmailAsync(
                    "AMAN_PROGRESS",
                    verifier.Email,
                    new
                    {
                        AmanID = aman.AmanID,
                        RecipientName = verifier.Name,
                        Status = "Completed - Waiting for Verification",
                        Progress = 100,
                        StartDate = aman.StartDate.ToString("dd MMMM yyyy"),
                        EndDate = aman.EndDate.ToString("dd MMMM yyyy"),
                        Findings = aman.Findings,
                        Recommendation = aman.Recommendation,
                        Responsible = (await userManager.FindByNameAsync(aman.Responsible)).Name,
                        Creator = (await userManager.FindByNameAsync(aman.Creator)).Name
                    },
                    "id",
                    EmailPriority.Medium,
                    "AMAN"
                );
            }
            TempData["message"] = $"Progress {aman.AmanID} has been updated";
            return RedirectToAction("ViewAction", new { ID = aman.AmanID });
        }

        [HttpPost]
        public async Task<IActionResult> Reschedule(Reschedule reschedule)
        {
            var aman = repository.Amans.FirstOrDefault(x => x.AmanID == reschedule.AmanID);
            var verifier = await userManager.FindByNameAsync(aman.Verifier);
            var responsible = await userManager.FindByNameAsync(aman.Responsible);

            await _emailService.SendTemplatedEmailAsync(
                "AMAN_PROGRESS",
                verifier.Email,
                new
                {
                    AmanID = aman.AmanID,
                    RecipientName = verifier.Name,
                    Status = "Reschedule Request",
                    Progress = 0,
                    StartDate = aman.StartDate.ToString("dd MMMM yyyy"),
                    EndDate = aman.EndDate.ToString("dd MMMM yyyy"),
                    OldEndDate = reschedule.OldEndDate.ToString("dd MMMM yyyy"),
                    NewEndDate = reschedule.NewEndDate.ToString("dd MMMM yyyy"),
                    RescheduleReason = reschedule.Reason,
                    Findings = aman.Findings,
                    Recommendation = aman.Recommendation,
                    Responsible = responsible.Name,
                    ResponsibleEmail = responsible.Email,
                    Creator = (await userManager.FindByNameAsync(aman.Creator)).Name,
                    Source = crepository.GetAmanSources().FirstOrDefault(x => x.AmanSourceID == aman.Source)?.Deskripsi,
                    Location = crepository.GetLocations().FirstOrDefault(x => x.LocationID == aman.Location)?.Deskripsi
                },
                "id",
                EmailPriority.Medium,
                "AMAN"
            );

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
                await _emailService.SendTemplatedEmailAsync(
                    "AMAN_PROGRESS",
                    responsible.Email,
                    new
                    {
                        AmanID = aman.AmanID,
                        RecipientName = responsible.Name,
                        Status = "Reschedule Approved",
                        Progress = 0,
                        NextStep = "Continue with new deadline",
                        NewEndDate = reschedule.NewEndDate.ToString("dd MMMM yyyy")
                    },
                    "id",
                    EmailPriority.Medium,
                    "AMAN"
                );
                repository.ApproveReschedule(reschedule);
                TempData["message"] = $"New schedule for {reschedule.AmanID} has been approved";
            }
            else if (action == "Reject")
            {
                await _emailService.SendTemplatedEmailAsync(
                    "AMAN_PROGRESS",
                    responsible.Email,
                    new
                    {
                        AmanID = aman.AmanID,
                        RecipientName = responsible.Name,
                        Status = "Reschedule Rejected",
                        Progress = 0,
                        NextStep = "Continue with original deadline",
                        RejectionReason = "Reschedule request has been rejected by Verifier"
                    },
                    "id",
                    EmailPriority.Medium,
                    "AMAN"
                );
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

        private byte[] GeneratePdf(string htmlContent)
        {
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4
                },
                Objects = {
                    new ObjectSettings() {
                        HtmlContent = htmlContent,
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };
            return _pdfConverter.Convert(doc);
        }

        public async Task<IActionResult> ExportToPdf(string id)
        {
            var aman = repository.Amans.FirstOrDefault(a => a.AmanID == id);
            // Set up ViewBag as in Report action
            ViewBag.Locations = crepository.GetLocations();
            ViewBag.Classifications = crepository.GetClassifications();
            ViewBag.Priorities = crepository.GetPriorities();
            ViewBag.Responsibles = crepository.GetResponsibles();
            ViewBag.AmanSources = crepository.GetAmanSources();
            ViewBag.AmanStatuses = crepository.GetAmanStatuses();
            ViewBag.Reschedules = repository.GetReschedules(id);
            ViewBag.ReschedulesCount = repository.GetReschedules(id).Count();
            ViewBag.Users = userManager.Users;

            // Null checks for debugging
            if (ViewBag.Locations == null) throw new Exception("ViewBag.Locations is null");
            if (ViewBag.Classifications == null) throw new Exception("ViewBag.Classifications is null");
            if (ViewBag.Priorities == null) throw new Exception("ViewBag.Priorities is null");
            if (ViewBag.Responsibles == null) throw new Exception("ViewBag.Responsibles is null");
            if (ViewBag.AmanSources == null) throw new Exception("ViewBag.AmanSources is null");
            if (ViewBag.AmanStatuses == null) throw new Exception("ViewBag.AmanStatuses is null");
            if (ViewBag.Reschedules == null) throw new Exception("ViewBag.Reschedules is null");
            if (ViewBag.Users == null) throw new Exception("ViewBag.Users is null");

            var html = await _viewRenderService.RenderToStringAsync(this.ControllerContext, "Report", aman, this.ViewData);
            var pdfContent = GeneratePdf(html);
            var fileName = $"{id}.pdf";
            return File(pdfContent, "application/pdf", fileName);
        }
    }
}
