using FieldOps.Safety.Domain;
using FieldOps.Safety.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FieldOps.Safety.Application;

public sealed class EfIncidentStore(IncidentDbContext db) : IIncidentStore
{
    public IReadOnlyCollection<Incident> GetAll(IncidentStatus? status = null) => db.Incidents
        .AsNoTracking()
        .Where(incident => status == null || incident.Status == status)
        .ToList()
        .OrderByDescending(incident => incident.ReportedAt)
        .Select(incident => incident.ToDomain())
        .ToArray();

    public Incident? Get(Guid id) => db.Incidents
        .AsNoTracking()
        .SingleOrDefault(incident => incident.Id == id)
        ?.ToDomain();

    public Incident Add(string title, string description, string area, IncidentSeverity severity)
    {
        var incident = new Incident(
            Guid.NewGuid(),
            title,
            description,
            area,
            severity,
            IncidentStatus.Open,
            DateTimeOffset.UtcNow);

        db.Incidents.Add(IncidentRecord.FromDomain(incident));
        db.SaveChanges();
        return incident;
    }

    public Incident? ChangeStatus(Guid id, IncidentStatus status)
    {
        var record = db.Incidents.SingleOrDefault(incident => incident.Id == id);
        if (record is null) return null;

        record.Status = status;
        db.SaveChanges();
        return record.ToDomain();
    }
}
