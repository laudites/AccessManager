namespace AccessManager.Domain.Entities;

public class Cliente
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Observacao { get; set; }
    public int? DiaPagamentoPreferido { get; set; }
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; }

    public ICollection<TelaCliente> Telas { get; set; } = [];
    public ICollection<LancamentoFinanceiro> LancamentosFinanceiros { get; set; } = [];
}
