using AccessManager.Api.BackgroundServices;
using AccessManager.Infrastructure;
using AccessManager.Application.Clientes.Interfaces;
using AccessManager.Application.Clientes.Services;
using AccessManager.Application.Dashboard.Interfaces;
using AccessManager.Application.Dashboard.Services;
using AccessManager.Application.Financeiro.Interfaces;
using AccessManager.Application.Financeiro.Services;
using AccessManager.Application.Servidores.Interfaces;
using AccessManager.Application.Servidores.Services;
using AccessManager.Application.Telas.Interfaces;
using AccessManager.Application.Telas.Services;

const string DevelopmentCorsPolicy = "DevelopmentCors";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy(DevelopmentCorsPolicy, policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IServidorService, ServidorService>();
builder.Services.AddScoped<ITelaClienteService, TelaClienteService>();
builder.Services.AddScoped<ILancamentoFinanceiroService, LancamentoFinanceiroService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddHostedService<GerarLancamentosFinanceirosPendentesBackgroundService>();
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors(DevelopmentCorsPolicy);
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();
