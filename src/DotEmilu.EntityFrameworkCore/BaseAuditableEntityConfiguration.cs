namespace DotEmilu.EntityFrameworkCore;

public sealed class BaseAuditableEntityConfiguration<TBaseAuditableEntity, TUserKey>(MappingStrategy strategy)
    : IEntityTypeConfiguration<TBaseAuditableEntity>
    where TBaseAuditableEntity : class, IBaseAuditableEntity<TUserKey>
    where TUserKey : struct
{
    public void Configure(EntityTypeBuilder<TBaseAuditableEntity> builder)
    {
        builder
            .HasNoKey();

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

        builder
            .UseIsDeleted();

        builder
            .ApplyMappingStrategy(strategy);
    }
}