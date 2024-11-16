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
using System.Security.Claims;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    [Authorize]
    public class OvertimeController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly ICommonRepository crepository;
        private readonly ISDMRepository _sdmRepository;
        private readonly ISDMService _sdmService;
        private readonly IConfiguration _configuration;

        public OvertimeController(UserManager<ApplicationUser> userManager, IWebHostEnvironment environment, ISDMRepository sdmRepository, ISDMService sdmService, ICommonRepository common, IConfiguration configuration)
        {
            _environment = environment;
            _sdmRepository = sdmRepository;
            _sdmService = sdmService;
            crepository = common;
            this.userManager = userManager;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Nusantara Regas Overtime Dashboard";
            ViewBag.OvertimeStatus = crepository.GetOvertimeStatuses();
            ViewBag.Department = crepository.GetAllDepartments();
            ViewBag.Jabatan = crepository.GetJabatan();
            IEnumerable<ApplicationUser> users = userManager.Users;
            ViewBag.Users = users;
            ViewBag.Subdir = _configuration["Url:Subdir"];

            string onlineUser = userManager.GetUserName(User);
            ApplicationUser user = userManager.Users.FirstOrDefault(x => x.UserName == onlineUser);

            if (User.IsInRole("AdminSDM") || User.IsInRole("SecretarySDM"))
            {
                return View(_sdmRepository.Overtime.ToList());
            }
            return View(_sdmRepository.Overtime.Where(x => x.Department.ToString() == user.Department).ToList());
        }

        public IActionResult GetOvertimes(string startDate, string endDate)
        {
            ViewBag.OvertimeStatus = crepository.GetOvertimeStatuses();
            ViewBag.Department = crepository.GetAllDepartments();
            ViewBag.Jabatan = crepository.GetJabatan();
            IEnumerable<ApplicationUser> users = userManager.Users;
            ViewBag.Users = users;

            CultureInfo provider = new CultureInfo("id-ID");
            DateTime dtStart = DateTime.ParseExact(startDate, "dd/MM/yyyy", provider);
            DateTime dtEnd = DateTime.ParseExact(endDate, "dd/MM/yyyy", provider);

            string onlineUser = userManager.GetUserName(User);
            ApplicationUser user = userManager.Users.FirstOrDefault(x => x.UserName == onlineUser);

            List<Overtime> list = new List<Overtime>();
            if (User.IsInRole("AdminSDM") || User.IsInRole("SecretarySDM"))
            {
                list = _sdmService.Overtimes.Where(x => x.Tanggal >= dtStart && x.Tanggal <= dtEnd.AddDays(1)).ToList();
            }
            else
            {
                list = _sdmService.Overtimes.Where(x => x.Tanggal >= dtStart && x.Tanggal <= dtEnd.AddDays(1) && x.Department.ToString() == user.Department).ToList();
            }

            return View("List", list);
        }

        public IActionResult GetRecap(string bulan, string username, string tahun)
        {
            try
            {
                if (string.IsNullOrEmpty(tahun))
                {
                    tahun = DateTime.Now.Year.ToString();
                }
                List<Recap> list = _sdmRepository.getRecap(int.Parse(bulan), username, int.Parse(tahun));

                return View("RecapList", list);
            }
            catch
            {
                return View("RecapList");
            }
        }

        public IActionResult Recap()
        {
            ViewBag.Title = "Recap";
            ViewBag.Jabatan = crepository.GetJabatan();
            ViewBag.Atasan = crepository.GetAtasan();
            IEnumerable<ApplicationUser> users = userManager.Users;

            string onlineUser = userManager.GetUserName(User);
            ApplicationUser userObject = userManager.Users.FirstOrDefault(x => x.UserName == onlineUser);

            if (User.IsInRole("AdminSDM") || User.IsInRole("SecretarySDM"))
            {
                ViewBag.Tkjp = users.Where(x => x.IsTkjp == true);
            }
            else
            {
                ViewBag.Tkjp = users.Where(x => x.Department == userObject.Department);
            }

            return View("RecapIndex");
        }

        public IActionResult View(int id)
        {
            ViewBag.Title = "View";
            ViewBag.OvertimeStatus = crepository.GetOvertimeStatuses();
            ViewBag.Department = crepository.GetAllDepartments();
            ViewBag.Jabatan = crepository.GetJabatan();
            ViewBag.Atasan = crepository.GetAtasan();
            ViewBag.JamKerjaStatus = crepository.GetJamKerjaStatuses();

            IEnumerable<ApplicationUser> users = userManager.Users;
            ViewBag.Users = users;

            Overtime overtime = _sdmRepository.Overtime.FirstOrDefault(a => a.OvertimeID == id);

            return View(overtime);
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Edit";
            ViewBag.OvertimeStatus = crepository.GetOvertimeStatuses();
            ViewBag.Department = crepository.GetAllDepartments();
            ViewBag.Jabatan = crepository.GetJabatan();
            ViewBag.Atasan = crepository.GetAtasan();
            ViewBag.JamKerjaStatus = crepository.GetJamKerjaStatuses();

            IEnumerable<ApplicationUser> users = userManager.Users;
            ViewBag.Users = users;

            Overtime overtime = _sdmRepository.Overtime.FirstOrDefault(a => a.OvertimeID == id);

            return View(overtime);
        }

        public IActionResult EditSuperior(int id)
        {
            ViewBag.Title = "Edit";
            ViewBag.OvertimeStatus = crepository.GetOvertimeStatuses();
            ViewBag.Department = crepository.GetAllDepartments();
            ViewBag.Jabatan = crepository.GetJabatan();
            ViewBag.Atasan = crepository.GetAtasan();
            ViewBag.JamKerjaStatus = crepository.GetJamKerjaStatuses();

            IEnumerable<ApplicationUser> users = userManager.Users;
            ViewBag.Users = users;

            Overtime overtime = _sdmRepository.Overtime.FirstOrDefault(a => a.OvertimeID == id);

            return View(overtime);
        }

        [HttpPost]
        public IActionResult Edit(Overtime overtime)
        {
            string mode = overtime.OvertimeID == 0 ? "add" : "edit";

            if (ModelState.IsValid)
            {
                _sdmRepository.SaveOvertime(overtime, mode);
                TempData["message"] = "Data has been saved";

                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.OvertimeStatus = crepository.GetOvertimeStatuses();
                ViewBag.Department = crepository.GetAllDepartments();
                ViewBag.Jabatan = crepository.GetJabatan();
                ViewBag.Atasan = crepository.GetAtasan();

                return View(overtime);
            }
        }

        [HttpPost]
        public IActionResult Delete(Overtime overtime)
        {
            _sdmRepository.DeleteOvertime(overtime);
            TempData["message"] = "Data has been deleted.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Approve([FromBody] string[] data)
        {
            if (data.Length > 0)
            {
                _sdmRepository.BatchApproval(data);
                TempData["message"] = "Data has been Approved";
                return Ok();
            }
            else
            {
                TempData["message"] = "No Data Selected";
                return BadRequest();
            }
        }
    }
}
