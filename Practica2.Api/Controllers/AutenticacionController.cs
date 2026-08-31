using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practica2.Api.Data;
using Practica2.Api.DTOs.Autenticacion;
using Practica2.Api.Models;
using Practica2.Api.Services;
using System.Security.Claims;

namespace Practica2.Api.Controllers;

[ApiController]
[Route("api/autenticacion")]
public class AutenticacionController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public AutenticacionController(
        AppDbContext context,
        JwtService jwtService,
        IPasswordHasher<Usuario> passwordHasher)
    {
        _context = context;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost("registro")]
    public async Task<ActionResult> Registrar(RegistroRequest request)
    {
        string correoNormalizado =
            request.Correo.Trim().ToLowerInvariant();

        bool correoRegistrado = await _context.Usuarios
            .AnyAsync(usuario => usuario.Correo == correoNormalizado);

        if (correoRegistrado)
        {
            return Conflict(new
            {
                mensaje = "El correo ya está registrado."
            });
        }

        Usuario usuario = new()
        {
            Nombre = request.Nombre.Trim(),
            Correo = correoNormalizado,
            Rol = "Empleado"
        };

        usuario.PasswordHash = _passwordHasher.HashPassword(
            usuario,
            request.Contrasena
        );

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, new
        {
            usuario.Id,
            usuario.Nombre,
            usuario.Correo,
            usuario.Rol
        });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request)
    {
        string correoNormalizado =
            request.Correo.Trim().ToLowerInvariant();

        Usuario? usuario = await _context.Usuarios
            .SingleOrDefaultAsync(
                usuario => usuario.Correo == correoNormalizado
            );

        if (usuario is null)
        {
            return Unauthorized(new
            {
                mensaje = "Correo o contraseña incorrectos."
            });
        }

        PasswordVerificationResult resultado =
            _passwordHasher.VerifyHashedPassword(
                usuario,
                usuario.PasswordHash,
                request.Contrasena
            );

        if (resultado == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new
            {
                mensaje = "Correo o contraseña incorrectos."
            });
        }

        LoginResponse respuesta =
            _jwtService.GenerarToken(usuario);

        return Ok(respuesta);
    }
    
    [Authorize]
    [HttpGet("perfil")]
    public ActionResult ObtenerPerfil()
    {
        return Ok(new
        {
            id = User.FindFirstValue(ClaimTypes.NameIdentifier),
            nombre = User.FindFirstValue(ClaimTypes.Name),
            correo = User.FindFirstValue(ClaimTypes.Email),
            rol = User.FindFirstValue(ClaimTypes.Role)
        });
    }

}