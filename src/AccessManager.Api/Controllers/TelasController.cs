using AccessManager.Api.Responses;
using AccessManager.Application.Telas.DTOs;
using AccessManager.Application.Telas.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AccessManager.Api.Controllers;

[ApiController]
[Route("api/telas")]
public class TelasController(ITelaClienteService telaClienteService) : ControllerBase
{
    private const string NotFoundMessage = "Tela nao encontrada.";

    [HttpPost]
    public async Task<IActionResult> Create(CreateTelaClienteDto dto, CancellationToken cancellationToken)
    {
        var result = await telaClienteService.CreateAsync(dto, cancellationToken);
        if (!result.Success)
        {
            return BadRequest(ApiResponse<TelaClienteDto>.Fail(result.Errors));
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data!.Id },
            ApiResponse<TelaClienteDto>.Ok(result.Data));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? clienteId,
        [FromQuery] Guid? servidorId,
        CancellationToken cancellationToken)
    {
        var result = await telaClienteService.GetAllAsync(clienteId, servidorId, cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<TelaClienteDto>>.Ok(result.Data!));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await telaClienteService.GetByIdAsync(id, cancellationToken);
        if (!result.Success)
        {
            return NotFound(ApiResponse<TelaClienteDto>.Fail(result.Errors));
        }

        return Ok(ApiResponse<TelaClienteDto>.Ok(result.Data!));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTelaClienteDto dto, CancellationToken cancellationToken)
    {
        var result = await telaClienteService.UpdateAsync(id, dto, cancellationToken);
        if (!result.Success)
        {
            var response = ApiResponse<TelaClienteDto>.Fail(result.Errors);

            return result.Errors.Contains(NotFoundMessage)
                ? NotFound(response)
                : BadRequest(response);
        }

        return Ok(ApiResponse<TelaClienteDto>.Ok(result.Data!));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await telaClienteService.DeleteAsync(id, cancellationToken);
        if (!result.Success)
        {
            return NotFound(ApiResponse.Fail(result.Errors));
        }

        return Ok(ApiResponse.Ok());
    }
}
