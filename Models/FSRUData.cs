using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    [Table("Gm_FSRUData")]
    public class FSRUData
    {
        public String FSRUDataID { get; set; }
        public Int32 FSRUID { get; set; }
        public DateTime Date { get; set; }
        public String Time { get; set; }
        public Decimal Pressure { get; set; }
        public Decimal Temperature { get; set; }
        public Decimal Flow1 { get; set; }
        public Decimal Flow2 { get; set; }
        public Decimal RobLNG { get; set; }
        public Decimal MMSCF { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdated { get; set; }
        public String CreatedBy { get; set; }
        public String LastUpdatedBy { get; set; }
    }
}
