using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    [Table("Gm_VesselData")]
    public class VesselData
    {
        public string VesselDataID { get; set; }
        public Int32 VesselID { get; set; }
        public Int32 CargoID { get; set; }
        public DateTime Date { get; set; }
        public string Position { get; set; }
        public string NextPort { get; set; }
        public string ETANextPort { get; set; }
        public string WindSpeedDirection { get; set; }
        public Decimal CargoQuantityOnBoard { get; set; }
        public Decimal BoilOff { get; set; }
        public Decimal BunkerROBHFO { get; set; }
        public Decimal BunkerROBMDO { get; set; }
        public Decimal BunkerROBMGO { get; set; }
        public Decimal ConsumpHFO { get; set; }
        public Decimal ConsumpMDO { get; set; }
        public Decimal ConsumpMGO { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBt { get; set; }
    }
}
