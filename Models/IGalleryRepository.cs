using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public interface IGalleryRepository
    {
        IEnumerable<Gallery> Galleries { get; }
        void Save(Gallery gallery);
        void DeleteGallery(Gallery gallery);

        void SavePhoto(Photo photo);
        IEnumerable<Photo> GetPhotos(int GalleryID);
        Photo GetPhoto(int PhotoID);
        IEnumerable<Photo> GetLastPhotos(int n);
        void DeletePhoto(Photo photo);

        void SaveVideo(Video video);
        IEnumerable<Video> GetVideos(int GalleryID);
        Video GetVideo(int VideoID);
        Video GetLastVideo();
        void DeleteVideo(Video video);
    }
}
