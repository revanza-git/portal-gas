using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.GCG
{
    [Table("GCG_CocCoi")]
    public class CocCoi
    {
        public int Year { get; set; }
        public string UserID { get; set; }
        public bool CoI { get; set; }
        public bool CoC { get; set; }
        public DateTime? CoISignedTime { get; set; }
        public DateTime? CoCSignedTime { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
