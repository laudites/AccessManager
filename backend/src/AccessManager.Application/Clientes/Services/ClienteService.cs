using AccessManager.Application.Clientes.DTOs;
using AccessManager.Application.Clientes.Interfaces;
using AccessManager.Application.Common;
using AccessManager.Domain.Entities;
using AccessManager.Domain.Enums;

namespace AccessManager.Application.Clientes.Services;

public class ClienteService(IClienteRepository clienteRepository) : IClienteService
{
    public async Task<OperationResult<ClienteDto>> CreateAsync(CreateClienteDto dto, CancellationToken cancellationToken)
    {
        var validationErrors = Validate(dto.Nome, dto.DiaPagamentoPreferido);
        if (validationErrors.Count > 0)
        {
            return OperationResult<ClienteDto>.Fail(validationErrors.ToArray());
        }

        var cliente = new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = dto.Nome.Trim(),
            Telefone = dto.Telefone.Trim(),
            Observacao = NormalizeOptionalText(dto.Observacao),
            DiaPagamentoPreferido = dto.DiaPagamentoPreferido,
            DataCadastro = DateTime.UtcNow,
            Ativo = true
        };

        await clienteRepository.AddAsync(cliente, cancellationToken);
        await clienteRepository.SaveChangesAsync(cancellationToken);

        return OperationResult<ClienteDto>.Ok(MapToDto(cliente));
    }

    public async Task<OperationResult<IReadOnlyCollection<ClienteDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var clientes = await clienteRepository.GetAllAsync(cancellationToken);
        var dtos = clientes.Select(MapToDto).ToArray();

        return OperationResult<IReadOnlyCollection<ClienteDto>>.Ok(dtos);
    }

    public async Task<OperationResult<ClienteDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var cliente = await clienteRepository.GetByIdAsync(id, cancellationToken);
        if (cliente is null)
        {
            return OperationResult<ClienteDto>.Fail("Cliente não encontrado.");
        }

        return OperationResult<ClienteDto>.Ok(MapToDto(cliente));
    }

    public async Task<OperationResult<ClienteDto>> UpdateAsync(Guid id, UpdateClienteDto dto, CancellationToken cancellationToken)
    {
        var validationErrors = Validate(dto.Nome, dto.DiaPagamentoPreferido);
        if (validationErrors.Count > 0)
        {
            return OperationResult<ClienteDto>.Fail(validationErrors.ToArray());
        }

        var cliente = await clienteRepository.GetByIdAsync(id, cancellationToken);
        if (cliente is null)
        {
            return OperationResult<ClienteDto>.Fail("Cliente não encontrado.");
        }

        cliente.Nome = dto.Nome.Trim();
        cliente.Telefone = dto.Telefone.Trim();
        cliente.Observacao = NormalizeOptionalText(dto.Observacao);
        cliente.DiaPagamentoPreferido = dto.DiaPagamentoPreferido;
        cliente.Ativo = dto.Ativo;

        clienteRepository.Update(cliente);
        await clienteRepository.SaveChangesAsync(cancellationToken);

        return OperationResult<ClienteDto>.Ok(MapToDto(cliente));
    }

    public async Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var cliente = await clienteRepository.GetByIdAsync(id, cancellationToken);
        if (cliente is null)
        {
            return OperationResult.Fail("Cliente não encontrado.");
        }

        clienteRepository.Remove(cliente);
        await clienteRepository.SaveChangesAsync(cancellationToken);

        return OperationResult.Ok();
    }

    private static List<string> Validate(string nome, int? diaPagamentoPreferido)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(nome))
        {
            errors.Add("Nome é obrigatório.");
        }

        if (diaPagamentoPreferido is < 1 or > 31)
        {
            errors.Add("DiaPagamentoPreferido deve ser nulo ou estar entre 1 e 31.");
        }

        return errors;
    }

    private static ClienteDto MapToDto(Cliente cliente)
    {
        var hoje = DateTime.UtcNow.Date;
        var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);
        var inicioProximoMes = inicioMes.AddMonths(1);

        return new ClienteDto
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            Telefone = cliente.Telefone,
            Observacao = cliente.Observacao,
            DiaPagamentoPreferido = cliente.DiaPagamentoPreferido,
            DataCadastro = cliente.DataCadastro,
            Ativo = cliente.Ativo,
            QuantidadeTelas = cliente.Telas.Count(tela => tela.Ativo),
            ValorTotalTelas = cliente.Telas
                .Where(tela => tela.Ativo)
                .Sum(tela => tela.ValorAcordado),
            StatusFinanceiroCliente = CalculateStatusFinanceiroCliente(cliente, inicioMes, inicioProximoMes, hoje)
        };
    }

    private static string CalculateStatusFinanceiroCliente(
        Cliente cliente,
        DateTime inicioMes,
        DateTime inicioProximoMes,
        DateTime hoje)
    {
        var lancamentosPeriodo = cliente.LancamentosFinanceiros
            .Where(lancamento =>
                lancamento.DataVencimentoFinanceiro.Date >= inicioMes &&
                lancamento.DataVencimentoFinanceiro.Date < inicioProximoMes)
            .ToArray();

        if (lancamentosPeriodo.Length == 0)
        {
            return "Sem lançamento";
        }

        if (lancamentosPeriodo.Any(lancamento => IsAtrasado(lancamento, hoje)))
        {
            return "Atrasado";
        }

        if (lancamentosPeriodo.Any(lancamento => lancamento.StatusFinanceiro == StatusFinanceiro.Pendente))
        {
            return "Pendente";
        }

        if (lancamentosPeriodo.Any(lancamento => lancamento.StatusFinanceiro == StatusFinanceiro.Pago))
        {
            return "Pago";
        }

        return "Sem lançamento";
    }

    private static bool IsAtrasado(LancamentoFinanceiro lancamento, DateTime hoje)
    {
        return lancamento.StatusFinanceiro == StatusFinanceiro.Atrasado ||
            (lancamento.DataPagamento is null &&
                lancamento.StatusFinanceiro != StatusFinanceiro.Pago &&
                lancamento.StatusFinanceiro != StatusFinanceiro.Cancelado &&
                lancamento.DataVencimentoFinanceiro.Date < hoje);
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
