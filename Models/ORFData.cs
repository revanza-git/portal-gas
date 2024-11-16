using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    [Table("Gm_ORFData")]
    public class ORFData
    {
        public String ORFDataID { get; set; }
        public Int32 LineID { get; set; }
        public DateTime Date {get;set;}
        public String Time { get; set; }
        public Decimal VolumeA { get; set; }
        public Decimal VolumeB { get; set; }
        public Decimal VolumeC { get; set; }
        public Decimal Volume { get; set; }
        public Decimal FlowA { get; set; }
        public Decimal FlowB { get; set; }
        public Decimal FlowC { get; set; }
        public Decimal Flow { get; set; }
        public Decimal GHV { get; set; }
        public Decimal Temperature { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdated { get; set; }
        public String CreatedBy { get; set; }
        public String LastUpdatedBy { get; set; }
    }
}
