using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practica2.Api.Models;

public enum EstadoCita
{
    Pendiente,
    Confirmada,
    Completada,
    Cancelada
}

public class Cita
{
    [Key]
    public int Id { get; set; }

    [NotMapped]
    public string Codigo => $"CT-{Id:D3}";

    public DateTime FechaHora { get; set; }

    public EstadoCita Estado { get; set; } = EstadoCita.Pendiente;

    [Range(1, int.MaxValue)]
    public int ClienteId { get; set; }

    public Cliente? Cliente { get; set; }

    [Range(1, int.MaxValue)]
    public int ServicioId { get; set; }

    public Servicio? Servicio { get; set; }
}