using Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    [Authorize]
    public class GalleryController : Controller
    {
        public static IConfigurationRoot Configuration { get; set; }
        private UserManager<ApplicationUser> userManager;
        private IGalleryRepository repository;

        public GalleryController(UserManager<ApplicationUser> _userManager,IGalleryRepository repo)
        {
            repository = repo;
            userManager = _userManager;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public ViewResult Index()
        {
            // Get all news filter by department
            String department = userManager.FindByIdAsync(userManager.GetUserId(User)).Result.Department;
            IEnumerable<Gallery> galleries = repository.Galleries.Where(x => x.Department == department).OrderByDescending(x => x.GalleryID);

            return View(galleries);
        }

        public IActionResult ViewPhoto(int ID)
        {
            try
            {
                String department = userManager.FindByIdAsync(userManager.GetUserId(User)).Result.Department;
                Gallery gallery = repository.Galleries.FirstOrDefault(x => x.GalleryID == ID && x.Department == department);
                IEnumerable<Photo> photos = repository.GetPhotos(gallery.GalleryID);
                ViewBag.Gallery = gallery;
                return View(photos);
            }
            catch (Exception)
            {
                TempData["error"] = $"You can not access Photo ID {ID}";
                return RedirectToAction("Index");
            }
        }

        public IActionResult ViewVideo(int ID)
        {
            try
            {
                String department = userManager.FindByIdAsync(userManager.GetUserId(User)).Result.Department;
                Gallery gallery = repository.Galleries.FirstOrDefault(x => x.GalleryID == ID && x.Department == department);
                IEnumerable<Video> videos = repository.GetVideos(gallery.GalleryID);
                ViewBag.Gallery = gallery;
                return View(videos);
            }
            catch (Exception)
            {
                TempData["error"] = $"You can not access Video ID {ID}";
                return RedirectToAction("Index");
            }
        }

        public ViewResult Add()
        {
            ViewBag.Title = "Add";
            Gallery gallery = new Gallery();
            return View("Edit",gallery);
        }

        public ViewResult Edit(int Id)
        {
            ViewBag.Title = "Edit";
            Gallery gallery = repository.Galleries.FirstOrDefault(x => x.GalleryID == Id);
            return View(gallery);
        }

        [HttpPost]
        public IActionResult Edit(Gallery gallery)
        {
            if (ModelState.IsValid)
            {
                String department = userManager.FindByIdAsync(userManager.GetUserId(User)).Result.Department;
                gallery.Department = department;
                gallery.Creator = userManager.GetUserName(User);
                repository.Save(gallery);
                TempData["message"] = $"Gallery \"{gallery.Deskripsi}\" has been saved";
                return RedirectToAction("Index");
            }
            else
            {
                // there is something wrong with the data values
                return View(gallery);
            }
        }

        [HttpPost]
        public IActionResult DeleteGallery(int GalleryID)
        {
            Gallery gallery = repository.Galleries.FirstOrDefault(x => x.GalleryID == GalleryID);
            if (gallery != null)
            {
                List<Photo> photos = repository.GetPhotos(GalleryID).ToList();
                foreach (var photo in photos)
                {
                    repository.DeletePhoto(photo);
                    var UploadPath = Configuration["UploadPath:photo"];
                    String FileName = Path.Combine(UploadPath, photo.FileName);
                    if (System.IO.File.Exists(FileName))
                    {
                        System.IO.File.Delete(FileName);
                    }
                }

                List<Video> videos = repository.GetVideos(GalleryID).ToList();
                foreach (var video in videos)
                {
                    repository.DeleteVideo(video);
                    var UploadPath = Configuration["UploadPath:video"];
                    String FileName = Path.Combine(UploadPath, video.FileName);
                    if (System.IO.File.Exists(FileName))
                    {
                        System.IO.File.Delete(FileName);
                    }
                }

                repository.DeleteGallery(gallery);

                TempData["message"] = $"Gallery \"{gallery.Deskripsi}\" has been deleted";
            }
            return RedirectToAction("Index");
        }

        public IActionResult AddPhoto(int ID)
        {
            String department = userManager.FindByIdAsync(userManager.GetUserId(User)).Result.Department;
            Gallery gallery = repository.Galleries.FirstOrDefault(x => x.GalleryID == ID && x.Department == department);
            if (gallery != null)
            {
                ViewBag.Title = "Add Photo";
                Photo photo = new Photo();
                photo.GalleryID = ID;
                return View(photo);
            }
            else
            {
                TempData["error"] = $"You can not access Gallery ID {ID}";
                return RedirectToAction("Index");
            }
        }

        public ViewResult EditPhoto(int ID)
        {
            ViewBag.Title = "Edit Photo";
            Photo photo = repository.GetPhoto(ID);
            return View(photo);
        }

        [HttpPost]
        public IActionResult DeletePhoto(Photo photo)
        {
            repository.DeletePhoto(photo);
            var UploadPath = Configuration["UploadPath:photo"];
            String FileName = Path.Combine(UploadPath, photo.FileName);
            if (System.IO.File.Exists(FileName))
            {
                System.IO.File.Delete(FileName);
            }
            TempData["message"] = $"Photo {photo.PhotoID} has been deleted";
            return RedirectToAction("ViewPhoto",new { ID=photo.GalleryID });
        }


        [HttpPost]
        public async Task<IActionResult> EditPhoto(IFormFile file, Photo photo)
        {
            if (ModelState.IsValid)
            {
                var UploadPath = Configuration["UploadPath:photo"];
                if (file != null)
                {
                    if (photo.PhotoID > 0)
                    {
                        String FileName = Path.Combine(UploadPath, photo.FileName);
                        if (System.IO.File.Exists(FileName))
                        {
                            System.IO.File.Delete(FileName);
                        }
                    }

                    photo.CreatedOn = DateTime.Now;
                    photo.FileName = "IMG" + photo.CreatedOn.ToString("yyyyMMddHHmmss") + "g" + photo.GalleryID  + file.FileName.Substring(file.FileName.IndexOf('.'));
                    using (var fileStream = new FileStream(Path.Combine(UploadPath, photo.FileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
                else
                {
                    photo.FileName = null;
                }
                photo.Creator = userManager.GetUserName(User);
                photo.Department = userManager.FindByIdAsync(userManager.GetUserId(User)).Result.Department;

                repository.SavePhoto(photo);
                TempData["message"] = $"Photo {photo.PhotoID} has been saved";
                return RedirectToAction("ViewPhoto", new { ID = photo.GalleryID });
            }
            else
            {
                // there is something wrong with the data values
                return View(photo);
            }
        }

        [AllowAnonymous]
        public FileResult GetFileContent(int ID)
        {
            if (ID > 0)
            {
                Photo photo = repository.GetPhoto(ID);
                if (photo != null)
                {
                    var filepath = Path.Combine(Configuration["UploadPath:photo"],photo.FileName);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
                    String ContentType = "image/" + photo.FileName.Substring(photo.FileName.IndexOf('.') + 1);
                    return File(fileBytes, ContentType, photo.FileName);
                }
            }

            return null;
        }

        public IActionResult AddVideo(int ID)
        {
            String department = userManager.FindByIdAsync(userManager.GetUserId(User)).Result.Department;
            Gallery gallery = repository.Galleries.FirstOrDefault(x => x.GalleryID == ID && x.Department == department);
            if (gallery != null)
            {
                ViewBag.Title = "Add Video";
                Video video = new Video();
                video.GalleryID = ID;
                return View(video);
            }
            else
            {
                TempData["error"] = $"You can not access Gallery ID {ID}";
                return RedirectToAction("Index");
            }
        }

        public ViewResult EditVideo(int ID)
        {
            ViewBag.Title = "Edit Video";
            Video video = repository.GetVideo(ID);
            return View(video);
        }

        [HttpPost]
        public IActionResult DeleteVideo(Video video)
        {
            repository.DeleteVideo(video);
            var UploadPath = Configuration["UploadPath:video"];
            String FileName = Path.Combine(UploadPath, video.FileName);
            if (System.IO.File.Exists(FileName))
            {
                System.IO.File.Delete(FileName);
            }
            TempData["message"] = $"Video {video.VideoID} has been deleted";
            return RedirectToAction("ViewVideo",new { ID=video.GalleryID});
         }


        [HttpPost]
        public async Task<IActionResult> EditVideo(IFormFile file, Video video)
        {
            if (ModelState.IsValid)
            {
                var UploadPath = Configuration["UploadPath:video"];
                if (file != null)
                {
                    if (video.VideoID > 0)
                    {
                        String FileName = Path.Combine(UploadPath, video.FileName);
                        if (System.IO.File.Exists(FileName))
                        {
                            System.IO.File.Delete(FileName);
                        }
                    }

                    video.CreatedOn = DateTime.Now;
                    video.FileName = "IMG" + video.CreatedOn.ToString("yyyyMMddHHmmss") + "g" + video.GalleryID + file.FileName.Substring(file.FileName.IndexOf('.'));
                    using (var fileStream = new FileStream(Path.Combine(UploadPath, video.FileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
                else
                {
                    video.FileName = null;
                }
                video.Creator = userManager.GetUserName(User);
                video.Department = userManager.FindByIdAsync(userManager.GetUserId(User)).Result.Department;

                repository.SaveVideo(video);
                TempData["message"] = $"Video {video.VideoID} has been saved";
                return RedirectToAction("Index");
            }
            else
            {
                // there is something wrong with the data values
                return View(video);
            }
        }
        [AllowAnonymous]
        public FileResult GetVideoContent(int ID)
        {
            if (ID > 0)
            {
                Video video = repository.GetVideo(ID);
                if (video != null)
                {
                    var filepath = Path.Combine(Configuration["UploadPath:video"], video.FileName);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
                    String ContentType = "video/" + video.FileName.Substring(video.FileName.IndexOf('.') + 1);
                    return File(fileBytes, ContentType, video.FileName);
                }
            }

            return null;
        }

        // Public Area
        [AllowAnonymous]
        public ViewResult pIndex()
        {
            return View(repository.Galleries);
        }

        [AllowAnonymous]
        public ViewResult pViewPhoto(int ID)
        {
            Gallery gallery = repository.Galleries.FirstOrDefault(x => x.GalleryID == ID);
            IEnumerable<Photo> photos = repository.GetPhotos(gallery.GalleryID);
            ViewBag.Gallery = gallery;
            return View(photos);
        }

        [AllowAnonymous]
        public ViewResult pViewVideo(int ID)
        {
            Gallery gallery = repository.Galleries.FirstOrDefault(x => x.GalleryID == ID);
            IEnumerable<Video> videos = repository.GetVideos(gallery.GalleryID);
            ViewBag.Gallery = gallery;
            return View(videos);
        }
    }
}
