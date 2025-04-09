using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class AuditInterceptor : ISaveChangesInterceptor
{
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

        var audits = new List<CaseAudit>();
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is IAuditable auditableEntity)
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    audits.Add(new CaseAudit
                    {
                        EntityType = entry.Entity.GetType().Name,
                        EntityId = auditableEntity.Id,
                        Action = entry.State == EntityState.Added ? ActionType.Created : ActionType.Updated,
                        Timestamp = DateTime.UtcNow
                    });
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
