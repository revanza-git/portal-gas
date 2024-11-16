using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    [Table("Gm_TUGBoatsData")]
    public class TUGBoatsData
    {
        [Key]
        public String TUGBoatsDataID { get; set; }
        public Int32 BoatID { get; set; }
        public DateTime Date { get; set; }
        public Decimal FuelOilConsumption { get; set; }
        public Decimal FuelOilROB { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdated { get; set; }
        public String CreatedBy { get; set; }
        public String LastUpdatedBy { get; set; }
    }
}
