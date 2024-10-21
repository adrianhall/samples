using CommunityToolkit.Datasync.Server;
using CommunityToolkit.Datasync.Server.InMemory;
using CommunityToolkit.Datasync.Server.Swashbuckle;
using InMemoryDatasyncService.Database;
using InMemoryDatasyncService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new ApplicationException("DefaultConnection is not set");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.EnableDetailedErrors();
    options.EnableSensitiveDataLogging();
    options.EnableThreadSafetyChecks();
});
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

builder.Services.AddSingleton<IRepository<CategoryDTO>, InMemoryRepository<CategoryDTO>>();
builder.Services.AddScoped<IDatasyncInitializer, DatasyncInitializer>();

builder.Services.AddAutoMapper(typeof(TodoItemProfile));

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options => options.AddDatasyncControllers());

builder.Services.AddDatasyncServices();
builder.Services.AddControllers();

var app = builder.Build();

TimeSpan allowedInitializationTime = TimeSpan.FromMinutes(5);
CancellationTokenSource cts = new();

using (AsyncServiceScope scope = app.Services.CreateAsyncScope())
{
    IDbInitializer dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    IDatasyncInitializer datasyncInitializer = scope.ServiceProvider.GetRequiredService<IDatasyncInitializer>();
    cts.CancelAfter(allowedInitializationTime);
    try
    {
        CancellationToken ct = cts.Token;
        await dbInitializer.InitializeAsync(ct);
        await datasyncInitializer.InitializeAsync(ct);
    }
    catch (OperationCanceledException)
    {
        throw new ApplicationException($"Initialization failed to complete within {allowedInitializationTime}");
    }
}

app.UseSwagger().UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseRouting();
app.MapControllers();

app.Run();
