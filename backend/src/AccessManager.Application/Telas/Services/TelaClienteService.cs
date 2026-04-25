using AccessManager.Application.Clientes.Interfaces;
using AccessManager.Application.Common;
using AccessManager.Application.Servidores.Interfaces;
using AccessManager.Application.Telas.DTOs;
using AccessManager.Application.Telas.Interfaces;
using AccessManager.Domain.Entities;
using AccessManager.Domain.Enums;

namespace AccessManager.Application.Telas.Services;

public class TelaClienteService(
    ITelaClienteRepository telaClienteRepository,
    IRenovacaoTelaHistoricoRepository renovacaoTelaHistoricoRepository,
    IClienteRepository clienteRepository,
    IServidorRepository servidorRepository) : ITelaClienteService
{
    private const string NotFoundMessage = "Tela nao encontrada.";

    public async Task<OperationResult<TelaClienteDto>> CreateAsync(
        CreateTelaClienteDto dto,
        CancellationToken cancellationToken)
    {
        var validationErrors = Validate(
            dto.ClienteId,
            dto.ServidorId,
            dto.NomeIdentificacao,
            dto.UsuarioTela,
            dto.SenhaTela,
            dto.ValorAcordado,
            dto.DataVencimentoTecnico,
            dto.Status);

        await ValidateRelationsAsync(dto.ClienteId, dto.ServidorId, validationErrors, cancellationToken);

        if (validationErrors.Count > 0)
        {
            return OperationResult<TelaClienteDto>.Fail(validationErrors.ToArray());
        }

        var telaCliente = new TelaCliente
        {
            Id = Guid.NewGuid(),
            ClienteId = dto.ClienteId!.Value,
            NomeIdentificacao = dto.NomeIdentificacao.Trim(),
            ServidorId = dto.ServidorId!.Value,
            UsuarioTela = dto.UsuarioTela.Trim(),
            SenhaTela = dto.SenhaTela.Trim(),
            ValorAcordado = dto.ValorAcordado,
            DataInicio = DateTime.UtcNow,
            DataVencimentoTecnico = dto.DataVencimentoTecnico!.Value,
            Status = dto.Status,
            MarcaTv = dto.MarcaTv.Trim(),
            AppUtilizado = dto.AppUtilizado.Trim(),
            MacOuIdApp = NormalizeOptionalText(dto.MacOuIdApp),
            ChaveSecundaria = NormalizeOptionalText(dto.ChaveSecundaria),
            Observacao = NormalizeOptionalText(dto.Observacao),
            Ativo = true
        };

        await telaClienteRepository.AddAsync(telaCliente, cancellationToken);
        await telaClienteRepository.SaveChangesAsync(cancellationToken);

        return OperationResult<TelaClienteDto>.Ok(MapToDto(telaCliente));
    }

    public async Task<OperationResult<IReadOnlyCollection<TelaClienteDto>>> GetAllAsync(
        Guid? clienteId,
        Guid? servidorId,
        CancellationToken cancellationToken)
    {
        var telas = await telaClienteRepository.GetAllAsync(clienteId, servidorId, cancellationToken);
        var dtos = telas.Select(MapToDto).ToArray();

        return OperationResult<IReadOnlyCollection<TelaClienteDto>>.Ok(dtos);
    }

    public async Task<OperationResult<TelaClienteDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var telaCliente = await telaClienteRepository.GetByIdAsync(id, cancellationToken);
        if (telaCliente is null)
        {
            return OperationResult<TelaClienteDto>.Fail(NotFoundMessage);
        }

        return OperationResult<TelaClienteDto>.Ok(MapToDto(telaCliente));
    }

    public async Task<OperationResult<TelaClienteDto>> UpdateAsync(
        Guid id,
        UpdateTelaClienteDto dto,
        CancellationToken cancellationToken)
    {
        var validationErrors = Validate(
            dto.ClienteId,
            dto.ServidorId,
            dto.NomeIdentificacao,
            dto.UsuarioTela,
            dto.SenhaTela,
            dto.ValorAcordado,
            dto.DataVencimentoTecnico,
            dto.Status);

        await ValidateRelationsAsync(dto.ClienteId, dto.ServidorId, validationErrors, cancellationToken);

        if (validationErrors.Count > 0)
        {
            return OperationResult<TelaClienteDto>.Fail(validationErrors.ToArray());
        }

        var telaCliente = await telaClienteRepository.GetByIdAsync(id, cancellationToken);
        if (telaCliente is null)
        {
            return OperationResult<TelaClienteDto>.Fail(NotFoundMessage);
        }

        telaCliente.ClienteId = dto.ClienteId!.Value;
        telaCliente.NomeIdentificacao = dto.NomeIdentificacao.Trim();
        telaCliente.ServidorId = dto.ServidorId!.Value;
        telaCliente.UsuarioTela = dto.UsuarioTela.Trim();
        telaCliente.SenhaTela = dto.SenhaTela.Trim();
        telaCliente.ValorAcordado = dto.ValorAcordado;
        telaCliente.DataVencimentoTecnico = dto.DataVencimentoTecnico!.Value;
        telaCliente.Status = dto.Status;
        telaCliente.MarcaTv = dto.MarcaTv.Trim();
        telaCliente.AppUtilizado = dto.AppUtilizado.Trim();
        telaCliente.MacOuIdApp = NormalizeOptionalText(dto.MacOuIdApp);
        telaCliente.ChaveSecundaria = NormalizeOptionalText(dto.ChaveSecundaria);
        telaCliente.Observacao = NormalizeOptionalText(dto.Observacao);
        telaCliente.Ativo = dto.Ativo;

        telaClienteRepository.Update(telaCliente);
        await telaClienteRepository.SaveChangesAsync(cancellationToken);

        return OperationResult<TelaClienteDto>.Ok(MapToDto(telaCliente));
    }

    public async Task<OperationResult<TelaClienteDto>> RenovarAsync(
        Guid id,
        RenovarTelaRequest request,
        CancellationToken cancellationToken)
    {
        var validationErrors = ValidateRenovacao(request);
        if (validationErrors.Count > 0)
        {
            return OperationResult<TelaClienteDto>.Fail(validationErrors.ToArray());
        }

        var telaCliente = await telaClienteRepository.GetByIdAsync(id, cancellationToken);
        if (telaCliente is null)
        {
            return OperationResult<TelaClienteDto>.Fail(NotFoundMessage);
        }

        var dataVencimentoAnterior = telaCliente.DataVencimentoTecnico;
        var valorAcordadoAnterior = telaCliente.ValorAcordado;
        var valorAcordadoNovo = request.ValorAcordadoNovo ?? telaCliente.ValorAcordado;

        telaCliente.DataVencimentoTecnico = request.NovaDataVencimentoTecnico!.Value;
        telaCliente.ValorAcordado = valorAcordadoNovo;

        var historico = new RenovacaoTelaHistorico
        {
            Id = Guid.NewGuid(),
            TelaClienteId = telaCliente.Id,
            ClienteId = telaCliente.ClienteId,
            ServidorAnteriorId = null,
            ServidorNovoId = null,
            DataVencimentoTecnicoAnterior = dataVencimentoAnterior,
            NovaDataVencimentoTecnico = telaCliente.DataVencimentoTecnico,
            ValorAcordadoAnterior = valorAcordadoAnterior,
            ValorAcordadoNovo = valorAcordadoNovo,
            Observacao = NormalizeOptionalText(request.Observacao),
            DataCriacao = DateTime.UtcNow
        };

        telaClienteRepository.Update(telaCliente);
        await renovacaoTelaHistoricoRepository.AddAsync(historico, cancellationToken);
        await renovacaoTelaHistoricoRepository.SaveChangesAsync(cancellationToken);

        return OperationResult<TelaClienteDto>.Ok(MapToDto(telaCliente));
    }

    public async Task<OperationResult<TelaClienteDto>> TrocarServidorAsync(
        Guid id,
        TrocarServidorRequest request,
        CancellationToken cancellationToken)
    {
        var validationErrors = ValidateTrocaServidor(request);
        if (validationErrors.Count > 0)
        {
            return OperationResult<TelaClienteDto>.Fail(validationErrors.ToArray());
        }

        var telaCliente = await telaClienteRepository.GetByIdAsync(id, cancellationToken);
        if (telaCliente is null)
        {
            return OperationResult<TelaClienteDto>.Fail(NotFoundMessage);
        }

        var servidorNovo = await servidorRepository.GetByIdAsync(request.ServidorNovoId!.Value, cancellationToken);
        if (servidorNovo is null)
        {
            return OperationResult<TelaClienteDto>.Fail("Servidor novo nao encontrado.");
        }

        var servidorAnteriorId = telaCliente.ServidorId;

        telaCliente.ServidorId = request.ServidorNovoId.Value;

        var historico = new RenovacaoTelaHistorico
        {
            Id = Guid.NewGuid(),
            TelaClienteId = telaCliente.Id,
            ClienteId = telaCliente.ClienteId,
            ServidorAnteriorId = servidorAnteriorId,
            ServidorNovoId = telaCliente.ServidorId,
            DataVencimentoTecnicoAnterior = telaCliente.DataVencimentoTecnico,
            NovaDataVencimentoTecnico = telaCliente.DataVencimentoTecnico,
            ValorAcordadoAnterior = telaCliente.ValorAcordado,
            ValorAcordadoNovo = telaCliente.ValorAcordado,
            Observacao = NormalizeOptionalText(request.Observacao),
            DataCriacao = DateTime.UtcNow
        };

        telaClienteRepository.Update(telaCliente);
        await renovacaoTelaHistoricoRepository.AddAsync(historico, cancellationToken);
        await renovacaoTelaHistoricoRepository.SaveChangesAsync(cancellationToken);

        return OperationResult<TelaClienteDto>.Ok(MapToDto(telaCliente));
    }

    public async Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var telaCliente = await telaClienteRepository.GetByIdAsync(id, cancellationToken);
        if (telaCliente is null)
        {
            return OperationResult.Fail(NotFoundMessage);
        }

        telaClienteRepository.Remove(telaCliente);
        await telaClienteRepository.SaveChangesAsync(cancellationToken);

        return OperationResult.Ok();
    }

    private static List<string> Validate(
        Guid? clienteId,
        Guid? servidorId,
        string nomeIdentificacao,
        string usuarioTela,
        string senhaTela,
        decimal valorAcordado,
        DateTime? dataVencimentoTecnico,
        StatusTela status)
    {
        var errors = new List<string>();

        if (clienteId is null || clienteId == Guid.Empty)
        {
            errors.Add("ClienteId e obrigatorio.");
        }

        if (servidorId is null || servidorId == Guid.Empty)
        {
            errors.Add("ServidorId e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(nomeIdentificacao))
        {
            errors.Add("NomeIdentificacao e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(usuarioTela))
        {
            errors.Add("UsuarioTela e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(senhaTela))
        {
            errors.Add("SenhaTela e obrigatoria.");
        }

        if (valorAcordado < 0)
        {
            errors.Add("ValorAcordado deve ser maior ou igual a zero.");
        }

        if (dataVencimentoTecnico is null || dataVencimentoTecnico == default)
        {
            errors.Add("DataVencimentoTecnico e obrigatoria.");
        }

        if (!Enum.IsDefined(status))
        {
            errors.Add("Status deve ser valido.");
        }

        return errors;
    }

    private static List<string> ValidateRenovacao(RenovarTelaRequest request)
    {
        var errors = new List<string>();

        if (request.NovaDataVencimentoTecnico is null || request.NovaDataVencimentoTecnico == default)
        {
            errors.Add("NovaDataVencimentoTecnico e obrigatoria.");
        }

        if (request.ValorAcordadoNovo is < 0)
        {
            errors.Add("ValorAcordadoNovo deve ser maior ou igual a zero.");
        }

        return errors;
    }

    private static List<string> ValidateTrocaServidor(TrocarServidorRequest request)
    {
        var errors = new List<string>();

        if (request.ServidorNovoId is null || request.ServidorNovoId == Guid.Empty)
        {
            errors.Add("ServidorNovoId e obrigatorio.");
        }

        return errors;
    }

    private async Task ValidateRelationsAsync(
        Guid? clienteId,
        Guid? servidorId,
        List<string> errors,
        CancellationToken cancellationToken)
    {
        if (clienteId is not null && clienteId != Guid.Empty)
        {
            var cliente = await clienteRepository.GetByIdAsync(clienteId.Value, cancellationToken);
            if (cliente is null)
            {
                errors.Add("Cliente nao encontrado.");
            }
        }

        if (servidorId is not null && servidorId != Guid.Empty)
        {
            var servidor = await servidorRepository.GetByIdAsync(servidorId.Value, cancellationToken);
            if (servidor is null)
            {
                errors.Add("Servidor nao encontrado.");
            }
        }
    }

    private static TelaClienteDto MapToDto(TelaCliente telaCliente)
    {
        return new TelaClienteDto
        {
            Id = telaCliente.Id,
            ClienteId = telaCliente.ClienteId,
            NomeIdentificacao = telaCliente.NomeIdentificacao,
            ServidorId = telaCliente.ServidorId,
            UsuarioTela = telaCliente.UsuarioTela,
            SenhaTela = telaCliente.SenhaTela,
            ValorAcordado = telaCliente.ValorAcordado,
            DataInicio = telaCliente.DataInicio,
            DataVencimentoTecnico = telaCliente.DataVencimentoTecnico,
            Status = telaCliente.Status,
            MarcaTv = telaCliente.MarcaTv,
            AppUtilizado = telaCliente.AppUtilizado,
            MacOuIdApp = telaCliente.MacOuIdApp,
            ChaveSecundaria = telaCliente.ChaveSecundaria,
            Observacao = telaCliente.Observacao,
            Ativo = telaCliente.Ativo
        };
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
