using AccessManager.Domain.Enums;

namespace AccessManager.Domain.Entities;

public class TelaCliente
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public string NomeIdentificacao { get; set; } = string.Empty;
    public Guid ServidorId { get; set; }
    public string UsuarioTela { get; set; } = string.Empty;
    public string SenhaTela { get; set; } = string.Empty;
    public decimal ValorAcordado { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataVencimentoTecnico { get; set; }
    public StatusTela Status { get; set; }
    public string MarcaTv { get; set; } = string.Empty;
    public string AppUtilizado { get; set; } = string.Empty;
    public string? MacOuIdApp { get; set; }
    public string? ChaveSecundaria { get; set; }
    public string? Observacao { get; set; }
    public bool Ativo { get; set; }

    public Cliente? Cliente { get; set; }
    public Servidor? Servidor { get; set; }
    public ICollection<RenovacaoTelaHistorico> RenovacoesHistorico { get; set; } = [];
    public ICollection<LancamentoFinanceiro> LancamentosFinanceiros { get; set; } = [];
}
