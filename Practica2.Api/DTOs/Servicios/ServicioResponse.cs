namespace Practica2.Api.DTOs.Servicios;

public class ServicioResponse
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public decimal Precio { get; set; }

    public int DuracionMinutos { get; set; }

    public bool Activo { get; set; }
}