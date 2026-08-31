using System.ComponentModel.DataAnnotations;

namespace Practica2.Api.DTOs.Autenticacion;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Correo { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Contrasena { get; set; } = string.Empty;
}