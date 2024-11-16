using System;
using System.ComponentModel.DataAnnotations;

namespace Admin.Models
{
    public class Semar
    {
        public string SemarID { get; set;}
        [Required]
        public int Type {get; set;}
        public string NoDocument { get; set;}
        [Required]
        public string Title { get; set; }
        [Required]
        public int SemarLevel { get; set; }
        public string Owner { get; set; }
        [Required]
        public DateTime PublishDate { get; set; }
        [Required]
        public DateTime ExpiredDate { get; set; }
        [Required]
        public string Description { get; set; }
        public string Revision { get; set; }
        
        public string FileName { get; set; }
        public string ContentType { get; set; }
        [Required]
        public int Classification { get; set; }
        public string Creator { get; set; }
        public int Status { get; set; }
        public int ExpiredNotification { get; set; }
    }
}