using AccessManager.Domain.Enums;

namespace AccessManager.Application.Servidores.DTOs;

public class CreateServidorDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public StatusServidor Status { get; set; }
    public int LimiteClientes { get; set; }
    public string UsuarioPainel { get; set; } = string.Empty;
    public string SenhaPainel { get; set; } = string.Empty;
    public string? Observacao { get; set; }
}
