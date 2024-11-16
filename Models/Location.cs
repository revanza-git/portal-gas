using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public class Location
    {
        public int LocationID { get; set; }

        [Required]
        public String Deskripsi { get; set; }
    }
}
