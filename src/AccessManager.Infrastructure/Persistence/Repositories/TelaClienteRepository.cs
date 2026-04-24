using AccessManager.Application.Telas.Interfaces;
using AccessManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccessManager.Infrastructure.Persistence.Repositories;

public class TelaClienteRepository(AccessManagerDbContext dbContext) : ITelaClienteRepository
{
    public async Task<IReadOnlyCollection<TelaCliente>> GetAllAsync(
        Guid? clienteId,
        Guid? servidorId,
        CancellationToken cancellationToken)
    {
        var query = dbContext.TelasClientes.AsNoTracking();

        if (clienteId is not null)
        {
            query = query.Where(tela => tela.ClienteId == clienteId.Value);
        }

        if (servidorId is not null)
        {
            query = query.Where(tela => tela.ServidorId == servidorId.Value);
        }

        return await query
            .OrderBy(tela => tela.NomeIdentificacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<TelaCliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.TelasClientes
            .FirstOrDefaultAsync(tela => tela.Id == id, cancellationToken);
    }

    public async Task AddAsync(TelaCliente telaCliente, CancellationToken cancellationToken)
    {
        await dbContext.TelasClientes.AddAsync(telaCliente, cancellationToken);
    }

    public void Update(TelaCliente telaCliente)
    {
        dbContext.TelasClientes.Update(telaCliente);
    }

    public void Remove(TelaCliente telaCliente)
    {
        dbContext.TelasClientes.Remove(telaCliente);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
