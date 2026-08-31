using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Practica2.Api.Models;

namespace Practica2.Api.Data;

public static class AdministradorSeeder
{
    public static async Task CrearAdministradorInicialAsync(
        AppDbContext context,
        IPasswordHasher<Usuario> passwordHasher,
        IConfiguration configuration)
    {
        string nombre = configuration["UsuarioInicial:Nombre"]
            ?? throw new InvalidOperationException(
                "No se configuró el nombre del administrador."
            );

        string correo = configuration["UsuarioInicial:Correo"]
            ?? throw new InvalidOperationException(
                "No se configuró el correo del administrador."
            );

        string contrasena =
            configuration["UsuarioInicial:Contrasena"]
            ?? throw new InvalidOperationException(
                "No se configuró la contraseña del administrador."
            );

        correo = correo.Trim().ToLowerInvariant();

        bool administradorExiste = await context.Usuarios
            .AnyAsync(usuario => usuario.Correo == correo);

        if (administradorExiste)
        {
            return;
        }

        Usuario administrador = new()
        {
            Nombre = nombre.Trim(),
            Correo = correo,
            Rol = "Administrador"
        };

        administrador.PasswordHash =
            passwordHasher.HashPassword(
                administrador,
                contrasena
            );

        context.Usuarios.Add(administrador);
        await context.SaveChangesAsync();
    }
}