using Practica2.Api.Models;

namespace Practica2.Api.DTOs.Citas;

public class CitaResponse
{
    public int Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public DateTime FechaHora { get; set; }

    public EstadoCita Estado { get; set; }

    public int ClienteId { get; set; }

    public string ClienteNombre { get; set; } = string.Empty;

    public string NombreMascota { get; set; } = string.Empty;

    public string TipoMascota { get; set; } = string.Empty;

    public int ServicioId { get; set; }

    public string ServicioNombre { get; set; } = string.Empty;
}