using AccessManager.Domain.Enums;

namespace AccessManager.Application.Financeiro.DTOs;

public class CreateLancamentoFinanceiroDto
{
    public Guid? ClienteId { get; set; }
    public Guid? TelaClienteId { get; set; }
    public DateTime? CompetenciaReferencia { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime? DataVencimentoFinanceiro { get; set; }
    public DateTime? DataPagamento { get; set; }
    public StatusFinanceiro? StatusFinanceiro { get; set; }
    public string? Observacao { get; set; }
}
