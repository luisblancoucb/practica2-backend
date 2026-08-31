using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practica2.Api.Data;
using Practica2.Api.DTOs.Citas;
using Practica2.Api.Models;

namespace Practica2.Api.Controllers;

[ApiController]
[Route("api/citas")]
[Authorize(Roles = "Administrador,Empleado")]
public class CitasController : ControllerBase
{
    private readonly AppDbContext _context;

    public CitasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CitaResponse>>> ObtenerTodos()
    {
        List<Cita> citas = await _context.Citas
            .AsNoTracking()
            .Include(cita => cita.Cliente)
            .Include(cita => cita.Servicio)
            .OrderBy(cita => cita.FechaHora)
            .ToListAsync();

        List<CitaResponse> respuesta = citas
            .Select(CrearRespuesta)
            .ToList();

        return Ok(respuesta);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CitaResponse>> ObtenerPorId(int id)
    {
        Cita? cita = await _context.Citas
            .AsNoTracking()
            .Include(cita => cita.Cliente)
            .Include(cita => cita.Servicio)
            .FirstOrDefaultAsync(cita => cita.Id == id);

        if (cita is null)
        {
            return NotFound(new
            {
                mensaje = "Cita no encontrada."
            });
        }

        return Ok(CrearRespuesta(cita));
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<CitaResponse>> Crear(CitaRequest request)
    {
        if (request.FechaHora == default)
        {
            return BadRequest(new
            {
                mensaje = "La fecha y hora son obligatorias."
            });
        }

        Cliente? cliente = await _context.Clientes
            .FindAsync(request.ClienteId);

        if (cliente is null)
        {
            return BadRequest(new
            {
                mensaje = "El cliente indicado no existe."
            });
        }

        Servicio? servicio = await _context.Servicios
            .FindAsync(request.ServicioId);

        if (servicio is null)
        {
            return BadRequest(new
            {
                mensaje = "El servicio indicado no existe."
            });
        }

        if (!servicio.Activo)
        {
            return BadRequest(new
            {
                mensaje = "El servicio indicado no está activo."
            });
        }

        Cita cita = new()
        {
            FechaHora = request.FechaHora,
            Estado = request.Estado,
            ClienteId = cliente.Id,
            Cliente = cliente,
            ServicioId = servicio.Id,
            Servicio = servicio
        };

        _context.Citas.Add(cita);
        await _context.SaveChangesAsync();

        CitaResponse respuesta = CrearRespuesta(cita);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = cita.Id },
            respuesta
        );
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<CitaResponse>> Actualizar(int id, CitaRequest request)
    {
        Cita? cita = await _context.Citas
            .Include(cita => cita.Cliente)
            .Include(cita => cita.Servicio)
            .FirstOrDefaultAsync(cita => cita.Id == id);

        if (cita is null)
        {
            return NotFound(new
            {
                mensaje = "Cita no encontrada."
            });
        }

        if (request.FechaHora == default)
        {
            return BadRequest(new
            {
                mensaje = "La fecha y hora son obligatorias."
            });
        }

        Cliente? cliente = await _context.Clientes
            .FindAsync(request.ClienteId);

        if (cliente is null)
        {
            return BadRequest(new
            {
                mensaje = "El cliente indicado no existe."
            });
        }

        Servicio? servicio = await _context.Servicios
            .FindAsync(request.ServicioId);

        if (servicio is null)
        {
            return BadRequest(new
            {
                mensaje = "El servicio indicado no existe."
            });
        }

        if (!servicio.Activo)
        {
            return BadRequest(new
            {
                mensaje = "El servicio indicado no está activo."
            });
        }

        cita.FechaHora = request.FechaHora;
        cita.Estado = request.Estado;
        cita.ClienteId = cliente.Id;
        cita.Cliente = cliente;
        cita.ServicioId = servicio.Id;
        cita.Servicio = servicio;

        await _context.SaveChangesAsync();

        return Ok(CrearRespuesta(cita));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        Cita? cita = await _context.Citas.FindAsync(id);

        if (cita is null)
        {
            return NotFound(new
            {
                mensaje = "Cita no encontrada."
            });
        }

        _context.Citas.Remove(cita);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static CitaResponse CrearRespuesta(Cita cita)
    {
        return new CitaResponse
        {
            Id = cita.Id,
            Codigo = cita.Codigo,
            FechaHora = cita.FechaHora,
            Estado = cita.Estado,
            ClienteId = cita.ClienteId,
            ClienteNombre = cita.Cliente?.NombreCompleto ?? string.Empty,
            NombreMascota = cita.Cliente?.NombreMascota ?? string.Empty,
            TipoMascota = cita.Cliente?.TipoMascota ?? string.Empty,
            ServicioId = cita.ServicioId,
            ServicioNombre = cita.Servicio?.Nombre ?? string.Empty
        };
    }
}