using TestWeb.Components;
using MT.Toolkit.LogTool;
using TestWeb.Components.Pages;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Logging.AddLocalFileLogger(setting =>
{
    //setting.SaveByCategory = true;
    setting.SetFileWriteLevel<Counter>(LogLevel.Trace);
});

var app = builder.Build();

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
