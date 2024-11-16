using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public class PelaporanGratifikasi
    {
        public int PelaporanGratifikasiID { get; set; }
        public String UserID { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        [Range(0, 1)]
        public int AdaPenerimaanGratifikasi { get; set; }
        [Range(0, 1)]
        public int AdaPemberianGratifikasi { get; set; }
        [Range(0, 1)]
        public int AdaPermintaanGratifikasi { get; set; }
        public String DeskripsiPenerimaanGratifikasi { get; set; }
        public String DeskripsiPemberianGratifikasi { get; set; }
        public String DeskripsiPermintaanGratifikasi { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
