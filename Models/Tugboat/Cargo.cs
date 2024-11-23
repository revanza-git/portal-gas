using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Tugboat
{
    [Table("Gm_Cargo")]
    public class Cargo
    {
        public int CargoID { get; set; }
        public int Tahun { get; set; }
        public int IsTarget { get; set; }
        public string Code { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
