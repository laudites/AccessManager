using AccessManager.Api.Responses;
using AccessManager.Application.Financeiro.DTOs;
using AccessManager.Application.Financeiro.Interfaces;
using AccessManager.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AccessManager.Api.Controllers;

[ApiController]
[Route("api/lancamentos-financeiros")]
public class LancamentosFinanceirosController(ILancamentoFinanceiroService lancamentoFinanceiroService) : ControllerBase
{
    private const string NotFoundMessage = "Lancamento financeiro nao encontrado.";

    [HttpPost]
    public async Task<IActionResult> Create(CreateLancamentoFinanceiroDto dto, CancellationToken cancellationToken)
    {
        var result = await lancamentoFinanceiroService.CreateAsync(dto, cancellationToken);
        if (!result.Success)
        {
            return BadRequest(ApiResponse<LancamentoFinanceiroDto>.Fail(result.Errors));
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data!.Id },
            ApiResponse<LancamentoFinanceiroDto>.Ok(result.Data));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? clienteId,
        [FromQuery] Guid? telaClienteId,
        [FromQuery] StatusFinanceiro? statusFinanceiro,
        CancellationToken cancellationToken)
    {
        var result = await lancamentoFinanceiroService.GetAllAsync(
            clienteId,
            telaClienteId,
            statusFinanceiro,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(ApiResponse<IReadOnlyCollection<LancamentoFinanceiroDto>>.Fail(result.Errors));
        }

        return Ok(ApiResponse<IReadOnlyCollection<LancamentoFinanceiroDto>>.Ok(result.Data!));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await lancamentoFinanceiroService.GetByIdAsync(id, cancellationToken);
        if (!result.Success)
        {
            return NotFound(ApiResponse<LancamentoFinanceiroDto>.Fail(result.Errors));
        }

        return Ok(ApiResponse<LancamentoFinanceiroDto>.Ok(result.Data!));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateLancamentoFinanceiroDto dto,
        CancellationToken cancellationToken)
    {
        var result = await lancamentoFinanceiroService.UpdateAsync(id, dto, cancellationToken);
        if (!result.Success)
        {
            var response = ApiResponse<LancamentoFinanceiroDto>.Fail(result.Errors);

            return result.Errors.Contains(NotFoundMessage)
                ? NotFound(response)
                : BadRequest(response);
        }

        return Ok(ApiResponse<LancamentoFinanceiroDto>.Ok(result.Data!));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await lancamentoFinanceiroService.DeleteAsync(id, cancellationToken);
        if (!result.Success)
        {
            return NotFound(ApiResponse.Fail(result.Errors));
        }

        return Ok(ApiResponse.Ok());
    }

    [HttpPatch("{id:guid}/marcar-pago")]
    public async Task<IActionResult> MarcarComoPago(Guid id, CancellationToken cancellationToken)
    {
        var result = await lancamentoFinanceiroService.MarcarComoPagoAsync(id, cancellationToken);
        if (!result.Success)
        {
            return NotFound(ApiResponse<LancamentoFinanceiroDto>.Fail(result.Errors));
        }

        return Ok(ApiResponse<LancamentoFinanceiroDto>.Ok(result.Data!));
    }

    [HttpGet("pendentes")]
    public async Task<IActionResult> GetPendentes(CancellationToken cancellationToken)
    {
        var result = await lancamentoFinanceiroService.GetPendentesAsync(cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<LancamentoFinanceiroDto>>.Ok(result.Data!));
    }

    [HttpGet("atrasados")]
    public async Task<IActionResult> GetAtrasados(CancellationToken cancellationToken)
    {
        var result = await lancamentoFinanceiroService.GetAtrasadosAsync(cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<LancamentoFinanceiroDto>>.Ok(result.Data!));
    }
}
