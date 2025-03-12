namespace Example.SqlServer;

public static class SongEndpoints
{
    public static RouteGroupBuilder MapSong(this RouteGroupBuilder app)
    {
        app.MapPost(string.Empty, async (SqlServerContext context) =>
        {
            var song = new Song
            {
                Name = $"Mars - {DateTime.Now}",
            };
            await context.Songs.AddAsync(song);
            await context.SaveChangesAsync();
        });

        app.MapGet(string.Empty,
            async (SqlServerContext context, CancellationToken cancellationToken) =>
            {
                var songs = await context.Songs
                    .Select(s => new { s.Id, s.Name, s.IsDeleted })
                    .IgnoreQueryFilters()
                    .ToListAsync(cancellationToken);
                return Results.Ok(new { songs = songs, count = songs.Count });
            });

        app.MapGet("with-filter",
            async (SqlServerContext context, CancellationToken cancellationToken) =>
            {
                var songs = await context.Songs
                    .Select(s => new { s.Id, s.Name, s.IsDeleted })
                    .ToListAsync(cancellationToken);
                return Results.Ok(new { songs = songs, count = songs.Count });
            });

        app.MapPut("update-aleatory", async (SqlServerContext context, CancellationToken cancellationToken) =>
        {
            var song = await context.Songs.FirstOrDefaultAsync(cancellationToken);
            song!.Name = $"Bon Jovi - {DateTime.Now}";
            await context.SaveChangesAsync(cancellationToken);
        });

        app.MapPut(string.Empty, async (Guid id, SqlServerContext context, CancellationToken cancellationToken) =>
        {
            var song = await context.Songs
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
            song!.Name = $"Bon Jovi - {DateTime.Now}";
            await context.SaveChangesAsync(cancellationToken);
        });

        app.MapDelete(string.Empty, async (Guid id, SqlServerContext context, CancellationToken cancellationToken) =>
        {
            var song = await context.Songs
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
            song!.IsDeleted = true;
            await context.SaveChangesAsync(cancellationToken);
        });

        app.MapDelete("soft-delete", async (Guid id, SqlServerContext context, CancellationToken cancellationToken) =>
        {
            var song = await context.Songs
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
            context.Songs.Remove(song!);
            await context.SaveChangesAsync(cancellationToken);
        });

        return app;
    }
}