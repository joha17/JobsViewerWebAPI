using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using JobViewerWebApp.Components;
using JobViewerWebApp.Services;
using Microsoft.AspNetCore.Components.Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


IConfiguration configuration;
configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

builder.Services.AddHttpClient("JobsApiClient", client =>
{
    client.BaseAddress = new Uri(configuration["UrlApi:url"]);
});

builder.Services.AddScoped<SysJobsService>();
builder.Services.AddScoped<SysJobsHistoryService>();

builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrapProviders()
    .AddFontAwesomeIcons();

// Configure circuit options
builder.Services.Configure<CircuitOptions>(options =>
{
    options.DisconnectedCircuitMaxRetained = 100;
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseAntiforgery();


app.Run();
