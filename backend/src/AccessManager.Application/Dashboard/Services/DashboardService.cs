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

        var totalClientes = await dashboardRepository.CountClientesAsync(cancellationToken);
        var telas = await dashboardRepository.GetTelasAsync(cancellationToken);
        var servidores = await dashboardRepository.GetServidoresAsync(cancellationToken);
        var lancamentos = await dashboardRepository.GetLancamentosFinanceirosAsync(cancellationToken);
        var primeiroDiaMes = new DateTime(hoje.Year, hoje.Month, 1);
        var primeiroDiaProximoMes = primeiroDiaMes.AddMonths(1);

        var resumo = new DashboardResumoDto
        {
            RendimentoMensal = lancamentos
                .Where(lancamento => lancamento.StatusFinanceiro == StatusFinanceiro.Pago &&
                    lancamento.DataPagamento is not null &&
                    lancamento.DataPagamento.Value.Date >= primeiroDiaMes &&
                    lancamento.DataPagamento.Value.Date < primeiroDiaProximoMes)
                .Sum(lancamento => lancamento.Valor),
            CustoMensal = servidores.Sum(servidor =>
                telas.Count(tela => tela.Ativo && tela.ServidorId == servidor.Id) * servidor.ValorCustoCredito),
            TotalClientes = totalClientes,
            TotalClientesPagosMes = lancamentos
                .Where(lancamento => lancamento.StatusFinanceiro == StatusFinanceiro.Pago &&
                    lancamento.DataPagamento is not null &&
                    lancamento.DataPagamento.Value.Date >= primeiroDiaMes &&
                    lancamento.DataPagamento.Value.Date < primeiroDiaProximoMes)
                .Select(lancamento => lancamento.ClienteId)
                .Distinct()
                .Count(),
            TotalTelasVencidas = telas.Count(tela => IsTelaVencida(tela, hoje)),
            TotalTelasVencendo = telas.Count(tela => IsTelaVencendo(tela, hoje, limiteVencendo)),
            TotalTelasAtivas = telas.Count(tela => IsTelaAtiva(tela, limiteVencendo)),
            TotalLancamentosPendentes = lancamentos.Count(IsLancamentoPendente),
            TotalLancamentosAtrasados = lancamentos.Count(lancamento => IsLancamentoAtrasado(lancamento, hoje)),
            TotalEmAberto = lancamentos
                .Where(IsLancamentoEmAberto)
                .Sum(lancamento => lancamento.Valor),
            ClientesPendentes = lancamentos
                .Where(IsLancamentoEmAberto)
                .GroupBy(lancamento => new
                {
                    lancamento.ClienteId,
                    NomeCliente = lancamento.Cliente == null ? string.Empty : lancamento.Cliente.Nome
                })
                .Select(grupo => new ClientePendenteFinanceiroDto
                {
                    ClienteId = grupo.Key.ClienteId,
                    NomeCliente = grupo.Key.NomeCliente,
                    QuantidadeLancamentos = grupo.Count(),
                    ValorEmAberto = grupo.Sum(lancamento => lancamento.Valor),
                    ProximoVencimento = grupo.Min(lancamento => lancamento.DataVencimentoFinanceiro)
                })
                .OrderBy(item => item.ProximoVencimento)
                .ToArray()
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
