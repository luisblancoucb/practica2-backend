namespace Practica2.Api.DTOs.Autenticacion;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;

    public DateTime Expiracion { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public string Rol { get; set; } = string.Empty;
}