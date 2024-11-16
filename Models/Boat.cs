using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    [Table("Gm_Boats")]
    public class Boat
    {
        [Key]
        public Int32 BoatID { get; set; }
        public String Name { get; set; }
    }
}
