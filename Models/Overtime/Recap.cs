using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Overtime
{
    public class Recap
    {
        public string TanggalAbsensi { get; set; }


        public string JamMulaiKerja { get; set; }


        public string JamSelesaiKerja { get; set; }

        public string KeteranganKerja { get; set; }


        public string JamAwalLembur { get; set; }


        public string JamAkhirLembur { get; set; }

        public string TotalJamLembur { get; set; }

        public string KeteranganLembur { get; set; }
        public string PemberiTugas { get; set; }
    }
}
