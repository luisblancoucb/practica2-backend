using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practica2.Api.Models;

public class Cliente
{
    [Key]
    public int Id { get; set; }

    [NotMapped]
    public string Codigo => $"C-{Id:D3}";

    [Required]
    [MaxLength(100)]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Telefono { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Correo { get; set; } = string.Empty;

    [Required]
    [MaxLength(80)]
    public string NombreMascota { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string TipoMascota { get; set; } = string.Empty;

    public ICollection<Cita> Citas { get; set; } = new List<Cita>();
}