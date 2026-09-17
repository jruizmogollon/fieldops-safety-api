using FieldOps.Safety.Domain;

namespace FieldOps.Safety.Application;

public interface IIncidentStore
{
    IReadOnlyCollection<Incident> GetAll(IncidentStatus? status = null);
    Incident? Get(Guid id);
    Incident Add(string title, string description, string area, IncidentSeverity severity);
    Incident? ChangeStatus(Guid id, IncidentStatus status);
}
