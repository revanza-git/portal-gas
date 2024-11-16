using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    [Table("Gm_Parameters")]
    public class GasmonParameter
    {
        public String ParameterID { get; set; }
        public Int32 Tahun { get; set; }
        public String Label { get; set; }
        public String Satuan { get; set; }
        public Int32 Value { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdated { get; set; }
        public String CreatedBy { get; set; }
        public String LastUpdatedBy { get; set; }
    }
}
