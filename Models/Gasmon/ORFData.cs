using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Gasmon
{
    [Table("Gm_ORFData")]
    public class ORFData
    {
        public string ORFDataID { get; set; }
        public int LineID { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public decimal VolumeA { get; set; }
        public decimal VolumeB { get; set; }
        public decimal VolumeC { get; set; }
        public decimal Volume { get; set; }
        public decimal FlowA { get; set; }
        public decimal FlowB { get; set; }
        public decimal FlowC { get; set; }
        public decimal Flow { get; set; }
        public decimal GHV { get; set; }
        public decimal Temperature { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
