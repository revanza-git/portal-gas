using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Tugboat
{
    [Table("Gm_Boats")]
    public class Boat
    {
        [Key]
        public int BoatID { get; set; }
        public string Name { get; set; }
    }
}
