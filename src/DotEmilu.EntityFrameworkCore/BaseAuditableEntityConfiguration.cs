namespace DotEmilu.EntityFrameworkCore;

public class BaseAuditableEntityConfiguration : IEntityTypeConfiguration<BaseAuditableEntity>
{
    public void Configure(EntityTypeBuilder<BaseAuditableEntity> builder)
    {
        builder
            .Property(s => s.Created)
            .IsRequired();

        builder
            .Property(s => s.CreatedBy)
            .IsRequired();

        builder
            .Property(s => s.LastModified)
            .IsRequired();

        builder
            .Property(s => s.LastModifiedBy)
            .IsRequired();

        builder
            .Property(s => s.Deleted)
            .IsRequired(false);

        builder
            .Property(s => s.DeletedBy)
            .IsRequired(false);
    }
}