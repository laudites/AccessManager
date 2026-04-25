using AccessManager.Api.Responses;
using AccessManager.Application.Clientes.DTOs;
using AccessManager.Application.Clientes.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AccessManager.Api.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController(IClienteService clienteService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateClienteDto dto, CancellationToken cancellationToken)
    {
        var result = await clienteService.CreateAsync(dto, cancellationToken);
        if (!result.Success)
        {
            return BadRequest(ApiResponse<ClienteDto>.Fail(result.Errors));
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data!.Id },
            ApiResponse<ClienteDto>.Ok(result.Data));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await clienteService.GetAllAsync(cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<ClienteDto>>.Ok(result.Data!));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await clienteService.GetByIdAsync(id, cancellationToken);
        if (!result.Success)
        {
            return NotFound(ApiResponse<ClienteDto>.Fail(result.Errors));
        }

        return Ok(ApiResponse<ClienteDto>.Ok(result.Data!));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateClienteDto dto, CancellationToken cancellationToken)
    {
        var result = await clienteService.UpdateAsync(id, dto, cancellationToken);
        if (!result.Success)
        {
            var response = ApiResponse<ClienteDto>.Fail(result.Errors);

            return result.Errors.Contains("Cliente não encontrado.")
                ? NotFound(response)
                : BadRequest(response);
        }

        return Ok(ApiResponse<ClienteDto>.Ok(result.Data!));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await clienteService.DeleteAsync(id, cancellationToken);
        if (!result.Success)
        {
            return NotFound(ApiResponse.Fail(result.Errors));
        }

        return Ok(ApiResponse.Ok());
    }
}
