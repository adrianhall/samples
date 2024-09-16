using Samples.Identity.Extensions;

// =========================================================
// Application Services
// =========================================================
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();                   // .NET Aspire

builder.AddDatabase();                          // Aspire-linked Database
builder.AddAspNetIdentity();                    // ASP.NET Identity
builder.Services.AddControllersWithViews();     // ASP.NET MVC
builder.AddWebOptimizer();                      // LigerShark WebOptimizer

// =========================================================
// HTTP Pipeline
// =========================================================
var app = builder.Build();

app.MapDefaultEndpoints();                      // .NET Aspire

await app.InitializeDatabaseAsync();            // Aspire-linked Database

app.UseHttpsRedirection();
app.UseWebOptimizer();                          // LigerShark WebOptimizer
app.UseStaticFiles();
app.UseRouting();
app.UseAspNetIdentity();                        // ASP.NET Identity
app.MapDefaultControllerRoute();                // ASP.NET MVC

// =========================================================
// Run the Service
// =========================================================
app.Run();
