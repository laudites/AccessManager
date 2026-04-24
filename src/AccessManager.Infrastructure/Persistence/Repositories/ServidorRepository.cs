using AccessManager.Application.Servidores.Interfaces;
using AccessManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccessManager.Infrastructure.Persistence.Repositories;

public class ServidorRepository(AccessManagerDbContext dbContext) : IServidorRepository
{
    public async Task<IReadOnlyCollection<Servidor>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Servidores
            .AsNoTracking()
            .OrderBy(servidor => servidor.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<Servidor?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Servidores
            .FirstOrDefaultAsync(servidor => servidor.Id == id, cancellationToken);
    }

    public async Task AddAsync(Servidor servidor, CancellationToken cancellationToken)
    {
        await dbContext.Servidores.AddAsync(servidor, cancellationToken);
    }

    public void Update(Servidor servidor)
    {
        dbContext.Servidores.Update(servidor);
    }

    public void Remove(Servidor servidor)
    {
        dbContext.Servidores.Remove(servidor);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
