using AccessManager.Application.Clientes.Interfaces;
using AccessManager.Application.Common;
using AccessManager.Application.Financeiro.DTOs;
using AccessManager.Application.Financeiro.Interfaces;
using AccessManager.Domain.Entities;
using AccessManager.Domain.Enums;

namespace AccessManager.Application.Financeiro.Services;

public class LancamentoFinanceiroService(
    ILancamentoFinanceiroRepository lancamentoFinanceiroRepository,
    IClienteRepository clienteRepository) : ILancamentoFinanceiroService
{
    private const string NotFoundMessage = "Lancamento financeiro nao encontrado.";

    public async Task<OperationResult<LancamentoFinanceiroDto>> CreateAsync(
        CreateLancamentoFinanceiroDto dto,
        CancellationToken cancellationToken)
    {
        var validationErrors = Validate(dto.ClienteId, dto.DataVencimentoFinanceiro, dto.StatusFinanceiro);
        var cliente = await ValidateClienteAsync(dto.ClienteId, validationErrors, cancellationToken);

        if (validationErrors.Count > 0)
        {
            return OperationResult<LancamentoFinanceiroDto>.Fail(validationErrors.ToArray());
        }

        var valorAgrupado = CalculateValorTelasAtivas(cliente!);
        if (valorAgrupado <= 0)
        {
            return OperationResult<LancamentoFinanceiroDto>.Fail("Cliente nao possui telas ativas com valor acordado.");
        }

        var dataVencimentoFinanceiro = dto.DataVencimentoFinanceiro!.Value.Date;
        var statusFinanceiro = dto.StatusFinanceiro ?? StatusFinanceiro.Pendente;
        var lancamento = new LancamentoFinanceiro
        {
            Id = Guid.NewGuid(),
            ClienteId = dto.ClienteId!.Value,
            TelaClienteId = null,
            CompetenciaReferencia = CalculateCompetenciaReferencia(dataVencimentoFinanceiro),
            Descricao = string.IsNullOrWhiteSpace(dto.Descricao)
                ? $"Mensalidade {cliente!.Nome}"
                : dto.Descricao.Trim(),
            Valor = valorAgrupado,
            DataVencimentoFinanceiro = dataVencimentoFinanceiro,
            DataPagamento = statusFinanceiro == StatusFinanceiro.Pago
                ? dto.DataPagamento ?? DateTime.UtcNow
                : dto.DataPagamento,
            StatusFinanceiro = statusFinanceiro,
            Observacao = NormalizeOptionalText(dto.Observacao),
            DataCriacao = DateTime.UtcNow
        };

        await lancamentoFinanceiroRepository.AddAsync(lancamento, cancellationToken);
        await lancamentoFinanceiroRepository.SaveChangesAsync(cancellationToken);

        return OperationResult<LancamentoFinanceiroDto>.Ok(MapToDto(lancamento, cliente));
    }

    public async Task<OperationResult<IReadOnlyCollection<LancamentoFinanceiroDto>>> GetAllAsync(
        Guid? clienteId,
        Guid? telaClienteId,
        StatusFinanceiro? statusFinanceiro,
        int? mes,
        int? ano,
        CancellationToken cancellationToken)
    {
        if (statusFinanceiro is not null && !Enum.IsDefined(statusFinanceiro.Value))
        {
            return OperationResult<IReadOnlyCollection<LancamentoFinanceiroDto>>.Fail("StatusFinanceiro deve ser valido.");
        }

        var filtroPeriodoErrors = ValidateFiltroPeriodo(mes, ano);
        if (filtroPeriodoErrors.Count > 0)
        {
            return OperationResult<IReadOnlyCollection<LancamentoFinanceiroDto>>.Fail(filtroPeriodoErrors.ToArray());
        }

        var lancamentos = await lancamentoFinanceiroRepository.GetAllAsync(
            clienteId,
            telaClienteId,
            statusFinanceiro,
            mes,
            ano,
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
        var validationErrors = Validate(dto.ClienteId, dto.DataVencimentoFinanceiro, dto.StatusFinanceiro);
        var cliente = await ValidateClienteAsync(dto.ClienteId, validationErrors, cancellationToken);

        if (validationErrors.Count > 0)
        {
            return OperationResult<LancamentoFinanceiroDto>.Fail(validationErrors.ToArray());
        }

        var lancamento = await lancamentoFinanceiroRepository.GetByIdAsync(id, cancellationToken);
        if (lancamento is null)
        {
            return OperationResult<LancamentoFinanceiroDto>.Fail(NotFoundMessage);
        }

        var valorAgrupado = CalculateValorTelasAtivas(cliente!);
        if (valorAgrupado <= 0)
        {
            return OperationResult<LancamentoFinanceiroDto>.Fail("Cliente nao possui telas ativas com valor acordado.");
        }

        var dataVencimentoFinanceiro = dto.DataVencimentoFinanceiro!.Value.Date;
        var statusFinanceiro = dto.StatusFinanceiro ?? lancamento.StatusFinanceiro;

        lancamento.ClienteId = dto.ClienteId!.Value;
        lancamento.TelaClienteId = null;
        lancamento.CompetenciaReferencia = CalculateCompetenciaReferencia(dataVencimentoFinanceiro);
        lancamento.Descricao = string.IsNullOrWhiteSpace(dto.Descricao)
            ? $"Mensalidade {cliente!.Nome}"
            : dto.Descricao.Trim();
        lancamento.Valor = valorAgrupado;
        lancamento.DataVencimentoFinanceiro = dataVencimentoFinanceiro;
        lancamento.StatusFinanceiro = statusFinanceiro;
        lancamento.DataPagamento = lancamento.StatusFinanceiro == StatusFinanceiro.Pago
            ? dto.DataPagamento ?? DateTime.UtcNow
            : dto.DataPagamento;
        lancamento.Observacao = NormalizeOptionalText(dto.Observacao);

        lancamentoFinanceiroRepository.Update(lancamento);
        await lancamentoFinanceiroRepository.SaveChangesAsync(cancellationToken);

        return OperationResult<LancamentoFinanceiroDto>.Ok(MapToDto(lancamento, cliente));
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

    public async Task<OperationResult<IReadOnlyCollection<LancamentoFinanceiroDto>>> GerarPendentesAsync(
        GerarLancamentosFinanceirosRequest request,
        CancellationToken cancellationToken)
    {
        var dataReferencia = (request.DataReferencia ?? DateTime.UtcNow).Date;
        var dataVencimentoAlvo = dataReferencia.AddDays(5);
        var clientes = await clienteRepository.GetAllAsync(cancellationToken);
        var gerados = new List<LancamentoFinanceiro>();

        foreach (var cliente in clientes.Where(cliente => cliente.Ativo && cliente.DiaPagamentoPreferido is not null))
        {
            if (cliente.DiaPagamentoPreferido != dataVencimentoAlvo.Day)
            {
                continue;
            }

            var valorAgrupado = CalculateValorTelasAtivas(cliente);
            if (valorAgrupado <= 0)
            {
                continue;
            }

            var exists = await lancamentoFinanceiroRepository.ExistsByClienteAndVencimentoAsync(
                cliente.Id,
                dataVencimentoAlvo,
                cancellationToken);

            if (exists)
            {
                continue;
            }

            var lancamento = new LancamentoFinanceiro
            {
                Id = Guid.NewGuid(),
                ClienteId = cliente.Id,
                TelaClienteId = null,
                CompetenciaReferencia = CalculateCompetenciaReferencia(dataVencimentoAlvo),
                Descricao = $"Mensalidade {cliente.Nome}",
                Valor = valorAgrupado,
                DataVencimentoFinanceiro = dataVencimentoAlvo,
                StatusFinanceiro = StatusFinanceiro.Pendente,
                Observacao = "Gerado automaticamente por regra de vencimento.",
                DataCriacao = DateTime.UtcNow
            };

            await lancamentoFinanceiroRepository.AddAsync(lancamento, cancellationToken);
            gerados.Add(lancamento);
        }

        if (gerados.Count > 0)
        {
            await lancamentoFinanceiroRepository.SaveChangesAsync(cancellationToken);
        }

        return OperationResult<IReadOnlyCollection<LancamentoFinanceiroDto>>.Ok(
            gerados.Select(lancamento =>
            {
                var cliente = clientes.First(item => item.Id == lancamento.ClienteId);
                return MapToDto(lancamento, cliente);
            }).ToArray());
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
        DateTime? dataVencimentoFinanceiro,
        StatusFinanceiro? statusFinanceiro)
    {
        var errors = new List<string>();

        if (clienteId is null || clienteId == Guid.Empty)
        {
            errors.Add("ClienteId e obrigatorio.");
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

    private static List<string> ValidateFiltroPeriodo(int? mes, int? ano)
    {
        var errors = new List<string>();

        if (mes is not null and (< 1 or > 12))
        {
            errors.Add("Mes deve estar entre 1 e 12.");
        }

        if (ano is not null and < 1)
        {
            errors.Add("Ano deve ser maior que zero.");
        }

        return errors;
    }

    private async Task<Cliente?> ValidateClienteAsync(
        Guid? clienteId,
        List<string> errors,
        CancellationToken cancellationToken)
    {
        if (clienteId is null || clienteId == Guid.Empty)
        {
            return null;
        }

        var cliente = await clienteRepository.GetByIdAsync(clienteId.Value, cancellationToken);
        if (cliente is null)
        {
            errors.Add("Cliente nao encontrado.");
        }

        return cliente;
    }

    private static decimal CalculateValorTelasAtivas(Cliente cliente)
    {
        return cliente.Telas
            .Where(tela => tela.Ativo)
            .Sum(tela => tela.ValorAcordado);
    }

    private static DateTime CalculateCompetenciaReferencia(DateTime dataVencimentoFinanceiro)
    {
        return new DateTime(dataVencimentoFinanceiro.Year, dataVencimentoFinanceiro.Month, 1);
    }

    private static LancamentoFinanceiroDto MapToDto(LancamentoFinanceiro lancamento)
    {
        return MapToDto(lancamento, lancamento.Cliente);
    }

    private static LancamentoFinanceiroDto MapToDto(LancamentoFinanceiro lancamento, Cliente? cliente)
    {
        return new LancamentoFinanceiroDto
        {
            Id = lancamento.Id,
            ClienteId = lancamento.ClienteId,
            ClienteNome = cliente?.Nome ?? lancamento.Cliente?.Nome ?? string.Empty,
            TelaClienteId = lancamento.TelaClienteId,
            TelaClienteNome = lancamento.TelaCliente?.NomeIdentificacao,
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
