using LoggerProviderExtensions;
using Microsoft.Extensions.Options;
using TestWeb;
using TestWeb.Components;
using TestWeb.Components.Pages;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Logging.AddLocalFileLogger(setting =>
{
    setting.SaveByCategory = true;
}).AddDbLogger(setting =>
{
    setting.DbLoggerFacotry = () => new Dblogger();
});

var app = builder.Build();
var o = app.Services.GetService<IOptions<LoggerFilterOptions>>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
