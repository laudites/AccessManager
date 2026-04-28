using AccessManager.Domain.Entities;
using AccessManager.Domain.Enums;

namespace AccessManager.Application.Financeiro.Interfaces;

public interface ILancamentoFinanceiroRepository
{
    Task<IReadOnlyCollection<LancamentoFinanceiro>> GetAllAsync(
        Guid? clienteId,
        Guid? telaClienteId,
        StatusFinanceiro? statusFinanceiro,
        int? mes,
        int? ano,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<LancamentoFinanceiro>> GetPendentesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<LancamentoFinanceiro>> GetAtrasadosAsync(DateTime hoje, CancellationToken cancellationToken);
    Task<LancamentoFinanceiro?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByClienteAndVencimentoAsync(
        Guid clienteId,
        DateTime dataVencimentoFinanceiro,
        CancellationToken cancellationToken);
    Task AddAsync(LancamentoFinanceiro lancamentoFinanceiro, CancellationToken cancellationToken);
    void Update(LancamentoFinanceiro lancamentoFinanceiro);
    void Remove(LancamentoFinanceiro lancamentoFinanceiro);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
