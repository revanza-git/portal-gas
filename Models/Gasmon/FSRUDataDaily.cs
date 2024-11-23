using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Gasmon
{
    [Table("Gm_FSRUDataDaily")]
    public class FSRUDataDaily
    {
        public string FSRUDataDailyID { get; set; }
        public int FSRUID { get; set; }
        public DateTime Date { get; set; }
        public decimal Rate { get; set; }
        public decimal Pressure { get; set; }
        public decimal Temperature { get; set; }
        public decimal LNGTankInventory { get; set; }
        public decimal BoFM3 { get; set; }
        public decimal BoFKg { get; set; }
        public decimal DeliveredToORFM3 { get; set; }
        public decimal DeliveredToORFKg { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
