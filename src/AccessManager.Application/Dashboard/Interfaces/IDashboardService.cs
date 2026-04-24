using AccessManager.Application.Common;
using AccessManager.Application.Dashboard.DTOs;

namespace AccessManager.Application.Dashboard.Interfaces;

public interface IDashboardService
{
    Task<OperationResult<DashboardResumoDto>> GetResumoAsync(CancellationToken cancellationToken);
    Task<OperationResult<IReadOnlyCollection<TelasPorServidorDto>>> GetTelasPorServidorAsync(CancellationToken cancellationToken);
}
