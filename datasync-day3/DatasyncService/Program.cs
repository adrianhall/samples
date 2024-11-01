using CommunityToolkit.Datasync.Server;
using CommunityToolkit.Datasync.Server.Swashbuckle;
using DatasyncService.Database;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen(options => options.AddDatasyncControllers());

builder.Services.AddDatasyncServices();
builder.Services.AddControllers();

var app = builder.Build();

using (CancellationTokenSource cts = new(TimeSpan.FromMinutes(5)))
{
    using AsyncServiceScope scope = app.Services.CreateAsyncScope();
    IDbInitializer svc = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    await svc.InitializeAsync(cts.Token);
}

app.UseSwagger().UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseRouting();
app.MapControllers();

app.Run();
