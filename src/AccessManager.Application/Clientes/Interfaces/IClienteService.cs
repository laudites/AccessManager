using AccessManager.Application.Clientes.DTOs;
using AccessManager.Application.Common;

namespace AccessManager.Application.Clientes.Interfaces;

public interface IClienteService
{
    Task<OperationResult<ClienteDto>> CreateAsync(CreateClienteDto dto, CancellationToken cancellationToken);
    Task<OperationResult<IReadOnlyCollection<ClienteDto>>> GetAllAsync(CancellationToken cancellationToken);
    Task<OperationResult<ClienteDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<OperationResult<ClienteDto>> UpdateAsync(Guid id, UpdateClienteDto dto, CancellationToken cancellationToken);
    Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
