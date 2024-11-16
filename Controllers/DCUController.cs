using Admin.Helpers;
using Admin.Models;
using Admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
using System.Threading.Tasks;

namespace Admin.Controllers
{
    [Authorize]
    public class DCUController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        public static IConfigurationRoot Configuration { get; set; }

        private IWebHostEnvironment _environment;
        private IDCURepository repository;
        private ICommonRepository crepository;
        private IDCUService dcuService;

        public DCUController(UserManager<ApplicationUser> _userManager, IWebHostEnvironment environment, IDCURepository repo, ICommonRepository common, IDCUService _dcuService)
        {
            _environment = environment;
            repository = repo;
            crepository = common;
            userManager = _userManager;
            dcuService = _dcuService;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        [HttpGet]
        public string GetCurrentCultureDate()
        {
            return DateTime.Now.ToString();
        }

        public ViewResult Index()
        {
            ViewData["Title"] = "Nusantara Regas Daily Check Up (DCU)";

            ViewBag.JenisPekerjaan = crepository.GetJenisPekerjaan();
            return View(repository.DCUs.Where(x => x.Date.Day == DateTime.Now.Day && x.Date.Month == DateTime.Now.Month && x.Date.Year == DateTime.Now.Year));
        }

        public ViewResult GetDCUs(string startDate, string endDate)
        {
            ViewBag.JenisPekerjaan = crepository.GetJenisPekerjaan();
            CultureInfo provider = new CultureInfo("id-ID");
            DateTime dtStart = DateTime.ParseExact(startDate, "dd/MM/yyyy", provider);
            DateTime dtEnd = DateTime.ParseExact(endDate, "dd/MM/yyyy", provider);

            List<DCU> list = new List<DCU>();
            list = (from x in dcuService.DCU where x.Date >= dtStart && x.Date <= dtEnd.AddDays(1) select x).ToList();

            return View("List", list);
        }

        public ViewResult ViewAction(string Id)
        {
            ViewBag.JenisPekerjaan = crepository.GetJenisPekerjaan();

            DCU dcu = repository.DCUs.FirstOrDefault(a => a.DCUID == Id);
            return View("View", dcu);
        }

        public ViewResult Add()
        {
            ViewBag.Title = "Add";
            ViewBag.JenisPekerjaan = crepository.GetJenisPekerjaan();
            DCU dcu = new DCU();
            return View("Edit", dcu);
        }

        public IActionResult Edit(string Id)
        {
            ViewBag.Title = "Edit";
            ViewBag.JenisPekerjaan = crepository.GetJenisPekerjaan();
            DCU dcu = repository.DCUs.FirstOrDefault(x => x.DCUID == Id);
            if (dcu == null)
            {
                return RedirectToAction("Index");
            }
            return View(dcu);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ICollection<IFormFile> files, DCU dcu)
        {
            string mode = "edit";

            if (ModelState.IsValid)
            {
                if (dcu.DCUID == null)
                {
                    mode = "add";
                    dcu.DCUID = repository.GetNextID();
                }

                var UploadPath = Configuration["UploadPath:dcu"];
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        string FileName = dcu.DCUID.ToString().PadLeft(4, '0') + file.FileName.Substring(file.FileName.IndexOf('.'));
                        ViewBag.Title = file.ContentType;
                        using (var fileStream = new FileStream(Path.Combine(UploadPath, FileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        dcu.ContentType = file.ContentType;
                        dcu.Foto = FileName;
                    }
                }
                dcu.Date = DateTime.Now;
                repository.Save(dcu, mode);
                TempData["message"] = $"{dcu.DCUID} has been saved";
                return RedirectToAction("Index");
            }
            else
            {
                // there is something wrong with the data values
                ViewBag.Title = "Edit";
                ViewBag.JenisPekerjaan = crepository.GetJenisPekerjaan();

                return View(dcu);
            }
        }

        public FileResult DownloadFile(string ID)
        {
            DCU dcu = repository.DCUs.FirstOrDefault(x => x.DCUID == ID);
            var filepath = Path.Combine(Configuration["UploadPath:dcu"], dcu.Foto);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
            return File(fileBytes, dcu.ContentType, dcu.Foto);
        }

        [HttpPost]
        public IActionResult Delete(DCU dcu)
        {
            repository.Delete(dcu);
            TempData["message"] = $"{dcu.DCUID} has been deleted.";
            return RedirectToAction("Index");
        }
    }
}
