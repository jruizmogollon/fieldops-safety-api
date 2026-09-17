namespace FieldOps.Safety.Domain;

public enum IncidentSeverity
{
    Low,
    Medium,
    High,
    Critical
}

public enum IncidentStatus
{
    Open,
    Investigating,
    Resolved
}

public sealed record Incident(
    Guid Id,
    string Title,
    string Description,
    string Area,
    IncidentSeverity Severity,
    IncidentStatus Status,
    DateTimeOffset ReportedAt);
