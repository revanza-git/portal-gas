using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Gasmon
{
    [Table("Gm_Parameters")]
    public class GasmonParameter
    {
        public string ParameterID { get; set; }
        public int Tahun { get; set; }
        public string Label { get; set; }
        public string Satuan { get; set; }
        public int Value { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
