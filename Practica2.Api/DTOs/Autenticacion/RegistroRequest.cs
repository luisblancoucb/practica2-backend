using System.ComponentModel.DataAnnotations;

namespace Practica2.Api.DTOs.Autenticacion;

public class RegistroRequest
{
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Correo { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Contrasena { get; set; } = string.Empty;
}