using AccessManager.Application.Clientes.Interfaces;
using AccessManager.Application.Dashboard.Interfaces;
using AccessManager.Application.Financeiro.Interfaces;
using AccessManager.Application.Servidores.Interfaces;
using AccessManager.Application.Telas.Interfaces;
using AccessManager.Infrastructure.Persistence;
using AccessManager.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AccessManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        }

        services.AddDbContext<AccessManagerDbContext>(options =>
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0))));

        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IServidorRepository, ServidorRepository>();
        services.AddScoped<ITelaClienteRepository, TelaClienteRepository>();
        services.AddScoped<IRenovacaoTelaHistoricoRepository, RenovacaoTelaHistoricoRepository>();
        services.AddScoped<ILancamentoFinanceiroRepository, LancamentoFinanceiroRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();

        return services;
    }
}
