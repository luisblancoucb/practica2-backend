using System.ComponentModel.DataAnnotations;
using Practica2.Api.Models;

namespace Practica2.Api.DTOs.Citas;

public class CitaRequest
{
    public DateTime FechaHora { get; set; }

    [EnumDataType(typeof(EstadoCita))]
    public EstadoCita Estado { get; set; } = EstadoCita.Pendiente;

    [Range(1, int.MaxValue)]
    public int ClienteId { get; set; }

    [Range(1, int.MaxValue)]
    public int ServicioId { get; set; }
}