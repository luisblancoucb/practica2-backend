using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practica2.Api.Data;
using Practica2.Api.DTOs.Servicios;
using Practica2.Api.Models;

namespace Practica2.Api.Controllers;

[ApiController]
[Route("api/servicios")]
[Authorize(Roles = "Administrador")]
public class ServiciosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ServiciosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServicioResponse>>> ObtenerTodos()
    {
        List<Servicio> servicios = await _context.Servicios
            .AsNoTracking()
            .OrderBy(servicio => servicio.Id)
            .ToListAsync();

        List<ServicioResponse> respuesta = servicios
            .Select(CrearRespuesta)
            .ToList();

        return Ok(respuesta);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ServicioResponse>> ObtenerPorId(int id)
    {
        Servicio? servicio = await _context.Servicios
            .AsNoTracking()
            .FirstOrDefaultAsync(servicio => servicio.Id == id);

        if (servicio is null)
        {
            return NotFound(new
            {
                mensaje = "Servicio no encontrado."
            });
        }

        return Ok(CrearRespuesta(servicio));
    }

    [HttpPost]
    public async Task<ActionResult<ServicioResponse>> Crear(ServicioRequest request)
    {
        Servicio servicio = new()
        {
            Nombre = request.Nombre.Trim(),
            Descripcion = request.Descripcion.Trim(),
            Precio = request.Precio,
            DuracionMinutos = request.DuracionMinutos,
            Activo = request.Activo
        };

        _context.Servicios.Add(servicio);
        await _context.SaveChangesAsync();

        ServicioResponse respuesta = CrearRespuesta(servicio);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = servicio.Id },
            respuesta
        );
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ServicioResponse>> Actualizar(int id, ServicioRequest request)
    {
        Servicio? servicio = await _context.Servicios.FindAsync(id);

        if (servicio is null)
        {
            return NotFound(new
            {
                mensaje = "Servicio no encontrado."
            });
        }

        servicio.Nombre = request.Nombre.Trim();
        servicio.Descripcion = request.Descripcion.Trim();
        servicio.Precio = request.Precio;
        servicio.DuracionMinutos = request.DuracionMinutos;
        servicio.Activo = request.Activo;

        await _context.SaveChangesAsync();

        return Ok(CrearRespuesta(servicio));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        Servicio? servicio = await _context.Servicios.FindAsync(id);

        if (servicio is null)
        {
            return NotFound(new
            {
                mensaje = "Servicio no encontrado."
            });
        }

        bool tieneCitas = await _context.Citas
            .AnyAsync(cita => cita.ServicioId == id);

        if (tieneCitas)
        {
            return Conflict(new
            {
                mensaje = "No se puede eliminar un servicio que tiene citas."
            });
        }

        _context.Servicios.Remove(servicio);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static ServicioResponse CrearRespuesta(Servicio servicio)
    {
        return new ServicioResponse
        {
            Id = servicio.Id,
            Nombre = servicio.Nombre,
            Descripcion = servicio.Descripcion,
            Precio = servicio.Precio,
            DuracionMinutos = servicio.DuracionMinutos,
            Activo = servicio.Activo
        };
    }
}