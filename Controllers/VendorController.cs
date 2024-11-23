using Admin.Interfaces.Services;
using Admin.Models.User;
using Admin.Models.Vendors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    [Authorize]
    public class VendorController : Controller
    {
        private readonly IVendorRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public VendorController(UserManager<ApplicationUser> userManager, IVendorRepository repository)
        {
            _userManager = userManager;
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View(_repository.Vendors);
        }

        public IActionResult Add()
        {
            var vendor = new Vendor();
            return View(vendor);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var vendor = await Task.Run(() => _repository.Vendors.FirstOrDefault(x => x.VendorID == id));
            if (vendor == null)
            {
                return NotFound();
            }

            ViewBag.Projects = _repository.GetProjects(vendor.VendorID);
            var users = _userManager.Users;
            var hsse = users.Where(x => x.Department == "105");
            var pemilikWilayah = users.Where(x => x.Department == "323" || x.Department == "231" || x.Department == "222");
            ViewBag.Users = users;
            ViewBag.HSSE = hsse;
            ViewBag.PemilikWilayah = pemilikWilayah;
            return View(vendor);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Vendor vendor)
        {
            if (ModelState.IsValid)
            {
                await _repository.SaveAsync(vendor);
                TempData["message"] = $"{vendor.VendorID} has been saved";
                return RedirectToAction("Index");
            }
            else
            {
                return View(vendor);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditProject(Project project)
        {
            if (ModelState.IsValid)
            {
                await _repository.SaveProjectAsync(project);
                TempData["message"] = $"{project.ProjectID} has been saved";
                return RedirectToAction("Edit", new { id = project.VendorID });
            }
            else
            {
                TempData["message"] = "System error. Please check input data.";
                return RedirectToAction("Edit", new { id = project.VendorID });
            }
        }

        public async Task<string> GetProjectById(string id)
        {
            var project = await Task.Run(() => _repository.GetProject(id));
            return JsonConvert.SerializeObject(project);
        }

    }
}
