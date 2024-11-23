using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Common
{
    public class JenisPekerjaan
    {
        [Key]
        public int jenis_pekerjaanID { get; set; }

        [Required]
        public string Deskripsi { get; set; }
    }
}
