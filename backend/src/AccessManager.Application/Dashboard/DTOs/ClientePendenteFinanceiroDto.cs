namespace AccessManager.Application.Dashboard.DTOs;

public class ClientePendenteFinanceiroDto
{
    public Guid ClienteId { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public int QuantidadeLancamentos { get; set; }
    public decimal ValorEmAberto { get; set; }
    public DateTime ProximoVencimento { get; set; }
}
