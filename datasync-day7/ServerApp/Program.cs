using CommunityToolkit.Datasync.Server;
using CommunityToolkit.Datasync.Server.InMemory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using ServerApp.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IRepository<TodoItem>, InMemoryRepository<TodoItem>>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddDatasyncServices();
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
