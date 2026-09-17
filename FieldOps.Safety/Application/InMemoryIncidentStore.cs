using System.Collections.Concurrent;
using FieldOps.Safety.Domain;

namespace FieldOps.Safety.Application;

public sealed class InMemoryIncidentStore : IIncidentStore
{
    private readonly ConcurrentDictionary<Guid, Incident> incidents = new();

    public IReadOnlyCollection<Incident> GetAll(IncidentStatus? status = null) => incidents.Values
        .Where(incident => status is null || incident.Status == status)
        .OrderByDescending(incident => incident.ReportedAt)
        .ToArray();

    public Incident? Get(Guid id) => incidents.TryGetValue(id, out var incident) ? incident : null;

    public Incident Add(string title, string description, string area, IncidentSeverity severity)
    {
        var incident = new Incident(Guid.NewGuid(), title, description, area, severity, IncidentStatus.Open, DateTimeOffset.UtcNow);
        incidents[incident.Id] = incident;
        return incident;
    }

    public Incident? ChangeStatus(Guid id, IncidentStatus status)
    {
        while (incidents.TryGetValue(id, out var current))
        {
            var updated = current with { Status = status };
            if (incidents.TryUpdate(id, updated, current)) return updated;
        }

        return null;
    }
}
