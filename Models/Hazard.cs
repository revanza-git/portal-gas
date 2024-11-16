using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public class Hazard
    {
        public int HazardID { get; set; }
        [Required]
        public String Deskripsi { get; set; }
    }
}
