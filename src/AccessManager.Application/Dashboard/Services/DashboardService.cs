using AccessManager.Application.Common;
using AccessManager.Application.Dashboard.DTOs;
using AccessManager.Application.Dashboard.Interfaces;
using AccessManager.Domain.Entities;
using AccessManager.Domain.Enums;

namespace AccessManager.Application.Dashboard.Services;

public class DashboardService(IDashboardRepository dashboardRepository) : IDashboardService
{
    public async Task<OperationResult<DashboardResumoDto>> GetResumoAsync(CancellationToken cancellationToken)
    {
        var hoje = DateTime.UtcNow.Date;
        var limiteVencendo = hoje.AddDays(3);

        var totalClientesTask = dashboardRepository.CountClientesAsync(cancellationToken);
        var telasTask = dashboardRepository.GetTelasAsync(cancellationToken);
        var lancamentosTask = dashboardRepository.GetLancamentosFinanceirosAsync(cancellationToken);

        await Task.WhenAll(totalClientesTask, telasTask, lancamentosTask);

        var telas = telasTask.Result;
        var lancamentos = lancamentosTask.Result;

        var resumo = new DashboardResumoDto
        {
            TotalClientes = totalClientesTask.Result,
            TotalTelasVencidas = telas.Count(tela => IsTelaVencida(tela, hoje)),
            TotalTelasVencendo = telas.Count(tela => IsTelaVencendo(tela, hoje, limiteVencendo)),
            TotalTelasAtivas = telas.Count(tela => IsTelaAtiva(tela, limiteVencendo)),
            TotalLancamentosPendentes = lancamentos.Count(IsLancamentoPendente),
            TotalLancamentosAtrasados = lancamentos.Count(lancamento => IsLancamentoAtrasado(lancamento, hoje)),
            TotalEmAberto = lancamentos
                .Where(IsLancamentoEmAberto)
                .Sum(lancamento => lancamento.Valor)
        };

        return OperationResult<DashboardResumoDto>.Ok(resumo);
    }

    public async Task<OperationResult<IReadOnlyCollection<TelasPorServidorDto>>> GetTelasPorServidorAsync(
        CancellationToken cancellationToken)
    {
        var telasPorServidor = await dashboardRepository.GetTelasPorServidorAsync(cancellationToken);

        return OperationResult<IReadOnlyCollection<TelasPorServidorDto>>.Ok(telasPorServidor);
    }

    private static bool IsTelaVencida(TelaCliente tela, DateTime hoje)
    {
        return tela.DataVencimentoTecnico.Date < hoje;
    }

    private static bool IsTelaVencendo(TelaCliente tela, DateTime hoje, DateTime limiteVencendo)
    {
        return tela.DataVencimentoTecnico.Date >= hoje &&
            tela.DataVencimentoTecnico.Date <= limiteVencendo;
    }

    private static bool IsTelaAtiva(TelaCliente tela, DateTime limiteVencendo)
    {
        return tela.DataVencimentoTecnico.Date > limiteVencendo;
    }

    private static bool IsLancamentoPendente(LancamentoFinanceiro lancamento)
    {
        return lancamento.StatusFinanceiro == StatusFinanceiro.Pendente;
    }

    private static bool IsLancamentoAtrasado(LancamentoFinanceiro lancamento, DateTime hoje)
    {
        return lancamento.StatusFinanceiro == StatusFinanceiro.Atrasado ||
            (lancamento.DataPagamento is null &&
                lancamento.StatusFinanceiro != StatusFinanceiro.Pago &&
                lancamento.StatusFinanceiro != StatusFinanceiro.Cancelado &&
                lancamento.DataVencimentoFinanceiro.Date < hoje);
    }

    private static bool IsLancamentoEmAberto(LancamentoFinanceiro lancamento)
    {
        return lancamento.DataPagamento is null &&
            lancamento.StatusFinanceiro != StatusFinanceiro.Pago &&
            lancamento.StatusFinanceiro != StatusFinanceiro.Cancelado;
    }
}
