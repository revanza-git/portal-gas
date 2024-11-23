using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Tra
{
    public class Tra
    {
        public string TraID { get; set; }

        [Required]
        public string Company { get; set; }

        [Required]
        public string Project { get; set; }

        [Required]
        public string PenanggungJawab { get; set; }

        [Required]
        public string Posisi { get; set; }

        public string DocNo { get; set; }
        public DateTime Date { get; set; }
        public int Status { get; set; }

        [Required]
        public string TeamLeader { get; set; }

        public string SponsorPekerjaan { get; set; }
        public string HSSE { get; set; }
        public string PimpinanPemilikWilayah { get; set; }
    }
}
