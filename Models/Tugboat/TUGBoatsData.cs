using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Tugboat
{
    [Table("Gm_TUGBoatsData")]
    public class TUGBoatsData
    {
        [Key]
        public string TUGBoatsDataID { get; set; }
        public int BoatID { get; set; }
        public DateTime Date { get; set; }
        public decimal FuelOilConsumption { get; set; }
        public decimal FuelOilROB { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
