
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Text;



namespace Admin.Models.DCU
{

    public class DCU
    {
        [Key]
        public string DCUID { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Nama { get; set; }
        [Required]
        public int JenisPekerjaan { get; set; }
        [Required]
        public string Sistole { get; set; }
        [Required]
        public string Diastole { get; set; }
        [Required]
        public string Nadi { get; set; }
        [Required]
        public string Suhu { get; set; }
        [Required]
        public string Keluhan { get; set; }
        [Required]
        public string Foto { get; set; }
        [Required]
        public string ContentType { get; set; }
        public string Other { get; set; }

        public string NamaPerusahaan { get; set; }

        public string DeskripsiPekerjaan { get; set; }

    }
}
