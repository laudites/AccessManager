namespace AccessManager.Domain.Entities;

public class RenovacaoTelaHistorico
{
    public Guid Id { get; set; }
    public Guid TelaClienteId { get; set; }
    public Guid ClienteId { get; set; }
    public Guid? ServidorAnteriorId { get; set; }
    public Guid? ServidorNovoId { get; set; }
    public DateTime DataVencimentoTecnicoAnterior { get; set; }
    public DateTime NovaDataVencimentoTecnico { get; set; }
    public decimal ValorAcordadoAnterior { get; set; }
    public decimal ValorAcordadoNovo { get; set; }
    public string? Observacao { get; set; }
    public DateTime DataCriacao { get; set; }

    public TelaCliente? TelaCliente { get; set; }
    public Cliente? Cliente { get; set; }
    public Servidor? ServidorAnterior { get; set; }
    public Servidor? ServidorNovo { get; set; }
}
