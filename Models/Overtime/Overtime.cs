using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Overtime
{
    public class Overtime
    {
        public int OvertimeID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Posisi { get; set; }
        public int Department { get; set; }
        [Required]
        public TimeSpan JamMulaiKerja { get; set; }
        [Required]
        public TimeSpan JamSelesaiKerja { get; set; }
        [Required]
        public DateTime Tanggal { get; set; }
        [Required]
        public TimeSpan JamAwalLembur { get; set; }
        [Required]
        public TimeSpan JamAkhirLembur { get; set; }
        [Required]
        public string WorkDescription { get; set; }
        public string Assigner { get; set; }
        public double TotalHours { get; set; }
        [Required]
        public int Status { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime? CreationDateTime { get; set; } = DateTime.Now;
    }
}
