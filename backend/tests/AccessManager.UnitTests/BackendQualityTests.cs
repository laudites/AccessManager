using AccessManager.Application.Clientes.DTOs;
using AccessManager.Application.Clientes.Interfaces;
using AccessManager.Application.Clientes.Services;
using AccessManager.Application.Dashboard.Interfaces;
using AccessManager.Application.Dashboard.Services;
using AccessManager.Application.Financeiro.DTOs;
using AccessManager.Application.Financeiro.Interfaces;
using AccessManager.Application.Financeiro.Services;
using AccessManager.Application.Servidores.DTOs;
using AccessManager.Application.Servidores.Interfaces;
using AccessManager.Application.Servidores.Services;
using AccessManager.Application.Telas.DTOs;
using AccessManager.Application.Telas.Interfaces;
using AccessManager.Application.Telas.Services;
using AccessManager.Domain.Entities;
using AccessManager.Domain.Enums;

namespace AccessManager.UnitTests;

public class BackendQualityTests
{
    [Fact]
    public async Task Cliente_NomeObrigatorio_DeveFalhar()
    {
        var service = new ClienteService(new ClienteRepositoryFake());

        var result = await service.CreateAsync(new CreateClienteDto
        {
            Nome = " ",
            Telefone = "11999999999"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("Nome"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(32)]
    public async Task Cliente_DiaPagamentoForaDoIntervalo_DeveFalhar(int diaPagamento)
    {
        var service = new ClienteService(new ClienteRepositoryFake());

        var result = await service.CreateAsync(new CreateClienteDto
        {
            Nome = "Cliente Teste",
            Telefone = "11999999999",
            DiaPagamentoPreferido = diaPagamento
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("DiaPagamentoPreferido"));
    }

    [Fact]
    public async Task Servidor_QuantidadeCreditosNegativa_DeveFalhar()
    {
        var service = new ServidorService(new ServidorRepositoryFake());

        var result = await service.CreateAsync(new CreateServidorDto
        {
            Nome = "Servidor Teste",
            Status = StatusServidor.Ativo,
            QuantidadeCreditos = -1,
            UsuarioPainel = "admin",
            SenhaPainel = "senha"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("QuantidadeCreditos"));
    }

    [Fact]
    public async Task Servidor_ValorCustoCreditoNegativo_DeveFalhar()
    {
        var service = new ServidorService(new ServidorRepositoryFake());

        var result = await service.CreateAsync(new CreateServidorDto
        {
            Nome = "Servidor Teste",
            Status = StatusServidor.Ativo,
            QuantidadeCreditos = 10,
            ValorCustoCredito = -1,
            UsuarioPainel = "admin",
            SenhaPainel = "senha"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("ValorCustoCredito"));
    }

    [Fact]
    public async Task TelaCliente_ValorAcordadoNegativo_DeveFalhar()
    {
        var clienteId = Guid.NewGuid();
        var servidorId = Guid.NewGuid();
        var service = new TelaClienteService(
            new TelaClienteRepositoryFake(),
            new RenovacaoTelaHistoricoRepositoryFake(),
            new ClienteRepositoryFake(new Cliente { Id = clienteId, Nome = "Cliente" }),
            new ServidorRepositoryFake(new Servidor { Id = servidorId, Nome = "Servidor", Status = StatusServidor.Ativo }));

        var result = await service.CreateAsync(new CreateTelaClienteDto
        {
            ClienteId = clienteId,
            ServidorId = servidorId,
            NomeIdentificacao = "Sala",
            UsuarioTela = "usuario",
            SenhaTela = "senha",
            ValorAcordado = -1,
            DataVencimentoTecnico = DateTime.UtcNow.AddDays(30),
            Status = StatusTela.Ativo
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("ValorAcordado"));
    }

    [Fact]
    public async Task Dashboard_DeveClassificarTelasVencidasVencendoEAtivasPelaDataTecnica()
    {
        var hoje = DateTime.UtcNow.Date;
        var repository = new DashboardRepositoryFake(telas:
        [
            new TelaCliente { Id = Guid.NewGuid(), DataVencimentoTecnico = hoje.AddDays(-1) },
            new TelaCliente { Id = Guid.NewGuid(), DataVencimentoTecnico = hoje.AddDays(2) },
            new TelaCliente { Id = Guid.NewGuid(), DataVencimentoTecnico = hoje.AddDays(4) }
        ]);
        var service = new DashboardService(repository);

        var result = await service.GetResumoAsync(CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(1, result.Data!.TotalTelasVencidas);
        Assert.Equal(1, result.Data.TotalTelasVencendo);
        Assert.Equal(1, result.Data.TotalTelasAtivas);
    }

    [Fact]
    public async Task Financeiro_MarcarPagamento_NaoAlteraVencimentoTecnicoDaTela()
    {
        var tela = new TelaCliente
        {
            Id = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            DataVencimentoTecnico = DateTime.UtcNow.AddDays(10)
        };
        var vencimentoTecnicoOriginal = tela.DataVencimentoTecnico;
        var lancamento = new LancamentoFinanceiro
        {
            Id = Guid.NewGuid(),
            ClienteId = tela.ClienteId,
            TelaClienteId = tela.Id,
            Valor = 100,
            StatusFinanceiro = StatusFinanceiro.Pendente,
            DataVencimentoFinanceiro = DateTime.UtcNow.AddDays(1)
        };
        var service = new LancamentoFinanceiroService(
            new LancamentoFinanceiroRepositoryFake(lancamento),
            new ClienteRepositoryFake(new Cliente { Id = tela.ClienteId, Nome = "Cliente", Telas = [tela] }));

        var result = await service.MarcarComoPagoAsync(lancamento.Id, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(StatusFinanceiro.Pago, result.Data!.StatusFinanceiro);
        Assert.NotNull(result.Data.DataPagamento);
        Assert.Equal(vencimentoTecnicoOriginal, tela.DataVencimentoTecnico);
    }

    [Fact]
    public async Task Financeiro_DeveAgruparValorDasTelasAtivasDoCliente()
    {
        var clienteId = Guid.NewGuid();
        var telas = new[]
        {
            new TelaCliente { Id = Guid.NewGuid(), ClienteId = clienteId, ValorAcordado = 30, Ativo = true },
            new TelaCliente { Id = Guid.NewGuid(), ClienteId = clienteId, ValorAcordado = 40, Ativo = true },
            new TelaCliente { Id = Guid.NewGuid(), ClienteId = clienteId, ValorAcordado = 99, Ativo = false }
        };
        var cliente = new Cliente { Id = clienteId, Nome = "Cliente", Telas = telas };
        var service = new LancamentoFinanceiroService(
            new LancamentoFinanceiroRepositoryFake(),
            new ClienteRepositoryFake(cliente));

        var result = await service.CreateAsync(new CreateLancamentoFinanceiroDto
        {
            ClienteId = clienteId,
            DataVencimentoFinanceiro = DateTime.UtcNow.Date.AddDays(5)
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(70, result.Data!.Valor);
        Assert.Equal(new DateTime(result.Data.DataVencimentoFinanceiro.Year, result.Data.DataVencimentoFinanceiro.Month, 1), result.Data.CompetenciaReferencia);
        Assert.Null(result.Data.TelaClienteId);
    }

    [Fact]
    public async Task Cliente_DeveRetornarQuantidadeTelasEValorAgrupado()
    {
        var cliente = new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = "Cliente",
            Telas =
            [
                new TelaCliente { Id = Guid.NewGuid(), ValorAcordado = 25, Ativo = true },
                new TelaCliente { Id = Guid.NewGuid(), ValorAcordado = 35, Ativo = true },
                new TelaCliente { Id = Guid.NewGuid(), ValorAcordado = 50, Ativo = false }
            ]
        };
        var service = new ClienteService(new ClienteRepositoryFake(cliente));

        var result = await service.GetByIdAsync(cliente.Id, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(2, result.Data!.QuantidadeTelas);
        Assert.Equal(60, result.Data.ValorTotalTelas);
    }

    [Fact]
    public async Task Dashboard_DeveCalcularRendimentoMensalECustoMensal()
    {
        var hoje = DateTime.UtcNow.Date;
        var servidorId = Guid.NewGuid();
        var repository = new DashboardRepositoryFake(
            telas:
            [
                new TelaCliente { Id = Guid.NewGuid(), ServidorId = servidorId, Ativo = true, DataVencimentoTecnico = hoje.AddDays(10) },
                new TelaCliente { Id = Guid.NewGuid(), ServidorId = servidorId, Ativo = true, DataVencimentoTecnico = hoje.AddDays(10) }
            ],
            lancamentos:
            [
                new LancamentoFinanceiro
                {
                    Id = Guid.NewGuid(),
                    ClienteId = Guid.NewGuid(),
                    Valor = 100,
                    StatusFinanceiro = StatusFinanceiro.Pago,
                    DataPagamento = hoje
                }
            ],
            servidores:
            [
                new Servidor { Id = servidorId, Nome = "Servidor", ValorCustoCredito = 7 }
            ]);
        var service = new DashboardService(repository);

        var result = await service.GetResumoAsync(CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(100, result.Data!.RendimentoMensal);
        Assert.Equal(14, result.Data.CustoMensal);
    }

    [Fact]
    public async Task Dashboard_NaoDeveExecutarConsultasEmParaleloNoMesmoRepositorio()
    {
        var service = new DashboardService(new DashboardRepositoryConcurrencyFake());

        var result = await service.GetResumoAsync(CancellationToken.None);

        Assert.True(result.Success);
    }

    private sealed class ClienteRepositoryFake(params Cliente[] clientes) : IClienteRepository
    {
        private readonly List<Cliente> _clientes = [.. clientes];

        public Task<IReadOnlyCollection<Cliente>> GetAllAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyCollection<Cliente>>(_clientes);
        }

        public Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_clientes.FirstOrDefault(cliente => cliente.Id == id));
        }

        public Task AddAsync(Cliente cliente, CancellationToken cancellationToken)
        {
            _clientes.Add(cliente);
            return Task.CompletedTask;
        }

        public void Update(Cliente cliente)
        {
        }

        public void Remove(Cliente cliente)
        {
            _clientes.Remove(cliente);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class ServidorRepositoryFake(params Servidor[] servidores) : IServidorRepository
    {
        private readonly List<Servidor> _servidores = [.. servidores];

        public Task<IReadOnlyCollection<Servidor>> GetAllAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyCollection<Servidor>>(_servidores);
        }

        public Task<Servidor?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_servidores.FirstOrDefault(servidor => servidor.Id == id));
        }

        public Task AddAsync(Servidor servidor, CancellationToken cancellationToken)
        {
            _servidores.Add(servidor);
            return Task.CompletedTask;
        }

        public void Update(Servidor servidor)
        {
        }

        public void Remove(Servidor servidor)
        {
            _servidores.Remove(servidor);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class TelaClienteRepositoryFake(params TelaCliente[] telas) : ITelaClienteRepository
    {
        private readonly List<TelaCliente> _telas = [.. telas];

        public Task<IReadOnlyCollection<TelaCliente>> GetAllAsync(
            Guid? clienteId,
            Guid? servidorId,
            CancellationToken cancellationToken)
        {
            IEnumerable<TelaCliente> query = _telas;

            if (clienteId is not null)
            {
                query = query.Where(tela => tela.ClienteId == clienteId);
            }

            if (servidorId is not null)
            {
                query = query.Where(tela => tela.ServidorId == servidorId);
            }

            return Task.FromResult<IReadOnlyCollection<TelaCliente>>(query.ToArray());
        }

        public Task<TelaCliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_telas.FirstOrDefault(tela => tela.Id == id));
        }

        public Task AddAsync(TelaCliente telaCliente, CancellationToken cancellationToken)
        {
            _telas.Add(telaCliente);
            return Task.CompletedTask;
        }

        public void Update(TelaCliente telaCliente)
        {
        }

        public void Remove(TelaCliente telaCliente)
        {
            _telas.Remove(telaCliente);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class RenovacaoTelaHistoricoRepositoryFake : IRenovacaoTelaHistoricoRepository
    {
        public Task AddAsync(RenovacaoTelaHistorico historico, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class LancamentoFinanceiroRepositoryFake(params LancamentoFinanceiro[] lancamentos) : ILancamentoFinanceiroRepository
    {
        private readonly List<LancamentoFinanceiro> _lancamentos = [.. lancamentos];

        public Task<IReadOnlyCollection<LancamentoFinanceiro>> GetAllAsync(
            Guid? clienteId,
            Guid? telaClienteId,
            StatusFinanceiro? statusFinanceiro,
            CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyCollection<LancamentoFinanceiro>>(_lancamentos);
        }

        public Task<IReadOnlyCollection<LancamentoFinanceiro>> GetPendentesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyCollection<LancamentoFinanceiro>>(
                _lancamentos.Where(lancamento => lancamento.StatusFinanceiro == StatusFinanceiro.Pendente).ToArray());
        }

        public Task<IReadOnlyCollection<LancamentoFinanceiro>> GetAtrasadosAsync(DateTime hoje, CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyCollection<LancamentoFinanceiro>>(
                _lancamentos.Where(lancamento => lancamento.DataVencimentoFinanceiro.Date < hoje).ToArray());
        }

        public Task<bool> ExistsByClienteAndVencimentoAsync(
            Guid clienteId,
            DateTime dataVencimentoFinanceiro,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_lancamentos.Any(lancamento =>
                lancamento.ClienteId == clienteId &&
                lancamento.DataVencimentoFinanceiro.Date == dataVencimentoFinanceiro.Date &&
                lancamento.StatusFinanceiro != StatusFinanceiro.Cancelado));
        }

        public Task<LancamentoFinanceiro?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_lancamentos.FirstOrDefault(lancamento => lancamento.Id == id));
        }

        public Task AddAsync(LancamentoFinanceiro lancamentoFinanceiro, CancellationToken cancellationToken)
        {
            _lancamentos.Add(lancamentoFinanceiro);
            return Task.CompletedTask;
        }

        public void Update(LancamentoFinanceiro lancamentoFinanceiro)
        {
        }

        public void Remove(LancamentoFinanceiro lancamentoFinanceiro)
        {
            _lancamentos.Remove(lancamentoFinanceiro);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class DashboardRepositoryFake(
        IReadOnlyCollection<TelaCliente>? telas = null,
        IReadOnlyCollection<LancamentoFinanceiro>? lancamentos = null,
        IReadOnlyCollection<Servidor>? servidores = null) : IDashboardRepository
    {
        public Task<int> CountClientesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task<IReadOnlyCollection<TelaCliente>> GetTelasAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(telas ?? []);
        }

        public Task<IReadOnlyCollection<LancamentoFinanceiro>> GetLancamentosFinanceirosAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(lancamentos ?? []);
        }

        public Task<IReadOnlyCollection<Servidor>> GetServidoresAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(servidores ?? []);
        }

        public Task<IReadOnlyCollection<AccessManager.Application.Dashboard.DTOs.TelasPorServidorDto>> GetTelasPorServidorAsync(
            CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyCollection<AccessManager.Application.Dashboard.DTOs.TelasPorServidorDto>>([]);
        }
    }

    private sealed class DashboardRepositoryConcurrencyFake : IDashboardRepository
    {
        private int _activeOperations;

        public async Task<int> CountClientesAsync(CancellationToken cancellationToken)
        {
            await SimulateDbContextOperationAsync(cancellationToken);
            return 0;
        }

        public async Task<IReadOnlyCollection<TelaCliente>> GetTelasAsync(CancellationToken cancellationToken)
        {
            await SimulateDbContextOperationAsync(cancellationToken);
            return [];
        }

        public async Task<IReadOnlyCollection<Servidor>> GetServidoresAsync(CancellationToken cancellationToken)
        {
            await SimulateDbContextOperationAsync(cancellationToken);
            return [];
        }

        public async Task<IReadOnlyCollection<LancamentoFinanceiro>> GetLancamentosFinanceirosAsync(
            CancellationToken cancellationToken)
        {
            await SimulateDbContextOperationAsync(cancellationToken);
            return [];
        }

        public Task<IReadOnlyCollection<AccessManager.Application.Dashboard.DTOs.TelasPorServidorDto>> GetTelasPorServidorAsync(
            CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyCollection<AccessManager.Application.Dashboard.DTOs.TelasPorServidorDto>>([]);
        }

        private async Task SimulateDbContextOperationAsync(CancellationToken cancellationToken)
        {
            if (Interlocked.Increment(ref _activeOperations) != 1)
            {
                throw new InvalidOperationException("Operacoes concorrentes no mesmo repositorio.");
            }

            try
            {
                await Task.Delay(10, cancellationToken);
            }
            finally
            {
                Interlocked.Decrement(ref _activeOperations);
            }
        }
    }
}
