using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    [Table("Gm_Activities")]
    public class GasmonActivity
    {
        [Key]
        public Int32 ActivityID { get; set; }
        public DateTime Date { get; set; }
        public Int32 Source { get; set; }
        public String Time { get; set; }
        public String Remark { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdated { get; set; }
        public String CreatedBy { get; set; }
        public String LastUpdatedBy { get; set; }
    }
}
