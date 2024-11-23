using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Gasmon
{
    [Table("Gm_Activities")]
    public class GasmonActivity
    {
        [Key]
        public int ActivityID { get; set; }
        public DateTime Date { get; set; }
        public int Source { get; set; }
        public string Time { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
