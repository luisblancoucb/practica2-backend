using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Practica2.Api.DTOs.Autenticacion;
using Practica2.Api.Models;

namespace Practica2.Api.Services;

public class JwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public LoginResponse GenerarToken(Usuario usuario)
    {
        string clave = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("No se encontró la clave JWT.");

        string issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("No se encontró el emisor JWT.");

        string audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("No se encontró la audiencia JWT.");

        int duracionMinutos =
            _configuration.GetValue<int>("Jwt:DurationMinutes");

        DateTime expiracion =
            DateTime.UtcNow.AddMinutes(duracionMinutos);

        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Email, usuario.Correo),
            new Claim(ClaimTypes.Role, usuario.Rol)
        ];

        SymmetricSecurityKey llaveSeguridad =
            new(Encoding.UTF8.GetBytes(clave));

        SigningCredentials credenciales =
            new(llaveSeguridad, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiracion,
            signingCredentials: credenciales
        );

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiracion = expiracion,
            Nombre = usuario.Nombre,
            Correo = usuario.Correo,
            Rol = usuario.Rol
        };
    }
}