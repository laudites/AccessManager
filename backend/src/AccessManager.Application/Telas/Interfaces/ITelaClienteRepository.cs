using AccessManager.Domain.Entities;

namespace AccessManager.Application.Telas.Interfaces;

public interface ITelaClienteRepository
{
    Task<IReadOnlyCollection<TelaCliente>> GetAllAsync(
        Guid? clienteId,
        Guid? servidorId,
        CancellationToken cancellationToken);

    Task<TelaCliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(TelaCliente telaCliente, CancellationToken cancellationToken);
    void Update(TelaCliente telaCliente);
    void Remove(TelaCliente telaCliente);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
