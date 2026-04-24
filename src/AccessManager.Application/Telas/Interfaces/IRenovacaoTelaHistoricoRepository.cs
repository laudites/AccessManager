using AccessManager.Domain.Entities;

namespace AccessManager.Application.Telas.Interfaces;

public interface IRenovacaoTelaHistoricoRepository
{
    Task AddAsync(RenovacaoTelaHistorico historico, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
