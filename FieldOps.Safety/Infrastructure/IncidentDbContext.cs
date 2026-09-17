using FieldOps.Safety.Domain;
using Microsoft.EntityFrameworkCore;

namespace FieldOps.Safety.Infrastructure;

public sealed class IncidentRecord
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public IncidentSeverity Severity { get; set; }
    public IncidentStatus Status { get; set; }
    public DateTimeOffset ReportedAt { get; set; }

    public Incident ToDomain() => new(Id, Title, Description, Area, Severity, Status, ReportedAt);

    public static IncidentRecord FromDomain(Incident incident) => new()
    {
        Id = incident.Id,
        Title = incident.Title,
        Description = incident.Description,
        Area = incident.Area,
        Severity = incident.Severity,
        Status = incident.Status,
        ReportedAt = incident.ReportedAt
    };
}

public sealed class IncidentDbContext(DbContextOptions<IncidentDbContext> options) : DbContext(options)
{
    public DbSet<IncidentRecord> Incidents => Set<IncidentRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var incident = modelBuilder.Entity<IncidentRecord>();
        incident.HasKey(item => item.Id);
        incident.Property(item => item.Title).HasMaxLength(160).IsRequired();
        incident.Property(item => item.Description).HasMaxLength(2000).IsRequired();
        incident.Property(item => item.Area).HasMaxLength(120).IsRequired();
        incident.Property(item => item.Severity).HasConversion<string>().HasMaxLength(20);
        incident.Property(item => item.Status).HasConversion<string>().HasMaxLength(20);
        incident.Property(item => item.ReportedAt).IsRequired();
    }
}
