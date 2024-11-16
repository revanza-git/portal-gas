using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    [Table("Gm_Boat")]
    public class Gm_Boat
    {
        public string BoatID { get; set; }
        public string Name { get; set; }
    }
}
