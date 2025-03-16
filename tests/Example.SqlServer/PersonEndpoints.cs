namespace Example.SqlServer;

public static class PersonEndpoints
{
    public static RouteGroupBuilder MapPerson(this RouteGroupBuilder app)
    {
        app.MapPost(string.Empty, async (SqlServerContext context) =>
        {
            var person = new Person
            {
                Name = $"John Doe - {DateTime.Now}",
            };
            await context.Persons.AddAsync(person);
            await context.SaveChangesAsync();
        });

        app.MapGet(string.Empty,
            async (SqlServerContext context, CancellationToken cancellationToken) =>
            {
                var persons = await context.Persons
                    .Select(s => new { s.Id, s.Name, s.IsDeleted })
                    .IgnoreQueryFilters()
                    .ToListAsync(cancellationToken);
                return Results.Ok(new { persons = persons, count = persons.Count });
            });
        
        app.MapGet("with-filter",
            async (SqlServerContext context, CancellationToken cancellationToken) =>
            {
                var persons = await context.Persons
                    .Select(s => new { s.Id, s.Name, s.IsDeleted })
                    .ToListAsync(cancellationToken);
                return Results.Ok(new { persons = persons, count = persons.Count });
            });

        app.MapPut("update-aleatory", async (SqlServerContext context, CancellationToken cancellationToken) =>
        {
            var person = await context.Persons.FirstOrDefaultAsync(cancellationToken);
            person!.LastName = $"Last Name - {DateTime.Now}";
            await context.SaveChangesAsync(cancellationToken);
        });

        app.MapPut(string.Empty,
            async (Guid id, SqlServerContext context, CancellationToken cancellationToken) =>
            {
                var person = await context.Persons
                    .Where(s => s.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);
                person!.LastName = $"Last Name - {DateTime.Now}";
                await context.SaveChangesAsync(cancellationToken);
            });

        app.MapDelete(string.Empty,
            async (Guid id, SqlServerContext context, CancellationToken cancellationToken) =>
            {
                var person = await context.Persons
                    .Where(s => s.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);
                person!.IsDeleted = true;
                await context.SaveChangesAsync(cancellationToken);
            });

        app.MapDelete("soft-delete",
            async (Guid id, SqlServerContext context, CancellationToken cancellationToken) =>
            {
                var person = await context.Persons.Where(s => s.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);
                context.Persons.Remove(person!);
                await context.SaveChangesAsync(cancellationToken);
            });

        return app;
    }
}