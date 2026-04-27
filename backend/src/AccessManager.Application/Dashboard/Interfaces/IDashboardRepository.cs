using AccessManager.Application.Dashboard.DTOs;
using AccessManager.Domain.Entities;

namespace AccessManager.Application.Dashboard.Interfaces;

public interface IDashboardRepository
{
    Task<int> CountClientesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<TelaCliente>> GetTelasAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Servidor>> GetServidoresAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<LancamentoFinanceiro>> GetLancamentosFinanceirosAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<TelasPorServidorDto>> GetTelasPorServidorAsync(CancellationToken cancellationToken);
}
