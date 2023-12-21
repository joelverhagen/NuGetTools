using Knapcode.NuGetTools.Website;
using Microsoft.ApplicationInsights.Extensibility;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddNuGetTools();
builder.Services.AddSingleton<ITelemetryInitializer, RequestSuccessInitializer>();

var app = builder.Build();

if (!app.Environment.IsDevelopment() && !app.Environment.IsAutomation())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
