using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public class Video
    {
        public int VideoID { get; set; }
        [Required]
        public String Keterangan { get; set; }
        public String FileName { get; set; }
        public int GalleryID { get; set; }
        public DateTime CreatedOn { get; set; }
        public String Creator { get; set; }
        public String Department { get; set; }
        public int Status { get; set; }
    }
}
