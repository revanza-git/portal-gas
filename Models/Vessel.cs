using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    [Table("Gm_Vessel")]
    public class Vessel
    {
        public Int32 VesselID { get; set; }
        public String Name { get; set; }
    }
}
