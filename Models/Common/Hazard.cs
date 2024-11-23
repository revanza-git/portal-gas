using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Common
{
    public class Hazard
    {
        public int HazardID { get; set; }
        [Required]
        public string Deskripsi { get; set; }
    }
}
