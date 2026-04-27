using AccessManager.Application.Common;
using AccessManager.Application.Servidores.DTOs;
using AccessManager.Application.Servidores.Interfaces;
using AccessManager.Domain.Entities;
using AccessManager.Domain.Enums;

namespace AccessManager.Application.Servidores.Services;

public class ServidorService(IServidorRepository servidorRepository) : IServidorService
{
    private const string NotFoundMessage = "Servidor nao encontrado.";

    public async Task<OperationResult<ServidorDto>> CreateAsync(CreateServidorDto dto, CancellationToken cancellationToken)
    {
        var validationErrors = Validate(dto.Nome, dto.QuantidadeCreditos, dto.ValorCustoCredito, dto.Status);
        if (validationErrors.Count > 0)
        {
            return OperationResult<ServidorDto>.Fail(validationErrors.ToArray());
        }

        var servidor = new Servidor
        {
            Id = Guid.NewGuid(),
            Nome = dto.Nome.Trim(),
            Descricao = NormalizeOptionalText(dto.Descricao),
            Status = dto.Status,
            QuantidadeCreditos = dto.QuantidadeCreditos,
            ValorCustoCredito = dto.ValorCustoCredito,
            UsuarioPainel = dto.UsuarioPainel.Trim(),
            SenhaPainel = dto.SenhaPainel.Trim(),
            Observacao = NormalizeOptionalText(dto.Observacao),
            Ativo = true
        };

        await servidorRepository.AddAsync(servidor, cancellationToken);
        await servidorRepository.SaveChangesAsync(cancellationToken);

        return OperationResult<ServidorDto>.Ok(MapToDto(servidor));
    }

    public async Task<OperationResult<IReadOnlyCollection<ServidorDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var servidores = await servidorRepository.GetAllAsync(cancellationToken);
        var dtos = servidores.Select(MapToDto).ToArray();

        return OperationResult<IReadOnlyCollection<ServidorDto>>.Ok(dtos);
    }

    public async Task<OperationResult<ServidorDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var servidor = await servidorRepository.GetByIdAsync(id, cancellationToken);
        if (servidor is null)
        {
            return OperationResult<ServidorDto>.Fail(NotFoundMessage);
        }

        return OperationResult<ServidorDto>.Ok(MapToDto(servidor));
    }

    public async Task<OperationResult<ServidorDto>> UpdateAsync(Guid id, UpdateServidorDto dto, CancellationToken cancellationToken)
    {
        var validationErrors = Validate(dto.Nome, dto.QuantidadeCreditos, dto.ValorCustoCredito, dto.Status);
        if (validationErrors.Count > 0)
        {
            return OperationResult<ServidorDto>.Fail(validationErrors.ToArray());
        }

        var servidor = await servidorRepository.GetByIdAsync(id, cancellationToken);
        if (servidor is null)
        {
            return OperationResult<ServidorDto>.Fail(NotFoundMessage);
        }

        servidor.Nome = dto.Nome.Trim();
        servidor.Descricao = NormalizeOptionalText(dto.Descricao);
        servidor.Status = dto.Status;
        servidor.QuantidadeCreditos = dto.QuantidadeCreditos;
        servidor.ValorCustoCredito = dto.ValorCustoCredito;
        servidor.UsuarioPainel = dto.UsuarioPainel.Trim();
        servidor.SenhaPainel = dto.SenhaPainel.Trim();
        servidor.Observacao = NormalizeOptionalText(dto.Observacao);
        servidor.Ativo = dto.Ativo;

        servidorRepository.Update(servidor);
        await servidorRepository.SaveChangesAsync(cancellationToken);

        return OperationResult<ServidorDto>.Ok(MapToDto(servidor));
    }

    public async Task<OperationResult<ServidorDto>> UpdateStatusAsync(
        Guid id,
        UpdateServidorStatusDto dto,
        CancellationToken cancellationToken)
    {
        var validationErrors = ValidateStatus(dto.Status);
        if (validationErrors.Count > 0)
        {
            return OperationResult<ServidorDto>.Fail(validationErrors.ToArray());
        }

        var servidor = await servidorRepository.GetByIdAsync(id, cancellationToken);
        if (servidor is null)
        {
            return OperationResult<ServidorDto>.Fail(NotFoundMessage);
        }

        servidor.Status = dto.Status;

        servidorRepository.Update(servidor);
        await servidorRepository.SaveChangesAsync(cancellationToken);

        return OperationResult<ServidorDto>.Ok(MapToDto(servidor));
    }

    public async Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var servidor = await servidorRepository.GetByIdAsync(id, cancellationToken);
        if (servidor is null)
        {
            return OperationResult.Fail(NotFoundMessage);
        }

        servidorRepository.Remove(servidor);
        await servidorRepository.SaveChangesAsync(cancellationToken);

        return OperationResult.Ok();
    }

    private static List<string> Validate(
        string nome,
        int quantidadeCreditos,
        decimal valorCustoCredito,
        StatusServidor status)
    {
        var errors = ValidateStatus(status);

        if (string.IsNullOrWhiteSpace(nome))
        {
            errors.Add("Nome e obrigatorio.");
        }

        if (quantidadeCreditos < 0)
        {
            errors.Add("QuantidadeCreditos deve ser maior ou igual a zero.");
        }

        if (valorCustoCredito < 0)
        {
            errors.Add("ValorCustoCredito deve ser maior ou igual a zero.");
        }

        return errors;
    }

    private static List<string> ValidateStatus(StatusServidor status)
    {
        var errors = new List<string>();

        if (!Enum.IsDefined(status))
        {
            errors.Add("Status deve ser valido.");
        }

        return errors;
    }

    private static ServidorDto MapToDto(Servidor servidor)
    {
        return new ServidorDto
        {
            Id = servidor.Id,
            Nome = servidor.Nome,
            Descricao = servidor.Descricao,
            Status = servidor.Status,
            QuantidadeCreditos = servidor.QuantidadeCreditos,
            ValorCustoCredito = servidor.ValorCustoCredito,
            UsuarioPainel = servidor.UsuarioPainel,
            SenhaPainel = servidor.SenhaPainel,
            Observacao = servidor.Observacao,
            Ativo = servidor.Ativo
        };
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
