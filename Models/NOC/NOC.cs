using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Admin.Models.NOC
{
    public class NOC
    {
        [Key]
        public string NOCID { get; set; }

        public string Photo { get; set; }

        public string ContentType { get; set; }

        public int Lokasi { get; set; }

        public int DaftarPengamatan { get; set; }

        public string Deskripsi { get; set; }

        public string Tindakan { get; set; }

        public string Rekomendasi { get; set; }

        public int Prioritas { get; set; }

        public int Status { get; set; }

        public DateTime EntryDate { get; set; }

        public DateTime DueDate { get; set; }

        public string NamaObserver { get; set; }

        public string DivisiObserver { get; set; }

        public int? Clsr { get; set; }
    }
}
