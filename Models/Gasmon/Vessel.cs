using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Gasmon
{
    [Table("Gm_Vessel")]
    public class Vessel
    {
        public int VesselID { get; set; }
        public string Name { get; set; }
    }
}
