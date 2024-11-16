using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public class Vendor
    {
        public String VendorID { get; set; }
        [Required]
        public String VendorName { get; set; }
        [Required]
        public String Email { get; set; }
        public int Status { get; set; }
    }
}
