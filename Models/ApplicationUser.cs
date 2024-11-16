using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Admin.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public String Name { get; set; }
        [Required]
        public String Department { get; set; }
        public Boolean GCG { get; set; }
        public Boolean GCGAdmin { get; set; }
        public Boolean CodeOfConduct { get; set; }
        public Boolean ConflictOfInterest { get; set; }
        public DateTime? CodeOfConductDt { get; set; }
        public DateTime? ConflictOfInterestDt { get; set; }
        public String NIP { get; set; }
        public int? Jabatan { get; set; }
        public Boolean IsTkjp { get; set; }
    }
}
