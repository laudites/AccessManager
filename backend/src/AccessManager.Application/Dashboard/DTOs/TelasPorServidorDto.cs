namespace AccessManager.Application.Dashboard.DTOs;

public class TelasPorServidorDto
{
    public Guid ServidorId { get; set; }
    public string NomeServidor { get; set; } = string.Empty;
    public int QuantidadeTelas { get; set; }
}
