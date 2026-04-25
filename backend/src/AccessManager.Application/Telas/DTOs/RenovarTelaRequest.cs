namespace AccessManager.Application.Telas.DTOs;

public class RenovarTelaRequest
{
    public DateTime? NovaDataVencimentoTecnico { get; set; }
    public decimal? ValorAcordadoNovo { get; set; }
    public string? Observacao { get; set; }
}
