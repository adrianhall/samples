using CommunityToolkit.Datasync.Server;
using Microsoft.EntityFrameworkCore;
using TodoApp.Server;
using TodoApp.Server.Database;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new ApplicationException("DefaultConnection is not found in the configuration.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.EnableDetailedErrors();
    options.EnableSensitiveDataLogging();
    options.EnableThreadSafetyChecks();
});

builder.Services.AddDatasyncServices();
builder.Services.AddControllers();

var app = builder.Build();

await app.InitializeDatabaseAsync(TimeSpan.FromMinutes(5));

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
