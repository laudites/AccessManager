using AccessManager.Infrastructure;
using AccessManager.Application.Clientes.Interfaces;
using AccessManager.Application.Clientes.Services;
using AccessManager.Application.Servidores.Interfaces;
using AccessManager.Application.Servidores.Services;
using AccessManager.Application.Telas.Interfaces;
using AccessManager.Application.Telas.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IServidorService, ServidorService>();
builder.Services.AddScoped<ITelaClienteService, TelaClienteService>();
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
