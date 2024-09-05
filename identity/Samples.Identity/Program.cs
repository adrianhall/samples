using Samples.Identity.Extensions;

// =========================================================
// Application Services
// =========================================================
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();                   // .NET Aspire

builder.AddDatabase();                          // Aspire-linked Database
builder.AddAspNetIdentity();                    // ASP.NET Identity
builder.Services.AddControllersWithViews();     // ASP.NET MVC
builder.Services.AddRazorPages();               // For ASP.NET Identity

// =========================================================
// HTTP Pipeline
// =========================================================
var app = builder.Build();

app.MapDefaultEndpoints();                      // .NET Aspire

await app.InitializeDatabaseAsync();            // Aspire-linked Database

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();                         // ASP.NET Identity
app.MapDefaultControllerRoute();                // ASP.NET MVC
app.MapRazorPages();                            // For ASP.NET Identity

// =========================================================
// Run the Service
// =========================================================
app.Run();
