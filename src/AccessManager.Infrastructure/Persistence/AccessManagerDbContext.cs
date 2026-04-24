using AccessManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccessManager.Infrastructure.Persistence;

public class AccessManagerDbContext(DbContextOptions<AccessManagerDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Servidor> Servidores => Set<Servidor>();
    public DbSet<TelaCliente> TelasClientes => Set<TelaCliente>();
    public DbSet<RenovacaoTelaHistorico> RenovacoesTelasHistorico => Set<RenovacaoTelaHistorico>();
    public DbSet<LancamentoFinanceiro> LancamentosFinanceiros => Set<LancamentoFinanceiro>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccessManagerDbContext).Assembly);
    }
}
