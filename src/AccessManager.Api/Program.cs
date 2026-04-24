using AccessManager.Infrastructure;
using AccessManager.Application.Clientes.Interfaces;
using AccessManager.Application.Clientes.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
