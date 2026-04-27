using AccessManager.Application.Financeiro.Interfaces;
using AccessManager.Domain.Entities;
using AccessManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AccessManager.Infrastructure.Persistence.Repositories;

public class LancamentoFinanceiroRepository(AccessManagerDbContext dbContext) : ILancamentoFinanceiroRepository
{
    public async Task<IReadOnlyCollection<LancamentoFinanceiro>> GetAllAsync(
        Guid? clienteId,
        Guid? telaClienteId,
        StatusFinanceiro? statusFinanceiro,
        CancellationToken cancellationToken)
    {
        IQueryable<LancamentoFinanceiro> query = dbContext.LancamentosFinanceiros
            .AsNoTracking()
            .Include(lancamento => lancamento.Cliente)
            .Include(lancamento => lancamento.TelaCliente);

        if (clienteId is not null)
        {
            query = query.Where(lancamento => lancamento.ClienteId == clienteId.Value);
        }

        if (telaClienteId is not null)
        {
            query = query.Where(lancamento => lancamento.TelaClienteId == telaClienteId.Value);
        }

        if (statusFinanceiro is not null)
        {
            query = query.Where(lancamento => lancamento.StatusFinanceiro == statusFinanceiro.Value);
        }

        return await query
            .OrderBy(lancamento => lancamento.DataVencimentoFinanceiro)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<LancamentoFinanceiro>> GetPendentesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.LancamentosFinanceiros
            .AsNoTracking()
            .Include(lancamento => lancamento.Cliente)
            .Include(lancamento => lancamento.TelaCliente)
            .Where(lancamento => lancamento.StatusFinanceiro == StatusFinanceiro.Pendente)
            .OrderBy(lancamento => lancamento.DataVencimentoFinanceiro)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<LancamentoFinanceiro>> GetAtrasadosAsync(
        DateTime hoje,
        CancellationToken cancellationToken)
    {
        return await dbContext.LancamentosFinanceiros
            .AsNoTracking()
            .Include(lancamento => lancamento.Cliente)
            .Include(lancamento => lancamento.TelaCliente)
            .Where(lancamento =>
                lancamento.StatusFinanceiro == StatusFinanceiro.Atrasado ||
                (lancamento.StatusFinanceiro == StatusFinanceiro.Pendente &&
                    lancamento.DataVencimentoFinanceiro.Date < hoje))
            .OrderBy(lancamento => lancamento.DataVencimentoFinanceiro)
            .ToListAsync(cancellationToken);
    }

    public async Task<LancamentoFinanceiro?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.LancamentosFinanceiros
            .Include(lancamento => lancamento.Cliente)
            .Include(lancamento => lancamento.TelaCliente)
            .FirstOrDefaultAsync(lancamento => lancamento.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByClienteAndVencimentoAsync(
        Guid clienteId,
        DateTime dataVencimentoFinanceiro,
        CancellationToken cancellationToken)
    {
        return await dbContext.LancamentosFinanceiros.AnyAsync(
            lancamento => lancamento.ClienteId == clienteId &&
                lancamento.DataVencimentoFinanceiro.Date == dataVencimentoFinanceiro.Date &&
                lancamento.StatusFinanceiro != StatusFinanceiro.Cancelado,
            cancellationToken);
    }

    public async Task AddAsync(LancamentoFinanceiro lancamentoFinanceiro, CancellationToken cancellationToken)
    {
        await dbContext.LancamentosFinanceiros.AddAsync(lancamentoFinanceiro, cancellationToken);
    }

    public void Update(LancamentoFinanceiro lancamentoFinanceiro)
    {
        dbContext.LancamentosFinanceiros.Update(lancamentoFinanceiro);
    }

    public void Remove(LancamentoFinanceiro lancamentoFinanceiro)
    {
        dbContext.LancamentosFinanceiros.Remove(lancamentoFinanceiro);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
