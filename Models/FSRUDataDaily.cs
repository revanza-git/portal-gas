using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    [Table("Gm_FSRUDataDaily")]
    public class FSRUDataDaily
    {
        public String FSRUDataDailyID { get; set; }
        public Int32 FSRUID { get; set; }
        public DateTime Date { get; set; }
        public Decimal Rate {get; set; }
        public Decimal Pressure { get; set; }
        public Decimal Temperature { get; set; }
        public Decimal LNGTankInventory { get; set; }
        public Decimal BoFM3 { get; set; }
        public Decimal BoFKg { get; set; }
        public Decimal DeliveredToORFM3 { get; set; }
        public Decimal DeliveredToORFKg { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdated { get; set; }
        public String CreatedBy { get; set; }
        public String LastUpdatedBy { get; set; }
    }
}
