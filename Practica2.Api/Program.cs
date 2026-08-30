using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Practica2.Api.Data;
using Practica2.Api.Services;
using Microsoft.AspNetCore.Identity;
using Practica2.Api.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddScoped<JwtService>();

// Agrega el servicio de hash de passwords
builder.Services.AddScoped<
    IPasswordHasher<Usuario>,
    PasswordHasher<Usuario>
>();

string claveJwt = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("No se encontró la clave JWT.");

string issuerJwt = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("No se encontró el emisor JWT.");

string audienceJwt = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("No se encontró la audiencia JWT.");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuerJwt,

            ValidateAudience = true,
            ValidAudience = audienceJwt,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(claveJwt)
            ),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });
builder.Services.AddOpenApi();

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    AppDbContext context =
        scope.ServiceProvider.GetRequiredService<AppDbContext>();

    IPasswordHasher<Usuario> passwordHasher =
        scope.ServiceProvider
            .GetRequiredService<IPasswordHasher<Usuario>>();

    await AdministradorSeeder.CrearAdministradorInicialAsync(
        context,
        passwordHasher,
        app.Configuration
    );
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();