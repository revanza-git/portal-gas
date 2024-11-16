using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public class GasmonGraph2
    {
        public int target_pasokan { get; set; }
        public int realisasi_pasokan { get; set; }
        public int prosentase_pasokan { get; set; }
        public int prosentase_penjualan { get; set; }
        public Decimal target_penjualan { get; set; }
        public Decimal realisasi_penjualan { get; set; }
        public Decimal realisasi_penjualan1 { get; set; }
        public Decimal realisasi_penjualan2 { get; set; }
        public Decimal realisasi_penjualan3 { get; set; }
        public Decimal realisasi_penjualan4 { get; set; }
    }
}
