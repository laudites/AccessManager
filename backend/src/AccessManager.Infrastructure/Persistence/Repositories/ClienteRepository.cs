using AccessManager.Application.Clientes.Interfaces;
using AccessManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccessManager.Infrastructure.Persistence.Repositories;

public class ClienteRepository(AccessManagerDbContext dbContext) : IClienteRepository
{
    public async Task<IReadOnlyCollection<Cliente>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Clientes
            .AsNoTracking()
            .OrderBy(cliente => cliente.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Clientes
            .FirstOrDefaultAsync(cliente => cliente.Id == id, cancellationToken);
    }

    public async Task AddAsync(Cliente cliente, CancellationToken cancellationToken)
    {
        await dbContext.Clientes.AddAsync(cliente, cancellationToken);
    }

    public void Update(Cliente cliente)
    {
        dbContext.Clientes.Update(cliente);
    }

    public void Remove(Cliente cliente)
    {
        dbContext.Clientes.Remove(cliente);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
