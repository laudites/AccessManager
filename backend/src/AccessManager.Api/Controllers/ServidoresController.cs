using AccessManager.Api.Responses;
using AccessManager.Application.Servidores.DTOs;
using AccessManager.Application.Servidores.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AccessManager.Api.Controllers;

[ApiController]
[Route("api/servidores")]
public class ServidoresController(IServidorService servidorService) : ControllerBase
{
    private const string NotFoundMessage = "Servidor nao encontrado.";

    [HttpPost]
    public async Task<IActionResult> Create(CreateServidorDto dto, CancellationToken cancellationToken)
    {
        var result = await servidorService.CreateAsync(dto, cancellationToken);
        if (!result.Success)
        {
            return BadRequest(ApiResponse<ServidorDto>.Fail(result.Errors));
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data!.Id },
            ApiResponse<ServidorDto>.Ok(result.Data));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await servidorService.GetAllAsync(cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<ServidorDto>>.Ok(result.Data!));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await servidorService.GetByIdAsync(id, cancellationToken);
        if (!result.Success)
        {
            return NotFound(ApiResponse<ServidorDto>.Fail(result.Errors));
        }

        return Ok(ApiResponse<ServidorDto>.Ok(result.Data!));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateServidorDto dto, CancellationToken cancellationToken)
    {
        var result = await servidorService.UpdateAsync(id, dto, cancellationToken);
        if (!result.Success)
        {
            var response = ApiResponse<ServidorDto>.Fail(result.Errors);

            return result.Errors.Contains(NotFoundMessage)
                ? NotFound(response)
                : BadRequest(response);
        }

        return Ok(ApiResponse<ServidorDto>.Ok(result.Data!));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await servidorService.DeleteAsync(id, cancellationToken);
        if (!result.Success)
        {
            return NotFound(ApiResponse.Fail(result.Errors));
        }

        return Ok(ApiResponse.Ok());
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        UpdateServidorStatusDto dto,
        CancellationToken cancellationToken)
    {
        var result = await servidorService.UpdateStatusAsync(id, dto, cancellationToken);
        if (!result.Success)
        {
            var response = ApiResponse<ServidorDto>.Fail(result.Errors);

            return result.Errors.Contains(NotFoundMessage)
                ? NotFound(response)
                : BadRequest(response);
        }

        return Ok(ApiResponse<ServidorDto>.Ok(result.Data!));
    }
}
