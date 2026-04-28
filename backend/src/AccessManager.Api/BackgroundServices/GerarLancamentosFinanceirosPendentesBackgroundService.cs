using AccessManager.Application.Financeiro.DTOs;
using AccessManager.Application.Financeiro.Interfaces;

namespace AccessManager.Api.BackgroundServices;

public class GerarLancamentosFinanceirosPendentesBackgroundService(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<GerarLancamentosFinanceirosPendentesBackgroundService> logger) : BackgroundService
{
    private static readonly TimeSpan IntervaloExecucao = TimeSpan.FromDays(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Job de geracao de lancamentos financeiros pendentes iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await ExecutarGeracaoAsync(stoppingToken);

            try
            {
                await Task.Delay(IntervaloExecucao, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        logger.LogInformation("Job de geracao de lancamentos financeiros pendentes finalizado.");
    }

    public async Task ExecutarGeracaoAsync(CancellationToken cancellationToken)
    {
        try
        {
            var dataReferencia = DateTime.UtcNow.Date;

            using var scope = serviceScopeFactory.CreateScope();
            var lancamentoFinanceiroService = scope.ServiceProvider.GetRequiredService<ILancamentoFinanceiroService>();

            logger.LogInformation(
                "Executando geracao automatica de lancamentos financeiros pendentes para {DataReferencia}.",
                dataReferencia);

            var result = await lancamentoFinanceiroService.GerarPendentesAsync(
                new GerarLancamentosFinanceirosRequest { DataReferencia = dataReferencia },
                cancellationToken);

            if (!result.Success)
            {
                logger.LogWarning(
                    "Geracao automatica de lancamentos financeiros pendentes falhou: {Erros}.",
                    string.Join("; ", result.Errors));
                return;
            }

            logger.LogInformation(
                "Geracao automatica de lancamentos financeiros pendentes concluida. Lancamentos gerados: {Quantidade}.",
                result.Data?.Count ?? 0);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Erro inesperado na geracao automatica de lancamentos financeiros pendentes.");
        }
    }
}
