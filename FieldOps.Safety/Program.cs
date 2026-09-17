using System.Text.Json.Serialization;
using FieldOps.Safety.Application;
using FieldOps.Safety.Domain;
using FieldOps.Safety.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var databaseProvider = builder.Configuration["DatabaseProvider"]?.Trim().ToLowerInvariant() ?? "sqlite";
var connectionString = builder.Configuration.GetConnectionString("Default");

switch (databaseProvider)
{
    case "sqlite":
        builder.Services.AddDbContext<IncidentDbContext>(options =>
            options.UseSqlite(connectionString ?? "Data Source=fieldops.db"));
        break;

    case "postgres":
    case "postgresql":
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Configura ConnectionStrings:Default para usar PostgreSQL o Supabase.");
        }

        builder.Services.AddDbContext<IncidentDbContext>(options =>
            options.UseNpgsql(connectionString));
        break;

    default:
        throw new InvalidOperationException(
            $"Proveedor de base de datos no reconocido: {databaseProvider}.");
}

builder.Services.AddScoped<IIncidentStore, EfIncidentStore>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IncidentDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/", () => Results.Ok(new
{
    service = "fieldops-safety-api",
    status = "running",
    message = "API activa. Consulta /health o /api/incidents."
}));

app.MapGet("/health", () => Results.Ok(new { service = "fieldops-safety-api", status = "ok" }));

app.MapGet("/api/incidents", (IIncidentStore store, IncidentStatus? status) =>
    Results.Ok(store.GetAll(status)));

app.MapGet("/api/incidents/{id:guid}", (Guid id, IIncidentStore store) =>
{
    var incident = store.Get(id);
    return incident is null ? Results.NotFound() : Results.Ok(incident);
});

app.MapPost("/api/incidents", (CreateIncidentRequest request, IIncidentStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description) || string.IsNullOrWhiteSpace(request.Area))
    {
        return Results.BadRequest(new { error = "Title, description and area are required." });
    }

    var incident = store.Add(request.Title.Trim(), request.Description.Trim(), request.Area.Trim(), request.Severity);
    return Results.Created($"/api/incidents/{incident.Id}", incident);
});

app.MapPatch("/api/incidents/{id:guid}/status", (Guid id, ChangeIncidentStatusRequest request, IIncidentStore store) =>
{
    var incident = store.ChangeStatus(id, request.Status);
    return incident is null ? Results.NotFound() : Results.Ok(incident);
});

app.Run();

public partial class Program;
