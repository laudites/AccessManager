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
    public async Task TelaCliente_DeveRetornarStatusExibicaoCalculadoSemAlterarStatusSalvo()
    {
        var hoje = DateTime.UtcNow.Date;
        var telaVencida = new TelaCliente
        {
            Id = Guid.NewGuid(),
            Status = StatusTela.Ativo,
            DataVencimentoTecnico = hoje.AddDays(-1)
        };
        var telaSuspensa = new TelaCliente
        {
            Id = Guid.NewGuid(),
            Status = StatusTela.Suspenso,
            DataVencimentoTecnico = hoje.AddDays(-1)
        };
        var telaCancelada = new TelaCliente
        {
            Id = Guid.NewGuid(),
            Status = StatusTela.Cancelado,
            DataVencimentoTecnico = hoje.AddDays(-1)
        };
        var telaVencendo = new TelaCliente
        {
            Id = Guid.NewGuid(),
            Status = StatusTela.Ativo,
            DataVencimentoTecnico = hoje.AddDays(3)
        };
        var telaValida = new TelaCliente
        {
            Id = Guid.NewGuid(),
            Status = StatusTela.Vencido,
            DataVencimentoTecnico = hoje.AddDays(4)
        };
        var service = new TelaClienteService(
            new TelaClienteRepositoryFake(telaVencida, telaSuspensa, telaCancelada, telaVencendo, telaValida),
            new RenovacaoTelaHistoricoRepositoryFake(),
            new ClienteRepositoryFake(),
            new ServidorRepositoryFake());

        var result = await service.GetAllAsync(null, null, CancellationToken.None);

        Assert.True(result.Success);
        var telas = result.Data!.ToDictionary(tela => tela.Id);
        Assert.Equal(StatusTela.Ativo, telas[telaVencida.Id].Status);
        Assert.Equal(StatusTela.Vencido, telas[telaVencida.Id].StatusExibicao);
        Assert.Equal(StatusTela.Suspenso, telas[telaSuspensa.Id].StatusExibicao);
        Assert.Equal(StatusTela.Cancelado, telas[telaCancelada.Id].StatusExibicao);
        Assert.Equal(StatusTela.Vencendo, telas[telaVencendo.Id].StatusExibicao);
        Assert.Equal(StatusTela.Ativo, telas[telaValida.Id].StatusExibicao);
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
    public async Task Cliente_DeveRetornarStatusFinanceiroDoMesAtualComPrioridade()
    {
        var hoje = DateTime.UtcNow.Date;
        var cliente = new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = "Cliente",
            LancamentosFinanceiros =
            [
                new LancamentoFinanceiro
                {
                    Id = Guid.NewGuid(),
                    StatusFinanceiro = StatusFinanceiro.Pago,
                    DataPagamento = hoje,
                    DataVencimentoFinanceiro = hoje
                },
                new LancamentoFinanceiro
                {
                    Id = Guid.NewGuid(),
                    StatusFinanceiro = StatusFinanceiro.Pendente,
                    DataVencimentoFinanceiro = hoje.AddDays(-1)
                }
            ]
        };
        var service = new ClienteService(new ClienteRepositoryFake(cliente));

        var result = await service.GetByIdAsync(cliente.Id, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal("Atrasado", result.Data!.StatusFinanceiroCliente);
    }

    [Fact]
    public async Task Cliente_SemLancamentoNoMesAtual_DeveRetornarSemLancamento()
    {
        var hoje = DateTime.UtcNow.Date;
        var cliente = new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = "Cliente",
            LancamentosFinanceiros =
            [
                new LancamentoFinanceiro
                {
                    Id = Guid.NewGuid(),
                    StatusFinanceiro = StatusFinanceiro.Pago,
                    DataPagamento = hoje.AddMonths(-1),
                    DataVencimentoFinanceiro = hoje.AddMonths(-1)
                }
            ]
        };
        var service = new ClienteService(new ClienteRepositoryFake(cliente));

        var result = await service.GetByIdAsync(cliente.Id, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal("Sem lançamento", result.Data!.StatusFinanceiroCliente);
    }

    [Fact]
    public async Task Financeiro_DeveFiltrarLancamentosPorMesEAnoDoVencimento()
    {
        var clienteId = Guid.NewGuid();
        var repository = new LancamentoFinanceiroRepositoryFake(
            new LancamentoFinanceiro
            {
                Id = Guid.NewGuid(),
                ClienteId = clienteId,
                DataVencimentoFinanceiro = new DateTime(2026, 4, 10),
                StatusFinanceiro = StatusFinanceiro.Pendente
            },
            new LancamentoFinanceiro
            {
                Id = Guid.NewGuid(),
                ClienteId = clienteId,
                DataVencimentoFinanceiro = new DateTime(2026, 5, 10),
                StatusFinanceiro = StatusFinanceiro.Pendente
            });
        var service = new LancamentoFinanceiroService(
            repository,
            new ClienteRepositoryFake(new Cliente { Id = clienteId, Nome = "Cliente" }));

        var result = await service.GetAllAsync(null, null, null, 4, 2026, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Single(result.Data!);
        Assert.Equal(new DateTime(2026, 4, 10), result.Data!.Single().DataVencimentoFinanceiro);
    }

    [Fact]
    public async Task Financeiro_DeveRetornarStatusFinanceiroExibicaoCalculadoSemAlterarStatusSalvo()
    {
        var hoje = DateTime.UtcNow.Date;
        var pendenteVencido = new LancamentoFinanceiro
        {
            Id = Guid.NewGuid(),
            StatusFinanceiro = StatusFinanceiro.Pendente,
            DataVencimentoFinanceiro = hoje.AddDays(-1)
        };
        var pendenteHoje = new LancamentoFinanceiro
        {
            Id = Guid.NewGuid(),
            StatusFinanceiro = StatusFinanceiro.Pendente,
            DataVencimentoFinanceiro = hoje
        };
        var pendenteFuturo = new LancamentoFinanceiro
        {
            Id = Guid.NewGuid(),
            StatusFinanceiro = StatusFinanceiro.Pendente,
            DataVencimentoFinanceiro = hoje.AddDays(1)
        };
        var pagoVencido = new LancamentoFinanceiro
        {
            Id = Guid.NewGuid(),
            StatusFinanceiro = StatusFinanceiro.Pago,
            DataPagamento = hoje,
            DataVencimentoFinanceiro = hoje.AddDays(-1)
        };
        var canceladoVencido = new LancamentoFinanceiro
        {
            Id = Guid.NewGuid(),
            StatusFinanceiro = StatusFinanceiro.Cancelado,
            DataVencimentoFinanceiro = hoje.AddDays(-1)
        };
        var service = new LancamentoFinanceiroService(
            new LancamentoFinanceiroRepositoryFake(pendenteVencido, pendenteHoje, pendenteFuturo, pagoVencido, canceladoVencido),
            new ClienteRepositoryFake());

        var result = await service.GetAllAsync(null, null, null, null, null, CancellationToken.None);

        Assert.True(result.Success);
        var lancamentos = result.Data!.ToDictionary(lancamento => lancamento.Id);
        Assert.Equal(StatusFinanceiro.Pendente, lancamentos[pendenteVencido.Id].StatusFinanceiro);
        Assert.Equal(StatusFinanceiro.Atrasado, lancamentos[pendenteVencido.Id].StatusFinanceiroExibicao);
        Assert.Equal(StatusFinanceiro.Pendente, lancamentos[pendenteHoje.Id].StatusFinanceiroExibicao);
        Assert.Equal(StatusFinanceiro.Pendente, lancamentos[pendenteFuturo.Id].StatusFinanceiroExibicao);
        Assert.Equal(StatusFinanceiro.Pago, lancamentos[pagoVencido.Id].StatusFinanceiroExibicao);
        Assert.Equal(StatusFinanceiro.Cancelado, lancamentos[canceladoVencido.Id].StatusFinanceiroExibicao);
    }

    [Fact]
    public async Task Financeiro_PendentesNaoDevemIncluirVencidosCalculadosComoAtrasado()
    {
        var hoje = DateTime.UtcNow.Date;
        var vencido = new LancamentoFinanceiro
        {
            Id = Guid.NewGuid(),
            StatusFinanceiro = StatusFinanceiro.Pendente,
            DataVencimentoFinanceiro = hoje.AddDays(-1)
        };
        var pendente = new LancamentoFinanceiro
        {
            Id = Guid.NewGuid(),
            StatusFinanceiro = StatusFinanceiro.Pendente,
            DataVencimentoFinanceiro = hoje
        };
        var service = new LancamentoFinanceiroService(
            new LancamentoFinanceiroRepositoryFake(vencido, pendente),
            new ClienteRepositoryFake());

        var result = await service.GetPendentesAsync(CancellationToken.None);

        Assert.True(result.Success);
        var lancamento = Assert.Single(result.Data!);
        Assert.Equal(pendente.Id, lancamento.Id);
        Assert.Equal(StatusFinanceiro.Pendente, lancamento.StatusFinanceiroExibicao);
    }

    [Fact]
    public async Task Financeiro_AtrasadosDevemIncluirPendentesVencidos()
    {
        var hoje = DateTime.UtcNow.Date;
        var vencido = new LancamentoFinanceiro
        {
            Id = Guid.NewGuid(),
            StatusFinanceiro = StatusFinanceiro.Pendente,
            DataVencimentoFinanceiro = hoje.AddDays(-1)
        };
        var service = new LancamentoFinanceiroService(
            new LancamentoFinanceiroRepositoryFake(vencido),
            new ClienteRepositoryFake());

        var result = await service.GetAtrasadosAsync(CancellationToken.None);

        Assert.True(result.Success);
        var lancamento = Assert.Single(result.Data!);
        Assert.Equal(vencido.Id, lancamento.Id);
        Assert.Equal(StatusFinanceiro.Atrasado, lancamento.StatusFinanceiroExibicao);
    }

    [Fact]
    public async Task Financeiro_GeracaoPendentes_DeveGerarQuandoFaltamMenosDeCincoDias()
    {
        var clienteId = Guid.NewGuid();
        var cliente = CreateClienteParaGeracaoPendente(clienteId, diaPagamentoPreferido: 1);
        var service = new LancamentoFinanceiroService(
            new LancamentoFinanceiroRepositoryFake(),
            new ClienteRepositoryFake(cliente));

        var result = await service.GerarPendentesAsync(
            new GerarLancamentosFinanceirosRequest { DataReferencia = new DateTime(2026, 4, 29) },
            CancellationToken.None);

        Assert.True(result.Success);
        var lancamento = Assert.Single(result.Data!);
        Assert.Equal(clienteId, lancamento.ClienteId);
        Assert.Equal(new DateTime(2026, 5, 1), lancamento.DataVencimentoFinanceiro);
        Assert.Equal(new DateTime(2026, 5, 1), lancamento.CompetenciaReferencia);
        Assert.Equal(StatusFinanceiro.Pendente, lancamento.StatusFinanceiro);
        Assert.Equal(100, lancamento.Valor);
        Assert.Null(lancamento.TelaClienteId);
    }

    [Fact]
    public async Task Financeiro_GeracaoPendentes_DeveGerarQuandoFaltamExatamenteCincoDiasComValorAgrupado()
    {
        var clienteId = Guid.NewGuid();
        var dataReferencia = new DateTime(2026, 4, 25);
        var cliente = CreateClienteParaGeracaoPendente(clienteId, diaPagamentoPreferido: 30);
        var service = new LancamentoFinanceiroService(
            new LancamentoFinanceiroRepositoryFake(),
            new ClienteRepositoryFake(cliente));

        var result = await service.GerarPendentesAsync(
            new GerarLancamentosFinanceirosRequest { DataReferencia = dataReferencia },
            CancellationToken.None);

        Assert.True(result.Success);
        var lancamento = Assert.Single(result.Data!);
        Assert.Equal(clienteId, lancamento.ClienteId);
        Assert.Equal(new DateTime(2026, 4, 30), lancamento.DataVencimentoFinanceiro);
        Assert.Equal(new DateTime(2026, 4, 1), lancamento.CompetenciaReferencia);
        Assert.Equal(StatusFinanceiro.Pendente, lancamento.StatusFinanceiro);
        Assert.Equal(100, lancamento.Valor);
        Assert.Null(lancamento.TelaClienteId);
    }

    [Fact]
    public async Task Financeiro_GeracaoPendentes_NaoDeveGerarQuandoFaltamMaisDeCincoDias()
    {
        var cliente = CreateClienteParaGeracaoPendente(Guid.NewGuid(), diaPagamentoPreferido: 30);
        var service = new LancamentoFinanceiroService(
            new LancamentoFinanceiroRepositoryFake(),
            new ClienteRepositoryFake(cliente));

        var result = await service.GerarPendentesAsync(
            new GerarLancamentosFinanceirosRequest { DataReferencia = new DateTime(2026, 4, 24) },
            CancellationToken.None);

        Assert.True(result.Success);
        Assert.Empty(result.Data!);
    }

    [Fact]
    public async Task Financeiro_GeracaoPendentes_DeveUsarProximoMesQuandoDiaPreferidoJaPassou()
    {
        var clienteId = Guid.NewGuid();
        var cliente = CreateClienteParaGeracaoPendente(clienteId, diaPagamentoPreferido: 1);
        var service = new LancamentoFinanceiroService(
            new LancamentoFinanceiroRepositoryFake(),
            new ClienteRepositoryFake(cliente));

        var result = await service.GerarPendentesAsync(
            new GerarLancamentosFinanceirosRequest { DataReferencia = new DateTime(2026, 4, 29) },
            CancellationToken.None);

        Assert.True(result.Success);
        var lancamento = Assert.Single(result.Data!);
        Assert.Equal(new DateTime(2026, 5, 1), lancamento.DataVencimentoFinanceiro);
    }

    [Fact]
    public async Task Financeiro_GeracaoPendentes_NaoDeveDuplicarClienteEVencimento()
    {
        var clienteId = Guid.NewGuid();
        var dataVencimento = new DateTime(2026, 4, 30);
        var repository = new LancamentoFinanceiroRepositoryFake(
            new LancamentoFinanceiro
            {
                Id = Guid.NewGuid(),
                ClienteId = clienteId,
                DataVencimentoFinanceiro = dataVencimento,
                StatusFinanceiro = StatusFinanceiro.Pendente
            });
        var cliente = new Cliente
        {
            Id = clienteId,
            Nome = "Cliente",
            Ativo = true,
            DiaPagamentoPreferido = 30,
            Telas =
            [
                new TelaCliente { Id = Guid.NewGuid(), ValorAcordado = 50, Ativo = true }
            ]
        };
        var service = new LancamentoFinanceiroService(repository, new ClienteRepositoryFake(cliente));

        var result = await service.GerarPendentesAsync(
            new GerarLancamentosFinanceirosRequest { DataReferencia = new DateTime(2026, 4, 25) },
            CancellationToken.None);

        Assert.True(result.Success);
        Assert.Empty(result.Data!);
    }

    [Theory]
    [InlineData(29)]
    [InlineData(30)]
    [InlineData(31)]
    public async Task Financeiro_GeracaoPendentes_DeveUsarUltimoDiaValidoEmMesesCurtos(int diaPagamentoPreferido)
    {
        var clienteId = Guid.NewGuid();
        var cliente = CreateClienteParaGeracaoPendente(clienteId, diaPagamentoPreferido);
        var service = new LancamentoFinanceiroService(
            new LancamentoFinanceiroRepositoryFake(),
            new ClienteRepositoryFake(cliente));

        var result = await service.GerarPendentesAsync(
            new GerarLancamentosFinanceirosRequest { DataReferencia = new DateTime(2026, 2, 25) },
            CancellationToken.None);

        Assert.True(result.Success);
        var lancamento = Assert.Single(result.Data!);
        Assert.Equal(new DateTime(2026, 2, 28), lancamento.DataVencimentoFinanceiro);
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
    public async Task Dashboard_NaoDeveDuplicarLancamentoVencidoEmPendentesEAtrasados()
    {
        var hoje = DateTime.UtcNow.Date;
        var repository = new DashboardRepositoryFake(lancamentos:
        [
            new LancamentoFinanceiro
            {
                Id = Guid.NewGuid(),
                StatusFinanceiro = StatusFinanceiro.Pendente,
                DataVencimentoFinanceiro = hoje.AddDays(-1)
            },
            new LancamentoFinanceiro
            {
                Id = Guid.NewGuid(),
                StatusFinanceiro = StatusFinanceiro.Pendente,
                DataVencimentoFinanceiro = hoje
            }
        ]);
        var service = new DashboardService(repository);

        var result = await service.GetResumoAsync(CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(1, result.Data!.TotalLancamentosPendentes);
        Assert.Equal(1, result.Data.TotalLancamentosAtrasados);
    }

    [Fact]
    public async Task Dashboard_NaoDeveExecutarConsultasEmParaleloNoMesmoRepositorio()
    {
        var service = new DashboardService(new DashboardRepositoryConcurrencyFake());

        var result = await service.GetResumoAsync(CancellationToken.None);

        Assert.True(result.Success);
    }

    private static Cliente CreateClienteParaGeracaoPendente(Guid clienteId, int diaPagamentoPreferido)
    {
        return new Cliente
        {
            Id = clienteId,
            Nome = "Cliente",
            Ativo = true,
            DiaPagamentoPreferido = diaPagamentoPreferido,
            Telas =
            [
                new TelaCliente { Id = Guid.NewGuid(), ValorAcordado = 45, Ativo = true },
                new TelaCliente { Id = Guid.NewGuid(), ValorAcordado = 55, Ativo = true },
                new TelaCliente { Id = Guid.NewGuid(), ValorAcordado = 90, Ativo = false }
            ]
        };
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
            int? mes,
            int? ano,
            CancellationToken cancellationToken)
        {
            IEnumerable<LancamentoFinanceiro> query = _lancamentos;

            if (clienteId is not null)
            {
                query = query.Where(lancamento => lancamento.ClienteId == clienteId.Value);
            }

            if (telaClienteId is not null)
            {
                query = query.Where(lancamento => lancamento.TelaClienteId == telaClienteId.Value);
            }

            if (statusFinanceiro is not null)
            {
                query = query.Where(lancamento => lancamento.StatusFinanceiro == statusFinanceiro.Value);
            }

            if (mes is not null)
            {
                query = query.Where(lancamento => lancamento.DataVencimentoFinanceiro.Month == mes.Value);
            }

            if (ano is not null)
            {
                query = query.Where(lancamento => lancamento.DataVencimentoFinanceiro.Year == ano.Value);
            }

            return Task.FromResult<IReadOnlyCollection<LancamentoFinanceiro>>(query.ToArray());
        }

        public Task<IReadOnlyCollection<LancamentoFinanceiro>> GetPendentesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyCollection<LancamentoFinanceiro>>(
                _lancamentos.Where(lancamento => lancamento.StatusFinanceiro == StatusFinanceiro.Pendente).ToArray());
        }

        public Task<IReadOnlyCollection<LancamentoFinanceiro>> GetAtrasadosAsync(DateTime hoje, CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyCollection<LancamentoFinanceiro>>(
                _lancamentos.Where(lancamento =>
                    lancamento.StatusFinanceiro == StatusFinanceiro.Atrasado ||
                    (lancamento.StatusFinanceiro == StatusFinanceiro.Pendente &&
                        lancamento.DataPagamento is null &&
                        lancamento.DataVencimentoFinanceiro.Date < hoje)).ToArray());
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
