namespace FieldOps.Safety.Domain;

public sealed record CreateIncidentRequest(
    string Title,
    string Description,
    string Area,
    IncidentSeverity Severity = IncidentSeverity.Medium);

public sealed record ChangeIncidentStatusRequest(IncidentStatus Status);
