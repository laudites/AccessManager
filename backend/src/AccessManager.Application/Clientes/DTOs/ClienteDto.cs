namespace AccessManager.Application.Clientes.DTOs;

public class ClienteDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Observacao { get; set; }
    public int? DiaPagamentoPreferido { get; set; }
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; }
    public int QuantidadeTelas { get; set; }
    public decimal ValorTotalTelas { get; set; }
    public string StatusFinanceiroCliente { get; set; } = string.Empty;
}
