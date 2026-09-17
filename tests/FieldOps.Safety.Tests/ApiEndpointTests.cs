using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FieldOps.Safety.Application;
using FieldOps.Safety.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace FieldOps.Safety.Tests;

public sealed class ApiEndpointTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient client = factory.CreateClient();

    [Fact]
    public async Task Post_incident_returns_created_incident()
    {
        var response = await client.PostAsJsonAsync("/api/incidents", new
        {
            title = "Cable suelto",
            description = "Se encontró un cable en una zona de paso",
            area = "Planta",
            severity = IncidentSeverity.High
        });

        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        jsonOptions.Converters.Add(new JsonStringEnumConverter());
        var incident = await response.Content.ReadFromJsonAsync<Incident>(jsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(incident);
        Assert.Equal("Cable suelto", incident.Title);
        Assert.Equal(IncidentStatus.Open, incident.Status);
    }

    [Fact]
    public async Task Post_incident_without_required_fields_returns_bad_request()
    {
        var response = await client.PostAsJsonAsync("/api/incidents", new
        {
            title = "",
            description = "",
            area = ""
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Health_endpoint_returns_ok()
    {
        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IIncidentStore>();
            services.AddSingleton<IIncidentStore, InMemoryIncidentStore>();
        });
    }
}
