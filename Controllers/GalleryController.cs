using Admin.Interfaces.Repositories;
using Admin.Models;
using Admin.Models.Gallery;
using Admin.Models.User;
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

        public GalleryController(UserManager<ApplicationUser> _userManager, IGalleryRepository repo)
        {
            repository = repo;
            userManager = _userManager;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public async Task<ViewResult> Index()
        {
            // Get current user
            var user = await userManager.FindByIdAsync(userManager.GetUserId(User));
            var userRoles = await userManager.GetRolesAsync(user);
            
            // Check if user has admin roles
            var adminRoles = new[] { "Admin", "AtasanAdmin", "AdminQM", "AdminNOC" };
            bool isAdmin = userRoles.Any(role => adminRoles.Contains(role, StringComparer.OrdinalIgnoreCase));
            
            IEnumerable<Gallery> galleries;
            
            if (isAdmin)
            {
                // Admin users can see all galleries
                galleries = repository.Galleries.OrderByDescending(x => x.GalleryID);
            }
            else
            {
                // Regular users see only galleries from their department
                galleries = repository.Galleries.Where(x => x.Department == user.Department).OrderByDescending(x => x.GalleryID);
            }

            return View(galleries);
        }

        [AllowAnonymous]
        public IActionResult GetRandomPhotoFromGallery(int galleryId)
        {
            // Fetch the list of photos for the given GalleryID
            var photos = repository.GetPhotos(galleryId).ToList();
            if (photos == null || !photos.Any())
            {
                // Return a 404 Not Found result if no photos are found
                return NotFound($"No photos found for GalleryID {galleryId}");
            }

            // Randomize the PhotoID
            var random = new Random();
            var randomPhoto = photos[random.Next(photos.Count)];

            // Use the random PhotoID to fetch the photo
            var filepath = Path.Combine(Configuration["UploadPath:photo"], randomPhoto.FileName);
            if (!System.IO.File.Exists(filepath))
            {
                // Return a 404 Not Found result if the file is not found
                return NotFound("Photo file not found.");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filepath);
            var fileExtension = Path.GetExtension(randomPhoto.FileName).ToLowerInvariant();

            string contentType;
            switch (fileExtension)
            {
                case ".jpg":
                case ".jpeg":
                    contentType = "image/jpeg";
                    break;
                case ".png":
                    contentType = "image/png";
                    break;
                default:
                    contentType = "application/octet-stream"; // Default to binary stream if unknown
                    break;
            }

            return File(fileBytes, contentType, randomPhoto.FileName);
        }

        public async Task<IActionResult> ViewPhoto(int ID)
        {
            try
            {
                // Get current user
                var user = await userManager.FindByIdAsync(userManager.GetUserId(User));
                var userRoles = await userManager.GetRolesAsync(user);
                
                // Check if user has admin roles
                var adminRoles = new[] { "admin", "atasanadmin", "adminQM", "adminNOC" };
                bool isAdmin = userRoles.Any(role => adminRoles.Contains(role, StringComparer.OrdinalIgnoreCase));
                
                Gallery gallery;
                
                if (isAdmin)
                {
                    // Admin users can view galleries from all departments
                    gallery = repository.Galleries.FirstOrDefault(x => x.GalleryID == ID);
                }
                else
                {
                    // Regular users can only view galleries from their department
                    gallery = repository.Galleries.FirstOrDefault(x => x.GalleryID == ID && x.Department == user.Department);
                }
                
                if (gallery == null)
                {
                    TempData["error"] = $"You can not access Photo ID {ID}";
                    return RedirectToAction("Index");
                }
                
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

        public async Task<IActionResult> ViewVideo(int ID)
        {
            try
            {
                // Get current user
                var user = await userManager.FindByIdAsync(userManager.GetUserId(User));
                var userRoles = await userManager.GetRolesAsync(user);
                
                // Check if user has admin roles
                var adminRoles = new[] { "admin", "atasanadmin", "adminQM", "adminNOC" };
                bool isAdmin = userRoles.Any(role => adminRoles.Contains(role, StringComparer.OrdinalIgnoreCase));
                
                Gallery gallery;
                
                if (isAdmin)
                {
                    // Admin users can view galleries from all departments
                    gallery = repository.Galleries.FirstOrDefault(x => x.GalleryID == ID);
                }
                else
                {
                    // Regular users can only view galleries from their department
                    gallery = repository.Galleries.FirstOrDefault(x => x.GalleryID == ID && x.Department == user.Department);
                }
                
                if (gallery == null)
                {
                    TempData["error"] = $"You can not access Video ID {ID}";
                    return RedirectToAction("Index");
                }
                
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
            return View("Edit", gallery);
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

        public async Task<IActionResult> AddPhoto(int ID)
        {
            // Get current user
            var user = await userManager.FindByIdAsync(userManager.GetUserId(User));
            var userRoles = await userManager.GetRolesAsync(user);
            
            // Check if user has admin roles
            var adminRoles = new[] { "admin", "atasanadmin", "adminQM", "adminNOC" };
            bool isAdmin = userRoles.Any(role => adminRoles.Contains(role, StringComparer.OrdinalIgnoreCase));
            
            Gallery gallery;
            
            if (isAdmin)
            {
                // Admin users can add photos to galleries from all departments
                gallery = repository.Galleries.FirstOrDefault(x => x.GalleryID == ID);
            }
            else
            {
                // Regular users can only add photos to galleries from their department
                gallery = repository.Galleries.FirstOrDefault(x => x.GalleryID == ID && x.Department == user.Department);
            }
            
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
            return RedirectToAction("ViewPhoto", new { ID = photo.GalleryID });
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
                    photo.FileName = "IMG" + photo.CreatedOn.ToString("yyyyMMddHHmmss") + "g" + photo.GalleryID + file.FileName.Substring(file.FileName.IndexOf('.'));
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
            try
            {
                if (ID <= 0)
                {
                    throw new ArgumentException($"Invalid PhotoID: {ID}");
                }

                Photo photo = repository.GetPhoto(ID);
                if (photo == null)
                {
                    throw new ArgumentException($"Photo not found for ID: {ID}");
                }

                if (string.IsNullOrEmpty(photo.FileName))
                {
                    throw new ArgumentException($"FileName is null or empty for PhotoID: {ID}");
                }

                var filepath = Path.Combine(Configuration["UploadPath:photo"], photo.FileName);
                
                if (!System.IO.File.Exists(filepath))
                {
                    throw new FileNotFoundException($"File not found: {filepath}");
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
                string ContentType = DetermineContentType(photo.FileName);
                
                return File(fileBytes, ContentType, photo.FileName);
            }
            catch (Exception ex)
            {
                // Log the error (you can replace this with your logging framework)
                System.Diagnostics.Debug.WriteLine($"GetFileContent Error for ID {ID}: {ex.Message}");
                
                // Return a 1x1 pixel transparent PNG as placeholder
                byte[] transparentPixel = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==");
                return File(transparentPixel, "image/png", "placeholder.png");
            }
        }

        private string DetermineContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                ".jfif" => "image/jpeg",
                _ => "image/jpeg" // Default fallback
            };
        }

        public async Task<IActionResult> AddVideo(int ID)
        {
            // Get current user
            var user = await userManager.FindByIdAsync(userManager.GetUserId(User));
            var userRoles = await userManager.GetRolesAsync(user);
            
            // Check if user has admin roles
            var adminRoles = new[] { "admin", "atasanadmin", "adminQM", "adminNOC" };
            bool isAdmin = userRoles.Any(role => adminRoles.Contains(role, StringComparer.OrdinalIgnoreCase));
            
            Gallery gallery;
            
            if (isAdmin)
            {
                // Admin users can add videos to galleries from all departments
                gallery = repository.Galleries.FirstOrDefault(x => x.GalleryID == ID);
            }
            else
            {
                // Regular users can only add videos to galleries from their department
                gallery = repository.Galleries.FirstOrDefault(x => x.GalleryID == ID && x.Department == user.Department);
            }
            
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
            return RedirectToAction("ViewVideo", new { ID = video.GalleryID });
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
        public IActionResult GetVideoContent(int ID)
        {
            if (ID <= 0)
            {
                return NotFound("Invalid video ID.");
            }

            var video = repository.GetVideo(ID);
            if (video == null)
            {
                return NotFound($"Video with ID {ID} not found.");
            }

            var videoFilePath = Path.Combine(Configuration["UploadPath:video"], video.FileName);
            if (!System.IO.File.Exists(videoFilePath))
            {
                return NotFound("Video file not found.");
            }

            var fileBytes = System.IO.File.ReadAllBytes(videoFilePath);
            var fileExtension = Path.GetExtension(video.FileName).TrimStart('.');
            var contentType = $"video/{fileExtension}";

            return File(fileBytes, contentType, video.FileName);
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
