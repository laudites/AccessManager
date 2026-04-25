using AccessManager.Domain.Enums;

namespace AccessManager.Application.Telas.DTOs;

public class UpdateTelaClienteDto
{
    public Guid? ClienteId { get; set; }
    public string NomeIdentificacao { get; set; } = string.Empty;
    public Guid? ServidorId { get; set; }
    public string UsuarioTela { get; set; } = string.Empty;
    public string SenhaTela { get; set; } = string.Empty;
    public decimal ValorAcordado { get; set; }
    public DateTime? DataVencimentoTecnico { get; set; }
    public StatusTela Status { get; set; }
    public string MarcaTv { get; set; } = string.Empty;
    public string AppUtilizado { get; set; } = string.Empty;
    public string? MacOuIdApp { get; set; }
    public string? ChaveSecundaria { get; set; }
    public string? Observacao { get; set; }
    public bool Ativo { get; set; }
}
