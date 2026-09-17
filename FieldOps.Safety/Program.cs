using System.Text.Json.Serialization;
using FieldOps.Safety.Application;
using FieldOps.Safety.Domain;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddSingleton<IIncidentStore, InMemoryIncidentStore>();

var app = builder.Build();

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
