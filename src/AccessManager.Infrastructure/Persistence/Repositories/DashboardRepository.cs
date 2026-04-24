using AccessManager.Application.Dashboard.DTOs;
using AccessManager.Application.Dashboard.Interfaces;
using AccessManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccessManager.Infrastructure.Persistence.Repositories;

public class DashboardRepository(AccessManagerDbContext dbContext) : IDashboardRepository
{
    public async Task<int> CountClientesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Clientes
            .AsNoTracking()
            .CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<TelaCliente>> GetTelasAsync(CancellationToken cancellationToken)
    {
        return await dbContext.TelasClientes
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<LancamentoFinanceiro>> GetLancamentosFinanceirosAsync(
        CancellationToken cancellationToken)
    {
        return await dbContext.LancamentosFinanceiros
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<TelasPorServidorDto>> GetTelasPorServidorAsync(
        CancellationToken cancellationToken)
    {
        return await dbContext.TelasClientes
            .AsNoTracking()
            .GroupBy(tela => new
            {
                tela.ServidorId,
                NomeServidor = tela.Servidor == null ? string.Empty : tela.Servidor.Nome
            })
            .Select(grupo => new TelasPorServidorDto
            {
                ServidorId = grupo.Key.ServidorId,
                NomeServidor = grupo.Key.NomeServidor,
                QuantidadeTelas = grupo.Count()
            })
            .OrderBy(item => item.NomeServidor)
            .ToListAsync(cancellationToken);
    }
}
