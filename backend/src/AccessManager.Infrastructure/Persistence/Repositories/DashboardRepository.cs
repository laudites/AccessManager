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

    public async Task<IReadOnlyCollection<Servidor>> GetServidoresAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Servidores
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<LancamentoFinanceiro>> GetLancamentosFinanceirosAsync(
        CancellationToken cancellationToken)
    {
        return await dbContext.LancamentosFinanceiros
            .AsNoTracking()
            .Include(lancamento => lancamento.Cliente)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<TelasPorServidorDto>> GetTelasPorServidorAsync(
        CancellationToken cancellationToken)
    {
        return await dbContext.Servidores
            .AsNoTracking()
            .Select(servidor => new TelasPorServidorDto
            {
                ServidorId = servidor.Id,
                NomeServidor = servidor.Nome,
                QuantidadeCreditos = servidor.QuantidadeCreditos,
                ValorCustoCredito = servidor.ValorCustoCredito,
                QuantidadeClientes = servidor.Telas
                    .Where(tela => tela.Ativo)
                    .Select(tela => tela.ClienteId)
                    .Distinct()
                    .Count(),
                QuantidadeTelas = servidor.Telas.Count(tela => tela.Ativo),
                CustoEstimado = servidor.Telas.Count(tela => tela.Ativo) * servidor.ValorCustoCredito
            })
            .OrderBy(item => item.NomeServidor)
            .ToListAsync(cancellationToken);
    }
}
