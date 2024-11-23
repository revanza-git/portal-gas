using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Gallery
{
    public class Gallery
    {
        public int GalleryID { get; set; }

        [Required]
        public string Deskripsi { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Department { get; set; }
        public string Creator { get; set; }
    }
}
