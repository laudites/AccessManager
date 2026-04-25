using AccessManager.Application.Clientes.Interfaces;
using AccessManager.Application.Common;
using AccessManager.Application.Financeiro.DTOs;
using AccessManager.Application.Financeiro.Interfaces;
using AccessManager.Application.Telas.Interfaces;
using AccessManager.Domain.Entities;
using AccessManager.Domain.Enums;

namespace AccessManager.Application.Financeiro.Services;

public class LancamentoFinanceiroService(
    ILancamentoFinanceiroRepository lancamentoFinanceiroRepository,
    IClienteRepository clienteRepository,
    ITelaClienteRepository telaClienteRepository) : ILancamentoFinanceiroService
{
    private const string NotFoundMessage = "Lancamento financeiro nao encontrado.";

    public async Task<OperationResult<LancamentoFinanceiroDto>> CreateAsync(
        CreateLancamentoFinanceiroDto dto,
        CancellationToken cancellationToken)
    {
        var validationErrors = Validate(
            dto.ClienteId,
            dto.TelaClienteId,
            dto.CompetenciaReferencia,
            dto.Valor,
            dto.DataVencimentoFinanceiro,
            dto.StatusFinanceiro);

        await ValidateRelationsAsync(dto.ClienteId, dto.TelaClienteId, validationErrors, cancellationToken);

        if (validationErrors.Count > 0)
        {
            return OperationResult<LancamentoFinanceiroDto>.Fail(validationErrors.ToArray());
        }

        var statusFinanceiro = dto.StatusFinanceiro ?? StatusFinanceiro.Pendente;
        var lancamento = new LancamentoFinanceiro
        {
            Id = Guid.NewGuid(),
            ClienteId = dto.ClienteId!.Value,
            TelaClienteId = dto.TelaClienteId!.Value,
            CompetenciaReferencia = dto.CompetenciaReferencia!.Value,
            Descricao = dto.Descricao.Trim(),
            Valor = dto.Valor,
            DataVencimentoFinanceiro = dto.DataVencimentoFinanceiro!.Value,
            DataPagamento = statusFinanceiro == StatusFinanceiro.Pago
                ? dto.DataPagamento ?? DateTime.UtcNow
                : dto.DataPagamento,
            StatusFinanceiro = statusFinanceiro,
            Observacao = NormalizeOptionalText(dto.Observacao),
            DataCriacao = DateTime.UtcNow
        };

        await lancamentoFinanceiroRepository.AddAsync(lancamento, cancellationToken);
        await lancamentoFinanceiroRepository.SaveChangesAsync(cancellationToken);

        return OperationResult<LancamentoFinanceiroDto>.Ok(MapToDto(lancamento));
    }

    public async Task<OperationResult<IReadOnlyCollection<LancamentoFinanceiroDto>>> GetAllAsync(
        Guid? clienteId,
        Guid? telaClienteId,
        StatusFinanceiro? statusFinanceiro,
        CancellationToken cancellationToken)
    {
        if (statusFinanceiro is not null && !Enum.IsDefined(statusFinanceiro.Value))
        {
            return OperationResult<IReadOnlyCollection<LancamentoFinanceiroDto>>.Fail("StatusFinanceiro deve ser valido.");
        }

        var lancamentos = await lancamentoFinanceiroRepository.GetAllAsync(
            clienteId,
            telaClienteId,
            statusFinanceiro,
            cancellationToken);

        return OperationResult<IReadOnlyCollection<LancamentoFinanceiroDto>>.Ok(lancamentos.Select(MapToDto).ToArray());
    }

    public async Task<OperationResult<LancamentoFinanceiroDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var lancamento = await lancamentoFinanceiroRepository.GetByIdAsync(id, cancellationToken);
        if (lancamento is null)
        {
            return OperationResult<LancamentoFinanceiroDto>.Fail(NotFoundMessage);
        }

        return OperationResult<LancamentoFinanceiroDto>.Ok(MapToDto(lancamento));
    }

    public async Task<OperationResult<LancamentoFinanceiroDto>> UpdateAsync(
        Guid id,
        UpdateLancamentoFinanceiroDto dto,
        CancellationToken cancellationToken)
    {
        var validationErrors = Validate(
            dto.ClienteId,
            dto.TelaClienteId,
            dto.CompetenciaReferencia,
            dto.Valor,
            dto.DataVencimentoFinanceiro,
            dto.StatusFinanceiro);

        await ValidateRelationsAsync(dto.ClienteId, dto.TelaClienteId, validationErrors, cancellationToken);

        if (validationErrors.Count > 0)
        {
            return OperationResult<LancamentoFinanceiroDto>.Fail(validationErrors.ToArray());
        }

        var lancamento = await lancamentoFinanceiroRepository.GetByIdAsync(id, cancellationToken);
        if (lancamento is null)
        {
            return OperationResult<LancamentoFinanceiroDto>.Fail(NotFoundMessage);
        }

        lancamento.ClienteId = dto.ClienteId!.Value;
        lancamento.TelaClienteId = dto.TelaClienteId!.Value;
        lancamento.CompetenciaReferencia = dto.CompetenciaReferencia!.Value;
        lancamento.Descricao = dto.Descricao.Trim();
        lancamento.Valor = dto.Valor;
        lancamento.DataVencimentoFinanceiro = dto.DataVencimentoFinanceiro!.Value;
        lancamento.StatusFinanceiro = dto.StatusFinanceiro!.Value;
        lancamento.DataPagamento = lancamento.StatusFinanceiro == StatusFinanceiro.Pago
            ? dto.DataPagamento ?? DateTime.UtcNow
            : dto.DataPagamento;
        lancamento.Observacao = NormalizeOptionalText(dto.Observacao);

        lancamentoFinanceiroRepository.Update(lancamento);
        await lancamentoFinanceiroRepository.SaveChangesAsync(cancellationToken);

        return OperationResult<LancamentoFinanceiroDto>.Ok(MapToDto(lancamento));
    }

    public async Task<OperationResult<LancamentoFinanceiroDto>> MarcarComoPagoAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var lancamento = await lancamentoFinanceiroRepository.GetByIdAsync(id, cancellationToken);
        if (lancamento is null)
        {
            return OperationResult<LancamentoFinanceiroDto>.Fail(NotFoundMessage);
        }

        lancamento.StatusFinanceiro = StatusFinanceiro.Pago;
        lancamento.DataPagamento = DateTime.UtcNow;

        lancamentoFinanceiroRepository.Update(lancamento);
        await lancamentoFinanceiroRepository.SaveChangesAsync(cancellationToken);

        return OperationResult<LancamentoFinanceiroDto>.Ok(MapToDto(lancamento));
    }

    public async Task<OperationResult<IReadOnlyCollection<LancamentoFinanceiroDto>>> GetPendentesAsync(
        CancellationToken cancellationToken)
    {
        var lancamentos = await lancamentoFinanceiroRepository.GetPendentesAsync(cancellationToken);

        return OperationResult<IReadOnlyCollection<LancamentoFinanceiroDto>>.Ok(lancamentos.Select(MapToDto).ToArray());
    }

    public async Task<OperationResult<IReadOnlyCollection<LancamentoFinanceiroDto>>> GetAtrasadosAsync(
        CancellationToken cancellationToken)
    {
        var lancamentos = await lancamentoFinanceiroRepository.GetAtrasadosAsync(DateTime.UtcNow.Date, cancellationToken);

        return OperationResult<IReadOnlyCollection<LancamentoFinanceiroDto>>.Ok(lancamentos.Select(MapToDto).ToArray());
    }

    public async Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var lancamento = await lancamentoFinanceiroRepository.GetByIdAsync(id, cancellationToken);
        if (lancamento is null)
        {
            return OperationResult.Fail(NotFoundMessage);
        }

        lancamentoFinanceiroRepository.Remove(lancamento);
        await lancamentoFinanceiroRepository.SaveChangesAsync(cancellationToken);

        return OperationResult.Ok();
    }

    private static List<string> Validate(
        Guid? clienteId,
        Guid? telaClienteId,
        DateTime? competenciaReferencia,
        decimal valor,
        DateTime? dataVencimentoFinanceiro,
        StatusFinanceiro? statusFinanceiro)
    {
        var errors = new List<string>();

        if (clienteId is null || clienteId == Guid.Empty)
        {
            errors.Add("ClienteId e obrigatorio.");
        }

        if (telaClienteId is null || telaClienteId == Guid.Empty)
        {
            errors.Add("TelaClienteId e obrigatorio.");
        }

        if (competenciaReferencia is null || competenciaReferencia == default)
        {
            errors.Add("CompetenciaReferencia e obrigatoria.");
        }

        if (valor <= 0)
        {
            errors.Add("Valor deve ser maior que zero.");
        }

        if (dataVencimentoFinanceiro is null || dataVencimentoFinanceiro == default)
        {
            errors.Add("DataVencimentoFinanceiro e obrigatoria.");
        }

        if (statusFinanceiro is not null && !Enum.IsDefined(statusFinanceiro.Value))
        {
            errors.Add("StatusFinanceiro deve ser valido.");
        }

        return errors;
    }

    private async Task ValidateRelationsAsync(
        Guid? clienteId,
        Guid? telaClienteId,
        List<string> errors,
        CancellationToken cancellationToken)
    {
        var clienteExists = false;

        if (clienteId is not null && clienteId != Guid.Empty)
        {
            var cliente = await clienteRepository.GetByIdAsync(clienteId.Value, cancellationToken);
            clienteExists = cliente is not null;

            if (!clienteExists)
            {
                errors.Add("Cliente nao encontrado.");
            }
        }

        if (telaClienteId is not null && telaClienteId != Guid.Empty)
        {
            var telaCliente = await telaClienteRepository.GetByIdAsync(telaClienteId.Value, cancellationToken);
            if (telaCliente is null)
            {
                errors.Add("TelaCliente nao encontrada.");
                return;
            }

            if (clienteExists && clienteId != telaCliente.ClienteId)
            {
                errors.Add("TelaCliente nao pertence ao Cliente informado.");
            }
        }
    }

    private static LancamentoFinanceiroDto MapToDto(LancamentoFinanceiro lancamento)
    {
        return new LancamentoFinanceiroDto
        {
            Id = lancamento.Id,
            ClienteId = lancamento.ClienteId,
            TelaClienteId = lancamento.TelaClienteId,
            CompetenciaReferencia = lancamento.CompetenciaReferencia,
            Descricao = lancamento.Descricao,
            Valor = lancamento.Valor,
            DataVencimentoFinanceiro = lancamento.DataVencimentoFinanceiro,
            DataPagamento = lancamento.DataPagamento,
            StatusFinanceiro = lancamento.StatusFinanceiro,
            Observacao = lancamento.Observacao,
            DataCriacao = lancamento.DataCriacao
        };
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
