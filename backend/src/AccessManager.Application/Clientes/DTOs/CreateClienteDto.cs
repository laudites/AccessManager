namespace AccessManager.Application.Clientes.DTOs;

public class CreateClienteDto
{
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Observacao { get; set; }
    public int? DiaPagamentoPreferido { get; set; }
}
