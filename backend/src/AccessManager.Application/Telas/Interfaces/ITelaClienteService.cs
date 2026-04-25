using AccessManager.Application.Common;
using AccessManager.Application.Telas.DTOs;

namespace AccessManager.Application.Telas.Interfaces;

public interface ITelaClienteService
{
    Task<OperationResult<TelaClienteDto>> CreateAsync(CreateTelaClienteDto dto, CancellationToken cancellationToken);

    Task<OperationResult<IReadOnlyCollection<TelaClienteDto>>> GetAllAsync(
        Guid? clienteId,
        Guid? servidorId,
        CancellationToken cancellationToken);

    Task<OperationResult<TelaClienteDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<OperationResult<TelaClienteDto>> UpdateAsync(Guid id, UpdateTelaClienteDto dto, CancellationToken cancellationToken);
    Task<OperationResult<TelaClienteDto>> RenovarAsync(Guid id, RenovarTelaRequest request, CancellationToken cancellationToken);
    Task<OperationResult<TelaClienteDto>> TrocarServidorAsync(Guid id, TrocarServidorRequest request, CancellationToken cancellationToken);
    Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
