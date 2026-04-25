namespace AccessManager.Application.Dashboard.DTOs;

public class DashboardResumoDto
{
    public int TotalClientes { get; set; }
    public int TotalTelasAtivas { get; set; }
    public int TotalTelasVencendo { get; set; }
    public int TotalTelasVencidas { get; set; }
    public int TotalLancamentosPendentes { get; set; }
    public int TotalLancamentosAtrasados { get; set; }
    public decimal TotalEmAberto { get; set; }
}
