using AccessManager.Domain.Enums;

namespace AccessManager.Application.Servidores.DTOs;

public class ServidorDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public StatusServidor Status { get; set; }
    public int QuantidadeCreditos { get; set; }
    public decimal ValorCustoCredito { get; set; }
    public string UsuarioPainel { get; set; } = string.Empty;
    public string SenhaPainel { get; set; } = string.Empty;
    public string? Observacao { get; set; }
    public bool Ativo { get; set; }
}
