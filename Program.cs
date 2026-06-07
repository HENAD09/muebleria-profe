using Microsoft.EntityFrameworkCore;
using MuebleriaProfe.Data;
using Microsoft.AspNetCore.HttpOverrides;
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Configuraci�n de PostgreSQL
// Prioridad de conexi�n:
//   1) DATABASE_CONNECTION  -> formato Npgsql manual (Host=...;Username=...)
//   2) DATABASE_URL         -> formato URL que Railway genera autom�ticamente
//                              (postgresql://user:pass@host:port/db). Basta con
//                              referenciar esa variable del Postgres en Railway.
//   3) Fallback a Neon       -> mientras no exista ninguna de las anteriores.
// As� se alterna de base sin tocar c�digo (rollback instant�neo).
// TODO: una vez confirmada la migraci�n y dado de baja Neon, eliminar el fallback.
static string? BuildConnFromUrl(string? url)
{
    if (string.IsNullOrWhiteSpace(url)) return null;
    try
    {
        var uri = new Uri(url);
        var parts = uri.UserInfo.Split(':');
        var user = Uri.UnescapeDataString(parts[0]);
        var pass = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : "";
        var db = uri.AbsolutePath.TrimStart('/');
        var port = uri.Port > 0 ? uri.Port : 5432;
        // SSL Mode=Prefer cubre tanto el host interno de Railway (sin SSL) como
        // el p�blico (con SSL). Trust Server Certificate evita validar el cert.
        return $"Host={uri.Host};Port={port};Database={db};Username={user};Password={pass};SSL Mode=Prefer;Trust Server Certificate=True";
    }
    catch
    {
        return null;
    }
}

var connectionString =
    Environment.GetEnvironmentVariable("DATABASE_CONNECTION")
    ?? BuildConnFromUrl(Environment.GetEnvironmentVariable("DATABASE_URL"))
    ?? "Host=ep-purple-dream-apgzgg0p.c-7.us-east-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_yH6LrDbAZ0cJ;SSL Mode=Require;Trust Server Certificate=True";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// En Program.cs
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});


var app = builder.Build();

// Auto-migrar base de datos al iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.Migrate();
        Console.WriteLine("Migraciones aplicadas correctamente.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al migrar BD: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
app.UseDeveloperExceptionPage();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseForwardedHeaders();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
