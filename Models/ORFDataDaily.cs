using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    [Table("Gm_ORFDataDaily")]
    public class ORFDataDaily
    {
        public String ORFDataDailyID { get; set; }
        public Int32 LineID { get; set; }
        public DateTime Date { get; set; }
        public Decimal Pressure { get; set; }
        public Decimal Temperature { get; set; }
        public Decimal DailyNet { get; set; }
        public Decimal HeatingValue { get; set; }
        public Decimal DailyEnergy { get; set; }
        public Decimal CO2 { get; set; }
        public Decimal Ethane { get; set; }
        public Decimal Methane { get; set; }
        public Decimal Nitrogen { get; set; }
        public Decimal Propane { get; set; }
        public Decimal Water { get; set; }
        public Decimal iPentane { get; set; }
        public Decimal nPentane { get; set; }
        public Decimal iButane { get; set; }
        public Decimal nButane { get; set; }
        public Decimal nHexane { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdated { get; set; }
        public String CreatedBy { get; set; }
        public String LastUpdatedBy { get; set; }
    }
}
