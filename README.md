# DotEmilu EF Core

A simple, easy-to-use .NET library designed for EF Core interceptors and auditable entities.

## Features

- Supports **CREATE**, **UPDATE**, **DELETE** tracking.
- Provides properties for **base** and **auditable entities**.
- Contracts (interfaces) and abstractions for easy inheritance and implementation.
- Supports **generic keys** for entity IDs and user keys for audit purposes.
- `Context User` supports **generic ID types** for better flexibility.

### Enhanced Interceptors
- **SoftDeleteInterceptor**: Handles soft deletes for entities implementing `IBaseEntity`.
- **AuditableEntityInterceptor**: Tracks changes for entities implementing `IBaseAuditableEntity`, now with support for generic user keys.

### Configuration and Extensibility
- Base configurations using **IEntityTypeConfiguration** for a consistent mapping strategy.
- Extension methods to register multiple configurations:
   - By passing an **assembly** or individually by **type**.
- Flexible Dependency Injection (DI) setup:
   - Register interceptors individually or by **assembly** for multiple interceptors.

## Why Use DotEmilu EF Core?
Ideal for projects requiring:
- Consistent tracking and flexible auditing mechanisms.
- Easy integration of EF Core interceptors.
- A scalable, customizable approach to entity configurations and dependency injection.

## How to Use

Follow these simple steps to get started:

1. **Define Your Database Entities**  
   Create your database entities as needed for your application.

   [View all contracts for `BaseEntity`](/src/DotEmilu.EntityFrameworkCore/BaseEntity.cs)

   [View all contracts for `BaseAuditableEntity`](/src/DotEmilu.EntityFrameworkCore/BaseAuditableEntity.cs)

   ```csharp
    public class Person : BaseEntity<Guid>
    {
        public string Name { get; set; } = null!;
        public string? LastName { get; set; }
    }

    public class Song : BaseAuditableEntity<int, Guid>
    {
        public required string Name { get; set; }
    }
   ```

2. **Implement Context User Information**  
   Create an implementation to retrieve user context information (e.g., user ID, roles).

   ```csharp
   //Feel free to inject your necessary dependencies to provide user information.
    public class ContextUser : IContextUser<Guid>
    {
        public Guid Id => Guid.Parse("66931274-341d-41a0-0ec4-08dd5e9e0461");
    }
   ```

3. **Working with Unit of Work**  
   Inherit your entity interfaces and use EF Core features in a decoupled manner.

   ```csharp
   public interface ICommands : IUnitOfWork
   {
       DbSet<Person> Persons { get; }
   }

   public class PersonHandler(ICommands commands)
   {
       public async Task WorkAsync()
       {
           var person = await commands.Persons.FirstOrDefaultAsync();
           person!.Name = "NEW NAME!";
           await commands.SaveChangesAsync();
       }
   }
   ```

4. **Registering Base Configurations**  
   Different methods to register base configurations.

   ```csharp
   public class SqlServerContext(DbContextOptions<SqlServerContext> options) : DbContext(options)
   {
       protected override void OnModelCreating(ModelBuilder modelBuilder)
       {
           base.OnModelCreating(modelBuilder);
           modelBuilder
               //.ApplyBaseAuditableEntityConfiguration<int>(strategy: MappingStrategy.Tpt) //method 1
               .ApplyBaseAuditableEntityConfiguration(Assembly.GetExecutingAssembly()) //method 2
               //.ApplyBaseEntityConfiguration<Guid>(strategy: MappingStrategy.Tph, enableRowVersion: true) //method 1
               .ApplyBaseEntityConfiguration(Assembly.GetExecutingAssembly()) //method 2
               .ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
       }
       public DbSet<Person> Persons => Set<Person>();
       public DbSet<Song> Songs => Set<Song>();
   }
   ```

5. **Register the Interceptor**  
   Add your custom interceptor to the dependency injection container in your application.

   [View sample](tests/Example.SqlServer/Program.cs)

   ```csharp
   builder.Services
       .AddSoftDeleteInterceptor()
       //.AddAuditableEntityInterceptor<ContextUser, Guid>() //method 1
       .AddAuditableEntityInterceptors(Assembly.GetExecutingAssembly()) //method 2
   ```

[See complete example](tests/Example.SqlServer)