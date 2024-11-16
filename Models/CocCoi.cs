using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    [Table("GCG_CocCoi")]
    public class CocCoi
    {
        public int Year { get; set; }
        public String UserID { get; set; }
        public Boolean CoI { get; set; }
        public Boolean CoC { get; set; }
        public DateTime? CoISignedTime { get; set; }
        public DateTime? CoCSignedTime { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
