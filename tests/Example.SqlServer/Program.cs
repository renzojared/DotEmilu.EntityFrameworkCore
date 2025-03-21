var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddSoftDeleteInterceptor()
    //.AddAuditableEntityInterceptor<ContextUser, Guid>()
    .AddAuditableEntityInterceptors(Assembly.GetExecutingAssembly())
    .AddDbContext<SqlServerContext>((sp, options) =>
    {
        options
            .UseSqlServer(connectionString: builder.Configuration.GetSection("DefaultConnection").Get<string>())
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGroup("api/person")
    .WithTags("person")
    .WithOpenApi()
    .MapPerson();

app.MapGroup("api/song")
    .WithTags("song")
    .WithOpenApi()
    .MapSong();

app.UseHttpsRedirection();

app.Run();