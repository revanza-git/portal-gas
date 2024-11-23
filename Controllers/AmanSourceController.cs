using Admin.Interfaces.Repositories;
using Admin.Models.Aman;
using Admin.Models.User;
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
    public class AmanSourceController : Controller
    {
        private IAmanSourceRepository repository;
        private UserManager<ApplicationUser> userManager;

        public AmanSourceController(UserManager<ApplicationUser> _userManager,IAmanSourceRepository repo)
        {
            userManager = _userManager;
            repository = repo;
        }

        public ViewResult Index()
        {
            return View(repository.AmanSources);
        }

        public ViewResult Add()
        {
            AmanSource source = new AmanSource();
            return View("Edit",source);
        }

        public ViewResult Edit(int ID)
        {
            AmanSource source = repository.AmanSources.FirstOrDefault(x => x.AmanSourceID == ID);
            return View(source);
        }

        [HttpPost]
        public IActionResult Edit(AmanSource source)
        {
            if (ModelState.IsValid)
            {
                repository.Save(source);
                TempData["message"] = $"Aman Source ID#{source.AmanSourceID} has been saved";
                return RedirectToAction("Index");
            }
            else
            {
                // there is something wrong with the data values
                return View(source);
            }
        }

        [HttpPost]
        public IActionResult Delete(AmanSource source)
        {
            repository.Delete(source);
            TempData["message"] = $"{source.Deskripsi} has been deleted.";
            return RedirectToAction("Index");
        }
    }
}
