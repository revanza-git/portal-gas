using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public class Jabatan
    {
        [Key]
        public int JabatanID { get; set; }

        public string Deskripsi { get; set; }

        public int Department { get; set; }

        public string Atasan { get; set; }

        public bool IsDriver { get; set; }

        public bool IsSecretary { get; set; }

        public bool IsDirector { get; set; }

        public bool Hide { get; set; }
    }
}

