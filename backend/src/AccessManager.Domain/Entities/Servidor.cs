using AccessManager.Domain.Enums;

namespace AccessManager.Domain.Entities;

public class Servidor
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

    public ICollection<TelaCliente> Telas { get; set; } = [];
}
