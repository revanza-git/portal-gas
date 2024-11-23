using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Aman
{
    public class Aman
    {
        public string AmanID { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public DateTime? ClosingDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "The Source field is required")]
        public int Source { get; set; }

        [Required]
        public int Location { get; set; }
        [Required]
        public string Findings { get; set; }
        [Required]
        public string Recommendation { get; set; }
        [Required]
        public string Responsible { get; set; }
        public int CorrectionType { get; set; }
        public int Status { get; set; }
        [Required]
        public string Verifier { get; set; }
        [Required]
        public int Priority { get; set; }
        [Required]
        public int Classification { get; set; }
        public int Progress { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public string Department { get; set; }
        public string Creator { get; set; }
        public DateTime CreationDateTime { get; set; }
        public int OverdueNotif { get; set; }
        public string Auditors { get; set; }

        public string NOCID { get; set; }
    }
}