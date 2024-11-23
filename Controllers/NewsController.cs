using Admin.Helpers;
using Admin.Interfaces.Repositories;
using Admin.Models.News;
using Admin.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    [Authorize]
    public class NewsController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly INewsRepository repository;
        private readonly ICommonRepository commonRepository;
        private readonly IEmailRepository emailRepository;
        private readonly ApiHelper apiHelper;
        private readonly IConfiguration configuration;

        public NewsController(UserManager<ApplicationUser> userManager, INewsRepository repo, IEmailRepository emailRepo, ICommonRepository common, ApiHelper apiHelper, IConfiguration configuration)
        {
            this.repository = repo;
            this.commonRepository = common;
            this.emailRepository = emailRepo;
            this.userManager = userManager;
            this.apiHelper = apiHelper;
            this.configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await userManager.FindByIdAsync(userManager.GetUserId(User));
                var department = user.Department;
                var news = repository.News.Where(x => x.Department == department).OrderByDescending(x => x.NewsID);
                return View(news);
            }
            catch
            {
                TempData["message"] = "System Error.";
                return null;
            }
        }

        public async Task<ViewResult> ViewNews(int id)
        {
            var user = await userManager.FindByIdAsync(userManager.GetUserId(User));
            var department = user.Department;
            var news = repository.News.FirstOrDefault(x => x.NewsID == id && x.Department == department);
            return View(news);
        }

        public ViewResult Add()
        {
            ViewBag.Title = "Add";
            var news = new News
            {
                PublishingDate = DateTime.Now
            };
            return View("Edit", news);
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Edit";
            var user = await userManager.FindByIdAsync(userManager.GetUserId(User));
            var department = user.Department;
            var news = repository.News.FirstOrDefault(x => x.NewsID == id && x.Department == department);
            if (news == null)
            {
                TempData["error"] = $"You cannot access News ID {id}";
                return RedirectToAction("Index");
            }
            return View("Edit", news);
        }

        [HttpPost]
        public IActionResult Delete(News news)
        {
            repository.Delete(news);
            TempData["message"] = $"News \"{news.Subject}\" has been deleted.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(News news, string action)
        {
            if (ModelState.IsValid)
            {
                var onlineUser = await userManager.FindByIdAsync(userManager.GetUserId(User));
                news.Author = userManager.GetUserName(User);
                news.Department = onlineUser.Department;

                if (action == "save")
                {
                    repository.Save(news);
                    TempData["message"] = $"News {news.NewsID} has been saved";
                }
                else if (action == "submit")
                {
                    repository.Submit(news);
                    var departmentUsers = userManager.Users.Where(x => x.Department == news.Department).ToList();
                    foreach (var departmentUser in departmentUsers)
                    {
                        var role = (await userManager.GetRolesAsync(departmentUser)).FirstOrDefault();
                        if (role == "AtasanAdmin")
                        {
                            var email = new Email
                            {
                                Receiver = departmentUser.Email,
                                Subject = "News Notification",
                                Message = $"Dear {onlineUser.Name},<br/><p>User {departmentUser.Name} has posted a news. Please log in to the Nusantara Regas Internal Portal to approve the news.</p>",
                                Schedule = DateTime.Now,
                                CreatedOn = DateTime.Now
                            };
                            emailRepository.Save(email);
                        }
                    }

                    await apiHelper.SendEmailAsync();
                    TempData["message"] = $"News {news.NewsID} has been submitted";
                }
                else if (action == "publish")
                {
                    repository.Publish(news);
                    TempData["message"] = $"News {news.NewsID} has been published";
                }

                return RedirectToAction("Index");
            }
            return View(news);
        }

        [AllowAnonymous]
        public ViewResult ViewContent(int id)
        {
            var news = repository.News.FirstOrDefault(x => x.NewsID == id);
            return View("View", news);
        }

        [AllowAnonymous]
        public ViewResult ViewByDepartment(string id)
        {
            ViewBag.Judul = "Berita " + commonRepository.GetDepartments().FirstOrDefault(x => x.DepartmentID == id)?.Deskripsi;
            var news = repository.News.Where(x => x.Department == id && x.Status == 2).OrderByDescending(x => x.NewsID);
            return View("List", news);
        }
    }
}

