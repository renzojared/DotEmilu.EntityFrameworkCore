namespace DotEmilu.EntityFrameworkCore;

/// <summary>
/// Represents the type of the user identifier performing the audit action.
/// It is recommended to use numeric types (such as int, long) or Guid for efficient user identification.
/// Although any struct type is allowed, using common types facilitates interoperability and performance.
/// </summary>
/// <typeparam name="TUserKey">Type of the audit user identifier.</typeparam>
public interface IBaseAuditableEntity<TUserKey>
    where TUserKey : struct
{
    public DateTimeOffset Created { get; set; }
    public TUserKey CreatedBy { get; set; }
    public DateTimeOffset LastModified { get; set; }
    public TUserKey LastModifiedBy { get; set; }
    public DateTimeOffset? Deleted { get; set; }
    public TUserKey? DeletedBy { get; set; }
}

/// <summary>
/// Abstract base class that provides common properties for entities requiring auditing.
/// Designed to be inherited by entities that need to record information about record creation, modification, and deletion.
/// </summary>
/// <typeparam name="TUserKey">Type of the audit user identifier.</typeparam>
public abstract class BaseAuditableEntity<TUserKey> : IBaseAuditableEntity<TUserKey>
    where TUserKey : struct
{
    public DateTimeOffset Created { get; set; }
    public TUserKey CreatedBy { get; set; }
    public DateTimeOffset LastModified { get; set; }
    public TUserKey LastModifiedBy { get; set; }
    public DateTimeOffset? Deleted { get; set; }
    public TUserKey? DeletedBy { get; set; }
}

/// <summary>
/// Abstract base class that combines the properties of a base entity and an auditable entity.
/// Designed to be inherited by entities that require both an identifier and audit information.
/// </summary>
/// <typeparam name="TKey">Type of the primary key for the entity.</typeparam>
/// <typeparam name="TUserKey">Type of the audit user identifier.</typeparam>
public abstract class BaseAuditableEntity<TKey, TUserKey> : IBaseEntity<TKey>, IBaseAuditableEntity<TUserKey>
    where TKey : struct
    where TUserKey : struct
{
    public TKey Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset Created { get; set; }
    public TUserKey CreatedBy { get; set; }
    public DateTimeOffset LastModified { get; set; }
    public TUserKey LastModifiedBy { get; set; }
    public DateTimeOffset? Deleted { get; set; }
    public TUserKey? DeletedBy { get; set; }
}

/// <summary>
/// Abstract base class for auditable entities, using Guid as the primary key and user identifier.
/// This class will be removed in future versions.
/// Please use BaseAuditableEntity&lt;TKey, TUserKey&gt; instead.
/// </summary>
public abstract class BaseAuditableEntity : BaseAuditableEntity<Guid, Guid>;