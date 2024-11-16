using Admin.Models;
using Admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    [Authorize]
    public class HazardController : Controller
    {
        private IHazardRepository repository;
        private UserManager<ApplicationUser> userManager;

        public HazardController(UserManager<ApplicationUser> _userManager,IHazardRepository repo)
        {
            userManager = _userManager;
            repository = repo;
        }

        public ViewResult Index()
        {
            return View(repository.hazards);
        }

        public ViewResult Add()
        {
            Hazard hazard = new Hazard();
            return View("Edit",hazard);
        }

        public ViewResult Edit(int ID)
        {
            Hazard hazard = repository.hazards.FirstOrDefault(x => x.HazardID == ID);
            return View(hazard);
        }

        [HttpPost]
        public IActionResult Edit(Hazard hazard)
        {
            if (ModelState.IsValid)
            {
                repository.Save(hazard);
                TempData["message"] = $"\"{hazard.Deskripsi}\" has been saved";
                return RedirectToAction("Index");
            }
            else
            {
                // there is something wrong with the data values
                return View(hazard);
            }
        }

        [HttpPost]
        public IActionResult Delete(Hazard hazard)
        {
            repository.Delete(hazard);
            TempData["message"] = $"\"{hazard.HazardID}\" has been deleted.";
            return RedirectToAction("Index");
        }
    }
}
