namespace AccessManager.Application.Dashboard.DTOs;

public class TelasPorServidorDto
{
    public Guid ServidorId { get; set; }
    public string NomeServidor { get; set; } = string.Empty;
    public int QuantidadeCreditos { get; set; }
    public decimal ValorCustoCredito { get; set; }
    public int QuantidadeClientes { get; set; }
    public int QuantidadeTelas { get; set; }
    public decimal CustoEstimado { get; set; }
}
