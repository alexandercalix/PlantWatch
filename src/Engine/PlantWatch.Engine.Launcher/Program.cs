using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using Serilog;
using Blazored.Toast;
using Blazored.Modal;
using FluentValidation;
using PlantWatch.Engine.Applications.WebApi;
using PlantWatch.Engine.Applications.Data.WebApi;
using PlantWatch.Engine.Core.Common;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// --- Logging with Serilog ---
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// --- Services ---
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// MudBlazor UI services
builder.Services.AddMudServices();

// Toasts & Modals
builder.Services.AddBlazoredToast();
builder.Services.AddBlazoredModal();

// FluentValidation (auto-discovery)
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// AddHttpClient if consuming APIs
builder.Services.AddHttpClient();


// --- Database Driver Manager ---
builder.Services.AddEngineDrivers(options =>
{
    options.LiteDbPath = "data/plantwatch.db";
    options.LiteDbPassword = "abcd1234";
});

builder.Services.AddEngineData(options =>
{
    options.LiteDbPath = "data/plantwatch.db";
    options.LiteDbPassword = "abcd1234";
});

// --- Build app ---
var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
