using Admin.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public class GalleryRepository : IGalleryRepository
    {
        private ApplicationDbContext context;
        public GalleryRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<Gallery> Galleries => context.Galleries;

        public void Save(Gallery gallery)
        {
            if (gallery.GalleryID == 0)
            {
                gallery.CreatedOn = DateTime.Now;
                context.Galleries.Add(gallery);
            }
            else
            {
                Gallery search = context.Galleries.FirstOrDefault(x => x.GalleryID == gallery.GalleryID);
                if (search != null)
                {
                    search.Deskripsi = gallery.Deskripsi;
                }
            }
            context.SaveChanges();
        }

        public void SavePhoto(Photo photo)
        {
            if (photo.PhotoID == 0)
            {
                context.Photos.Add(photo);
            }
            else
            {
                Photo search = context.Photos.FirstOrDefault(x => x.PhotoID == photo.PhotoID);
                if (search != null)
                {
                    search.Keterangan = photo.Keterangan;
                    if (photo.FileName != null)
                    {
                        search.CreatedOn = photo.CreatedOn;
                        search.FileName = photo.FileName;
                    }
                }
            }
            context.SaveChanges();
        }

        public IEnumerable<Photo> GetPhotos(int GalleryID)
        {
            return context.Photos.Where(x => x.GalleryID == GalleryID);
        }

        public Photo GetPhoto(int PhotoID)
        {
            return context.Photos.FirstOrDefault(x => x.PhotoID == PhotoID);
        }
        
        public IEnumerable<Photo> GetLastPhotos(int n)
        {
            return context.Photos.OrderByDescending(x => x.CreatedOn).Take(n);
        }
        
        public void DeletePhoto(Photo photo)
        {
            context.Photos.Remove(photo);
            context.SaveChanges();
        }

        public void SaveVideo(Video video)
        {
            if (video.VideoID == 0)
            {
                context.Videos.Add(video);
            }
            else
            {
                Video search = context.Videos.FirstOrDefault(x => x.VideoID == video.VideoID);
                if (search != null)
                {
                    search.Keterangan = video.Keterangan;
                    if (video.FileName != null)
                    {
                        search.CreatedOn = video.CreatedOn;
                        search.FileName = video.FileName;
                    }
                }
            }
            context.SaveChanges();
        }

        public IEnumerable<Video> GetVideos(int GalleryID)
        {
            return context.Videos.Where(x => x.GalleryID == GalleryID);
        }

        public Video GetVideo(int VideoID)
        {
            return context.Videos.FirstOrDefault(x => x.VideoID == VideoID);
        }

        public Video GetLastVideo()
        {
            Video video;
            try
            {
                video = context.Videos.OrderByDescending(x => x.CreatedOn).First();
            }
            catch (Exception)
            {
                video = null;
            }
            return video;
        }

        public void DeleteVideo(Video video)
        {
            context.Videos.Remove(video);
            context.SaveChanges();
        }

        public void DeleteGallery(Gallery gallery)
        {
            context.Galleries.Remove(gallery);
            context.SaveChanges();
        }
    }
}
