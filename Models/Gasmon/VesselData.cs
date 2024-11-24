using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Gasmon
{
    [Table("Gm_VesselData")]
    public class VesselData
    {
        public string VesselDataID { get; set; }
        public int VesselID { get; set; }
        public int CargoID { get; set; }
        public DateTime Date { get; set; }
        public string Position { get; set; }
        public string NextPort { get; set; }
        public string ETANextPort { get; set; }
        public string WindSpeedDirection { get; set; }
        public decimal CargoQuantityOnBoard { get; set; }
        public decimal BoilOff { get; set; }
        public decimal BunkerROBHFO { get; set; }
        public decimal BunkerROBMDO { get; set; }
        public decimal BunkerROBMGO { get; set; }
        public decimal ConsumpHFO { get; set; }
        public decimal ConsumpMDO { get; set; }
        public decimal ConsumpMGO { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
