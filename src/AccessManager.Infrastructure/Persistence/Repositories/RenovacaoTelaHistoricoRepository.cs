using AccessManager.Application.Telas.Interfaces;
using AccessManager.Domain.Entities;

namespace AccessManager.Infrastructure.Persistence.Repositories;

public class RenovacaoTelaHistoricoRepository(AccessManagerDbContext dbContext) : IRenovacaoTelaHistoricoRepository
{
    public async Task AddAsync(RenovacaoTelaHistorico historico, CancellationToken cancellationToken)
    {
        await dbContext.RenovacoesTelasHistorico.AddAsync(historico, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
