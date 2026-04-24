using AccessManager.Application.Common;
using AccessManager.Application.Servidores.DTOs;

namespace AccessManager.Application.Servidores.Interfaces;

public interface IServidorService
{
    Task<OperationResult<ServidorDto>> CreateAsync(CreateServidorDto dto, CancellationToken cancellationToken);
    Task<OperationResult<IReadOnlyCollection<ServidorDto>>> GetAllAsync(CancellationToken cancellationToken);
    Task<OperationResult<ServidorDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<OperationResult<ServidorDto>> UpdateAsync(Guid id, UpdateServidorDto dto, CancellationToken cancellationToken);
    Task<OperationResult<ServidorDto>> UpdateStatusAsync(Guid id, UpdateServidorStatusDto dto, CancellationToken cancellationToken);
    Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
