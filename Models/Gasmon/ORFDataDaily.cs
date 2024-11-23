using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Gasmon
{
    [Table("Gm_ORFDataDaily")]
    public class ORFDataDaily
    {
        public string ORFDataDailyID { get; set; }
        public int LineID { get; set; }
        public DateTime Date { get; set; }
        public decimal Pressure { get; set; }
        public decimal Temperature { get; set; }
        public decimal DailyNet { get; set; }
        public decimal HeatingValue { get; set; }
        public decimal DailyEnergy { get; set; }
        public decimal CO2 { get; set; }
        public decimal Ethane { get; set; }
        public decimal Methane { get; set; }
        public decimal Nitrogen { get; set; }
        public decimal Propane { get; set; }
        public decimal Water { get; set; }
        public decimal iPentane { get; set; }
        public decimal nPentane { get; set; }
        public decimal iButane { get; set; }
        public decimal nButane { get; set; }
        public decimal nHexane { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
