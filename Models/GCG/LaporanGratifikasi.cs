using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.GCG
{
    public class LaporanGratifikasi
    {
        public string Nama { get; set; }
        public string CoC { get; set; }
        public string CoI { get; set; }
        public string PenerimaanGratifikasi { get; set; }
        public string PemberianGratifikasi { get; set; }
        public string PermintaanGratifikasi { get; set; }
        public string WaktuPelaporan { get; set; }
    }
}
