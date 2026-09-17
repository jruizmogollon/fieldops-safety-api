using FieldOps.Safety.Application;
using FieldOps.Safety.Domain;
using Xunit;

namespace FieldOps.Safety.Tests;

public sealed class IncidentStoreTests
{
    [Fact]
    public void New_incident_starts_open()
    {
        var store = new InMemoryIncidentStore();

        var incident = store.Add("Derrame de aceite", "Se detectó aceite cerca del taller", "Taller", IncidentSeverity.High);

        Assert.Equal(IncidentStatus.Open, incident.Status);
        Assert.Equal(IncidentSeverity.High, incident.Severity);
    }

    [Fact]
    public void Status_filter_returns_matching_incidents()
    {
        var store = new InMemoryIncidentStore();
        var incident = store.Add("Zona señalizada", "Se colocó señalización temporal", "Almacén", IncidentSeverity.Low);

        store.ChangeStatus(incident.Id, IncidentStatus.Resolved);

        Assert.Single(store.GetAll(IncidentStatus.Resolved));
    }

    [Fact]
    public void Unknown_incident_returns_null_when_status_changes()
    {
        var store = new InMemoryIncidentStore();

        var result = store.ChangeStatus(Guid.NewGuid(), IncidentStatus.Investigating);

        Assert.Null(result);
    }
}
