using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Vendors
{
    public class Vendor
    {
        public string VendorID { get; set; }
        [Required]
        public string VendorName { get; set; }
        [Required]
        public string Email { get; set; }
        public int Status { get; set; }
    }
}
