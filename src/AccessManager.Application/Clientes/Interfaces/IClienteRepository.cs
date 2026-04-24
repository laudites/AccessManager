using AccessManager.Domain.Entities;

namespace AccessManager.Application.Clientes.Interfaces;

public interface IClienteRepository
{
    Task<IReadOnlyCollection<Cliente>> GetAllAsync(CancellationToken cancellationToken);
    Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Cliente cliente, CancellationToken cancellationToken);
    void Update(Cliente cliente);
    void Remove(Cliente cliente);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
