namespace Example.SqlServer;

public class ContextUser : IContextUser<Guid>
{
    public Guid Id => Guid.Parse("66931274-341d-41a0-0ec4-08dd5e9e0461");
}