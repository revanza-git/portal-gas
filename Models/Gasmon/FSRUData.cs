using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Gasmon
{
    [Table("Gm_FSRUData")]
    public class FSRUData
    {
        public string FSRUDataID { get; set; }
        public int FSRUID { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public decimal Pressure { get; set; }
        public decimal Temperature { get; set; }
        public decimal Flow1 { get; set; }
        public decimal Flow2 { get; set; }
        public decimal RobLNG { get; set; }
        public decimal MMSCF { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
