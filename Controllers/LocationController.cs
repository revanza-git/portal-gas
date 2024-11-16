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
    public class LocationController : Controller
    {
        private ILocationRepository repository;
        private UserManager<ApplicationUser> userManager;

        public LocationController(UserManager<ApplicationUser> _userManager,ILocationRepository repo)
        {
            userManager = _userManager;
            repository = repo;
        }

        public ViewResult Index()
        {
            return View(repository.Locations);
        }

        public ViewResult Add()
        {
            Location location = new Location();
            return View("Edit",location);
        }

        public ViewResult Edit(int ID)
        {
            Location location = repository.Locations.FirstOrDefault(x => x.LocationID == ID);
            return View(location);
        }

        [HttpPost]
        public IActionResult Edit(Location location)
        {
            if (ModelState.IsValid)
            {
                repository.Save(location);
                TempData["message"] = $"Location ID#{location.LocationID} has been saved";
                return RedirectToAction("Index");
            }
            else
            {
                // there is something wrong with the data values
                return View(location);
            }
        }

        [HttpPost]
        public IActionResult Delete(Location location)
        {
            repository.Delete(location);
            TempData["message"] = $"Location {location.Deskripsi} has been deleted.";
            return RedirectToAction("Index");
        }
    }
}
