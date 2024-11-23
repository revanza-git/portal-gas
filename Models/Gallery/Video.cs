using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Gallery
{
    public class Video
    {
        public int VideoID { get; set; }
        [Required]
        public string Keterangan { get; set; }
        public string FileName { get; set; }
        public int GalleryID { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Creator { get; set; }
        public string Department { get; set; }
        public int Status { get; set; }
    }
}
