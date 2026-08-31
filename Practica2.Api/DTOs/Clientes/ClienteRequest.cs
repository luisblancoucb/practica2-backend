using System.ComponentModel.DataAnnotations;

namespace Practica2.Api.DTOs.Clientes;

public class ClienteRequest
{
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
}