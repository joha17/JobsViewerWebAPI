using Microsoft.EntityFrameworkCore;
using JobsViewer_API.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.Extensions.Configuration;
using JobsViewer_API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<JobsViewer_API.CustomExceptionFilter>();
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

IConfiguration configuration;
configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();


DecryptConnectionString decryptConnectionString = new DecryptConnectionString(configuration);
string decryptedConnectionString = await decryptConnectionString.DecryptConnectionAsync(configuration["ConnectionStrings:MSDBConnectionString1"].ToString());

builder.Services.AddDbContext<JobsViewerContext>(options =>
    options.UseSqlServer(decryptedConnectionString));

builder.Services.AddTransient<DecryptConnectionString>();

var app = builder.Build();

// Configurar middleware para manejo de excepciones
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Habilitar archivos estáticos
app.UseStaticFiles();

app.Run();
