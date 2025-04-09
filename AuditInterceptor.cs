using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class AuditInterceptor : ISaveChangesInterceptor
{
    private readonly IUserContextService _userContextService;

    public AuditInterceptor(IUserContextService userContextService)
    {
        _userContextService = userContextService;
    }

    public async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;

        if (context == null)
        {
            return await ValueTask.FromResult(result);
        }

        var userId = _userContextService.UserId;
        var fullname = _userContextService.Fullname;

        var audits = new List<CaseAudit>();
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is ICaseAuditable auditableEntity)
            {
                if (entry.State == EntityState.Added)
                {
                    audits.Add(new CaseAudit
                    {
                        Action = entry.State == EntityState.Added ? ActionType.Created : ActionType.Updated,
                        Timestamp = DateTime.UtcNow,
                        CaseId = auditableEntity.CaseId,
                        UserId = int.Parse(userId ?? "0"),
                        Fullname = fullname ?? "Unknown",
                        FieldName = entry.CurrentValues.Properties.FirstOrDefault()?.Name ?? "Unknown",
                        FieldValue = entry.CurrentValues[entry.CurrentValues.Properties.FirstOrDefault()?.Name ?? "Unknown"]?.ToString() ?? "Unknown"
                    });
                }
                else if( entry.State == EntityState.Modified)
                {
                    foreach (var property in entry.Properties)
                    {
                        // Check if the property has been modified
                        if (property.IsModified || entry.State == EntityState.Added)
                        {
                            audits.Add(new CaseAudit
                            {
                                Action = entry.State == EntityState.Added ? ActionType.Created : ActionType.Updated,
                                Timestamp = DateTime.UtcNow,
                                CaseId = auditableEntity.CaseId,
                                UserId = int.Parse(userId ?? "0"),
                                Fullname = fullname ?? "Unknown",
                                FieldName = property.Metadata.Name, // Log the property name
                                FieldValue = property.CurrentValue?.ToString() ?? "NULL" // Log the new value
                            });
                        }
                    }
                }
            }
        }

        if (audits.Count > 0)
        {
            context.Set<CaseAudit>().AddRange(audits);
        }

        return await ValueTask.FromResult(result);
    }
}
