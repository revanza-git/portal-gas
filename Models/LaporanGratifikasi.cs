using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public class LaporanGratifikasi
    {
        public String Nama { get; set; }
        public String CoC { get; set; }
        public String CoI { get; set; }
        public String PenerimaanGratifikasi { get; set; }
        public String PemberianGratifikasi { get; set; }
        public String PermintaanGratifikasi { get; set; }
        public String WaktuPelaporan { get; set; }
    }
}
