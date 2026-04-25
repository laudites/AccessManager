using AccessManager.Api.Responses;
using AccessManager.Application.Dashboard.DTOs;
using AccessManager.Application.Dashboard.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AccessManager.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    [HttpGet("resumo")]
    public async Task<IActionResult> GetResumo(CancellationToken cancellationToken)
    {
        var result = await dashboardService.GetResumoAsync(cancellationToken);

        return Ok(ApiResponse<DashboardResumoDto>.Ok(result.Data!));
    }

    [HttpGet("telas-por-servidor")]
    public async Task<IActionResult> GetTelasPorServidor(CancellationToken cancellationToken)
    {
        var result = await dashboardService.GetTelasPorServidorAsync(cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<TelasPorServidorDto>>.Ok(result.Data!));
    }
}
