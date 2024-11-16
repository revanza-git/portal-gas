using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public class Tra
    {
        public string TraID { get; set; }

        [Required]
        public String Company { get; set; }

        [Required]
        public String Project { get; set; }

        [Required]
        public String PenanggungJawab { get; set; }

        [Required]
        public String Posisi { get; set; }

        public String DocNo { get; set; }
        public DateTime Date { get; set; }
        public int Status { get; set; }

        [Required]
        public String TeamLeader { get; set; }

        public String SponsorPekerjaan { get; set; }
        public String HSSE { get; set; }
        public String PimpinanPemilikWilayah { get; set; }
    }
}
