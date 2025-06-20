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

        public AmanController(UserManager<ApplicationUser> userManager, IAmanRepository repo, ICommonRepository common, IEmailRepository emailRepo, ApiHelper apiHelper, IConfiguration configuration, ViewRenderService viewRenderService, IConverter pdfConverter, IMemoryCache cache, ICacheService cacheService)
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
                var verifier = await userManager.FindByNameAsync(aman.Verifier);
                var responsible = await userManager.FindByNameAsync(aman.Responsible);

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

            // Use async save if available
            if (repository is EFAmanRepository efRepo)
            {
                await efRepo.SaveProgressAsync(aman);
            }
            else
            {
                repository.SaveProgress(aman);
            }
            
            // Phase 2 Optimization: Invalidate cache after progress update
            _cacheService.InvalidateAmanCache(aman.AmanID);

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
