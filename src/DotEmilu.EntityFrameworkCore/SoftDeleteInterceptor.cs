namespace DotEmilu.EntityFrameworkCore;

public sealed class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        SetSoftDelete(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        SetSoftDelete(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void SetSoftDelete(DbContext? context)
    {
        if (context is null) return;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State != EntityState.Deleted) continue;

            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
        }
    }
}