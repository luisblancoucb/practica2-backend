namespace Practica2.Api.DTOs.Clientes;

public class ClienteResponse
{
    public int Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public string NombreMascota { get; set; } = string.Empty;

    public string TipoMascota { get; set; } = string.Empty;
}