using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Admin.Models.User
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Department { get; set; }
        public bool GCG { get; set; }
        public bool GCGAdmin { get; set; }
        public bool CodeOfConduct { get; set; }
        public bool ConflictOfInterest { get; set; }
        public DateTime? CodeOfConductDt { get; set; }
        public DateTime? ConflictOfInterestDt { get; set; }
        public string NIP { get; set; }
        public int? Jabatan { get; set; }
        public bool IsTkjp { get; set; }
    }
}
