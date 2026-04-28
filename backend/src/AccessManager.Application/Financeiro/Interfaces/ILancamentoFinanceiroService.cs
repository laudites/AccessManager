using AccessManager.Application.Common;
using AccessManager.Application.Financeiro.DTOs;
using AccessManager.Domain.Enums;

namespace AccessManager.Application.Financeiro.Interfaces;

public interface ILancamentoFinanceiroService
{
    Task<OperationResult<LancamentoFinanceiroDto>> CreateAsync(
        CreateLancamentoFinanceiroDto dto,
        CancellationToken cancellationToken);

    Task<OperationResult<IReadOnlyCollection<LancamentoFinanceiroDto>>> GetAllAsync(
        Guid? clienteId,
        Guid? telaClienteId,
        StatusFinanceiro? statusFinanceiro,
        int? mes,
        int? ano,
        CancellationToken cancellationToken);

    Task<OperationResult<LancamentoFinanceiroDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<OperationResult<LancamentoFinanceiroDto>> UpdateAsync(Guid id, UpdateLancamentoFinanceiroDto dto, CancellationToken cancellationToken);
    Task<OperationResult<LancamentoFinanceiroDto>> MarcarComoPagoAsync(Guid id, CancellationToken cancellationToken);
    Task<OperationResult<IReadOnlyCollection<LancamentoFinanceiroDto>>> GerarPendentesAsync(
        GerarLancamentosFinanceirosRequest request,
        CancellationToken cancellationToken);
    Task<OperationResult<IReadOnlyCollection<LancamentoFinanceiroDto>>> GetPendentesAsync(CancellationToken cancellationToken);
    Task<OperationResult<IReadOnlyCollection<LancamentoFinanceiroDto>>> GetAtrasadosAsync(CancellationToken cancellationToken);
    Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
