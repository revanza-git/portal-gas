using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Aman
{
    public class AmanCorrectionType
    {
        public int AmanCorrectionTypeID { get; set; }
        [Required]
        public string Deskripsi { get; set; }
    }
}
