
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Text;



namespace Admin.Models
{

    public class DCU
    {
        [Key]
        public String DCUID { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public String Nama { get; set; }
        [Required]
        public int JenisPekerjaan { get; set; }
        [Required] 
        public String Sistole { get; set; }
        [Required]
        public String Diastole { get; set; }
        [Required]
        public String Nadi { get; set; }
        [Required]
        public String Suhu { get; set; }
        [Required]
        public String Keluhan { get; set; }
        [Required]
        public String Foto { get; set; }
        [Required]
        public String ContentType { get; set; }
        public String Other { get; set; }

        public string NamaPerusahaan { get; set; }

        public string DeskripsiPekerjaan { get; set; }

    }
}
