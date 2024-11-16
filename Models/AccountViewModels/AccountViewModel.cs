using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.AccountViewModels
{
    public class AccountViewModel
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Department { get; set; }
        [Required]
        public string Role { get; set; }
        public Boolean GCG { get; set; }
        public Boolean GCGAdmin { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public String NIP { get; set; }

        public int? Jabatan { get; set; }

        public Boolean IsTkjp { get; set; }

    }
}
