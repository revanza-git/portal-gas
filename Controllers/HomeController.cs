﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Admin.Helpers;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using Admin.Models.User;
using Admin.Models.HSSE;
using Admin.Interfaces.Repositories;
using Admin.Interfaces.Services;
using Admin.Models.Tra;

namespace Admin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ICommonRepository repository;
        private readonly IHSSEReportRepository hrepository;
        private readonly ITraRepository jrepository;
        private readonly IGalleryRepository galleryRepository;
        private readonly INewsRepository newsRepository;
        private readonly INOCRepository nocRepository;
        private readonly ApiHelper apiHelper;

        public HomeController(UserManager<ApplicationUser> userManager, ICommonRepository repository, IHSSEReportRepository hrepository, ITraRepository jrepository, IGalleryRepository galleryRepository, INewsRepository newsRepository, INOCRepository nocRepository, IConfiguration configuration, ApiHelper apiHelper)
        {
            this.userManager = userManager;
            this.repository = repository;
            this.hrepository = hrepository;
            this.jrepository = jrepository;
            this.galleryRepository = galleryRepository;
            this.newsRepository = newsRepository;
            this.nocRepository = nocRepository;
            this.configuration = configuration;
            this.apiHelper = apiHelper;
        }

        [AllowAnonymous]
        public IActionResult Index(string daterange)
        {
            var (startdate, enddate) = GetDateRange(daterange);

            ViewBag.startdate = startdate;
            ViewBag.enddate = enddate;

            var cultureInfo = new CultureInfo("id-ID");
            var dtStart = DateTime.ParseExact(startdate, "dd/MM/yyyy", cultureInfo);
            var dtEnd = DateTime.ParseExact(enddate, "dd/MM/yyyy", cultureInfo);

            ViewBag.photos = galleryRepository.GetLastPhotos(5);
            ViewBag.video = galleryRepository.GetLastVideo();
            ViewBag.news = newsRepository.News
                .Where(x => x.Status == 2)
                .OrderByDescending(x => x.NewsID)
                .Take(3);

            ViewBag.at = GetAccidentTriangle(dtStart, dtEnd);
            ViewBag.jsonTop = GetTopNocJson(dtStart, dtEnd);

            return View();
        }

        [AllowAnonymous]
        public IActionResult DebugGallery()
        {
            try
            {
                var photos = galleryRepository.GetLastPhotos(10).ToList();
                var totalPhotos = galleryRepository.GetLastPhotos(1000).Count();
                var allGalleries = galleryRepository.Galleries.ToList();
                
                var debug = new
                {
                    TotalPhotosInDatabase = totalPhotos,
                    TotalGalleries = allGalleries.Count(),
                    LastPhotosRetrieved = photos.Count(),
                    Photos = photos.Select(p => new {
                        p.PhotoID,
                        p.Keterangan,
                        p.FileName,
                        p.GalleryID,
                        p.CreatedOn,
                        p.Department,
                        p.Creator,
                        FileExists = !string.IsNullOrEmpty(p.FileName) && 
                                   System.IO.File.Exists(System.IO.Path.Combine(configuration["UploadPath:photo"], p.FileName)),
                        ImageUrl = Url.Action("GetFileContent", "Gallery", new { ID = p.PhotoID }),
                        FullFilePath = System.IO.Path.Combine(configuration["UploadPath:photo"], p.FileName ?? "")
                    }).ToList(),
                    Galleries = allGalleries.Select(g => new {
                        g.GalleryID,
                        g.Deskripsi,
                        g.Department,
                        g.CreatedOn
                    }).ToList(),
                    UploadPath = configuration["UploadPath:photo"]
                };
                
                return Json(debug);
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }

        [AllowAnonymous]
        public IActionResult TestPhotoLoad(int id = 54)
        {
            try
            {
                var photo = galleryRepository.GetPhoto(id);
                if (photo == null)
                {
                    return Json(new { Error = $"Photo {id} not found in database" });
                }

                var filePath = System.IO.Path.Combine(configuration["UploadPath:photo"], photo.FileName);
                var fileExists = System.IO.File.Exists(filePath);
                
                return Json(new {
                    PhotoID = photo.PhotoID,
                    FileName = photo.FileName,
                    FilePath = filePath,
                    FileExists = fileExists,
                    ImageUrl = Url.Action("GetFileContent", "Gallery", new { ID = photo.PhotoID }),
                    Message = fileExists ? "File exists - image should load" : "File missing from disk"
                });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
        }

        [AllowAnonymous]
        public IActionResult TestDirectImageLoad(int id = 54)
        {
            try
            {
                var photo = galleryRepository.GetPhoto(id);
                if (photo == null)
                {
                    return NotFound($"Photo {id} not found");
                }

                var filePath = System.IO.Path.Combine(configuration["UploadPath:photo"], photo.FileName);
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found on disk");
                }

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var contentType = GetContentType(photo.FileName);
                
                return File(fileBytes, contentType, photo.FileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        private string GetContentType(string fileName)
        {
            var extension = System.IO.Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" or ".jfif" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "image/jpeg"
            };
        }

        private (string startdate, string enddate) GetDateRange(string daterange)
        {
            if (string.IsNullOrEmpty(daterange))
            {
                return (DateTime.Now.ToString("01/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"));
            }

            var dates = daterange.Split('-');
            if (dates.Length == 2)
            {
                return (dates[0].Trim(), dates[1].Trim());
            }

            return (DateTime.Now.ToString("01/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"));
        }

        private AccidentTriangle GetAccidentTriangle(DateTime dtStart, DateTime dtEnd)
        {
            var hsseReports = hrepository.HSSEReports
                .Where(x => x.ReportingDate >= dtStart && x.ReportingDate <= dtEnd)
                .ToList() ?? [];

            var nocs = nocRepository.NOCs
                .Where(x => x.EntryDate >= dtStart && x.EntryDate <= dtEnd)
                .ToList();

            return new AccidentTriangle
            {
                Fatality = hsseReports.Sum(x => x.NumberOfFatalityCase.GetValueOrDefault(0)),
                LostTimeInjury = hsseReports.Sum(x => x.NumberOfLTICase.GetValueOrDefault(0)),
                RestrictedWorkCase = hsseReports.Sum(x => x.NumberOfRWC.GetValueOrDefault(0)),
                MedicalTreatmentCase = hsseReports.Sum(x => x.NumberOfMTC.GetValueOrDefault(0)),
                FirstAid = hsseReports.Sum(x => x.NumberOfFirstAid.GetValueOrDefault(0)),
                NearMiss = nocs.Count(x => x.DaftarPengamatan == 1),
                UnsafeAndActCondition = nocs.Count(x => x.DaftarPengamatan == 2 || x.DaftarPengamatan == 3)
            };
        }

        private string GetTopNocJson(DateTime dtStart, DateTime dtEnd)
        {
            var topNoc = nocRepository.NOCs
                .Where(x => x.EntryDate >= dtStart && x.EntryDate < dtEnd)
                .GroupBy(group => group.NamaObserver)
                .Select(group => new
                {
                    Nama = group.Key,
                    Count = group.Count()
                })
                .Take(5);

            return JsonConvert.SerializeObject(topNoc);
        }

        public async Task<IActionResult> Index2()
        {
            var userName = userManager.GetUserName(User);
            var user = await userManager.FindByNameAsync(userName);
            var role = (await userManager.GetRolesAsync(user)).First();
            TempData["Role"] = role;

            ViewBag.name = user.Name;
            var departmentID = user.Department;
            ViewBag.department = repository.GetAllDepartments().FirstOrDefault(x => x.DepartmentID == departmentID)?.Deskripsi;

            var notifications = "";
            var n = 0;

            try
            {
                var json = await apiHelper.GetTrasAsync();
                var tras1 = JsonConvert.DeserializeObject<IEnumerable<Tra>>(json);

                foreach (var tra in tras1)
                {
                    if (tra.SponsorPekerjaan == userName)
                    {
                        n++;
                    }
                }
                if (n > 0)
                {
                    notifications += $"- Ada {n} JSA yang memerlukan review Anda selaku Sponsor Pekerjaan <a class='btn btn-xs btn-primary btn-round' href='/Tra/Pending'><i class='fa fa-eye'></i> Detail</a><br/>";
                }
            }
            catch (Exception)
            {
                notifications += "- Error. System can not access http://mitra.nusantararegas.com.";
            }

            n = 0;
            try
            {
                var tras2 = jrepository.Tras.Where(x => x.Status == 2 && x.HSSE == userName);
                foreach (var tra in tras2)
                {
                    n++;
                }
                if (n > 0)
                {
                    notifications += $"- Ada {n} JSA yang memerlukan review Anda selaku HSSE. <a class='btn btn-xs btn-primary btn-round' href='/Tra/Pending'><i class='fa fa-eye'></i> Detail</a><br/>";
                }
            }
            catch (Exception)
            {
                // Handle exception
            }

            n = 0;
            try
            {
                var tras3 = jrepository.Tras.Where(x => x.Status == 3 && x.PimpinanPemilikWilayah == userName);
                foreach (var tra in tras3)
                {
                    n++;
                }
                if (n > 0)
                {
                    notifications += $"- Ada {n} JSA yang memerlukan approval Anda selaku Pemilik Wilayah. <a class='btn btn-xs btn-primary btn-round' href='/Tra/Pending'><i class='fa fa-eye'></i> Detail</a><br/>";
                }
            }
            catch (Exception)
            {
                // Handle exception
            }

            if (departmentID == "105" && (role == "Admin" || role == "AtasanAdmin"))
            {
                try
                {
                    n = 0;
                    var json2 = await apiHelper.GetReportsAsync();
                    var reports = JsonConvert.DeserializeObject<IEnumerable<HSSEReport>>(json2);
                    foreach (var report in reports)
                    {
                        n++;
                    }

                    if (n > 0)
                    {
                        notifications += $"- Ada {n} HSSE Report yang memerlukan approval Anda selaku HSSE. <a class='btn btn-xs btn-primary btn-round' href='/HSSEReport/Pending'><i class='fa fa-eye'></i> Detail</a><br/>";
                    }
                }
                catch (Exception)
                {
                    // Handle exception
                }
            }

            ViewBag.notifications = notifications;
            return View();
        }

        public RedirectResult RedirectTo(string url)
        {
            return Redirect(url);
        }
    }
}
