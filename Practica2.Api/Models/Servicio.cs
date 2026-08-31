using System.ComponentModel.DataAnnotations;

namespace Practica2.Api.Models;

public class Servicio
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(300)]
    public string Descripcion { get; set; } = string.Empty;

    [Range(
    typeof(decimal),
    "0.01",
    "999999.99",
    ParseLimitsInInvariantCulture = true,
    ConvertValueInInvariantCulture = true
    )]
    public decimal Precio { get; set; }

    [Range(1, 600)]
    public int DuracionMinutos { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<Cita> Citas { get; set; } = new List<Cita>();
}