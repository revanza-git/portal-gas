using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public class Gallery
    {
        public int GalleryID { get; set; }

        [Required]
        public String Deskripsi { get; set; }
        public DateTime CreatedOn { get; set; }
        public String Department { get; set; }
        public String Creator { get; set; }
    }
}
