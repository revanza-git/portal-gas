#nullable enable

using System.ComponentModel.DataAnnotations;
using System;

public class DCU
{
    [Key]
    public required string DCUID { get; set; }
    [Required]
    public DateTime Date { get; set; }
    [Required]
    public required string Nama { get; set; }
    [Required]
    public int JenisPekerjaan { get; set; }
    [Required]
    public required string Sistole { get; set; }
    [Required]
    public required string Diastole { get; set; }
    [Required]
    public required string Nadi { get; set; }
    [Required]
    public required string Suhu { get; set; }
    [Required]
    public required string Keluhan { get; set; }
    public string? Foto { get; set; } // Nullable
    public string? ContentType { get; set; } // Nullable
    public string? Other { get; set; } // Nullable
    public string? NamaPerusahaan { get; set; } // Nullable
    public string? DeskripsiPekerjaan { get; set; } // Nullable
}

#nullable disable
