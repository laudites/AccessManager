using AccessManager.Domain.Entities;

namespace AccessManager.Application.Servidores.Interfaces;

public interface IServidorRepository
{
    Task<IReadOnlyCollection<Servidor>> GetAllAsync(CancellationToken cancellationToken);
    Task<Servidor?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Servidor servidor, CancellationToken cancellationToken);
    void Update(Servidor servidor);
    void Remove(Servidor servidor);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
