using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practica2.Api.Data;
using Practica2.Api.DTOs.Clientes;
using Practica2.Api.Models;

namespace Practica2.Api.Controllers;

[ApiController]
[Route("api/clientes")]
[Authorize(Roles = "Administrador")]
public class ClientesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ClientesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteResponse>>> ObtenerTodos()
    {
        List<Cliente> clientes = await _context.Clientes
            .AsNoTracking()
            .OrderBy(cliente => cliente.Id)
            .ToListAsync();

        List<ClienteResponse> respuesta = clientes
            .Select(CrearRespuesta)
            .ToList();

        return Ok(respuesta);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteResponse>> ObtenerPorId(int id)
    {
        Cliente? cliente = await _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(cliente => cliente.Id == id);

        if (cliente is null)
        {
            return NotFound(new
            {
                mensaje = "Cliente no encontrado."
            });
        }

        return Ok(CrearRespuesta(cliente));
    }

    [HttpPost]
    public async Task<ActionResult<ClienteResponse>> Crear(ClienteRequest request)
    {
        Cliente cliente = new()
        {
            NombreCompleto = request.NombreCompleto.Trim(),
            Telefono = request.Telefono.Trim(),
            Correo = request.Correo.Trim().ToLowerInvariant(),
            NombreMascota = request.NombreMascota.Trim(),
            TipoMascota = request.TipoMascota.Trim()
        };

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        ClienteResponse respuesta = CrearRespuesta(cliente);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = cliente.Id },
            respuesta
        );
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ClienteResponse>> Actualizar(int id, ClienteRequest request)
    {
        Cliente? cliente = await _context.Clientes.FindAsync(id);

        if (cliente is null)
        {
            return NotFound(new
            {
                mensaje = "Cliente no encontrado."
            });
        }

        cliente.NombreCompleto = request.NombreCompleto.Trim();
        cliente.Telefono = request.Telefono.Trim();
        cliente.Correo = request.Correo.Trim().ToLowerInvariant();
        cliente.NombreMascota = request.NombreMascota.Trim();
        cliente.TipoMascota = request.TipoMascota.Trim();

        await _context.SaveChangesAsync();

        return Ok(CrearRespuesta(cliente));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        Cliente? cliente = await _context.Clientes.FindAsync(id);

        if (cliente is null)
        {
            return NotFound(new
            {
                mensaje = "Cliente no encontrado."
            });
        }

        bool tieneCitas = await _context.Citas
            .AnyAsync(cita => cita.ClienteId == id);

        if (tieneCitas)
        {
            return Conflict(new
            {
                mensaje = "No se puede eliminar un cliente que tiene citas."
            });
        }

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static ClienteResponse CrearRespuesta(Cliente cliente)
    {
        return new ClienteResponse
        {
            Id = cliente.Id,
            Codigo = cliente.Codigo,
            NombreCompleto = cliente.NombreCompleto,
            Telefono = cliente.Telefono,
            Correo = cliente.Correo,
            NombreMascota = cliente.NombreMascota,
            TipoMascota = cliente.TipoMascota
        };
    }
}