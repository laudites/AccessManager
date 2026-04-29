using AccessManager.Domain.Enums;

namespace AccessManager.Application.Financeiro.DTOs;

public class LancamentoFinanceiroDto
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public Guid? TelaClienteId { get; set; }
    public string? TelaClienteNome { get; set; }
    public DateTime CompetenciaReferencia { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataVencimentoFinanceiro { get; set; }
    public DateTime? DataPagamento { get; set; }
    public StatusFinanceiro StatusFinanceiro { get; set; }
    public StatusFinanceiro StatusFinanceiroExibicao { get; set; }
    public string? Observacao { get; set; }
    public DateTime DataCriacao { get; set; }
}
