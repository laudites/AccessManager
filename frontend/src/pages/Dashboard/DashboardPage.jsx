import { useEffect, useState } from 'react'
import { getDashboardResumo, getTelasPorServidor } from '../../api/dashboardApi'
import FeedbackAlert from '../../components/FeedbackAlert'

const currencyFormatter = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
})

const metricConfig = [
  { key: 'rendimentoMensal', label: 'Rendimento mensal', isCurrency: true },
  { key: 'custoMensal', label: 'Custo mensal', isCurrency: true },
  { key: 'totalClientes', label: 'Total de clientes' },
  { key: 'totalClientesPagosMes', label: 'Clientes pagos no mes' },
  { key: 'totalTelasAtivas', label: 'Telas ativas' },
  { key: 'totalTelasVencendo', label: 'Telas vencendo' },
  { key: 'totalTelasVencidas', label: 'Telas vencidas' },
  { key: 'totalLancamentosPendentes', label: 'Lancamentos pendentes' },
  { key: 'totalLancamentosAtrasados', label: 'Lancamentos atrasados' },
  { key: 'totalEmAberto', label: 'Total em aberto', isCurrency: true },
]

function DashboardPage() {
  const [resumo, setResumo] = useState(null)
  const [telasPorServidor, setTelasPorServidor] = useState([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    async function loadDashboard() {
      try {
        setIsLoading(true)
        setError('')

        const [resumoData, telasPorServidorData] = await Promise.all([
          getDashboardResumo(),
          getTelasPorServidor(),
        ])

        setResumo(resumoData)
        setTelasPorServidor(telasPorServidorData ?? [])
      } catch {
        setError('Nao foi possivel carregar os dados do dashboard.')
      } finally {
        setIsLoading(false)
      }
    }

    loadDashboard()
  }, [])

  if (isLoading) {
    return (
      <section>
        <h1>Dashboard</h1>
        <FeedbackAlert message="Carregando dados..." type="info" />
      </section>
    )
  }

  if (error) {
    return (
      <section>
        <h1>Dashboard</h1>
        <FeedbackAlert message={error} type="danger" />
      </section>
    )
  }

  return (
    <section>
      <div className="d-flex flex-column flex-md-row justify-content-between gap-2 mb-4">
        <div>
          <h1>Dashboard</h1>
          <p className="text-muted mb-0">Resumo tecnico e financeiro.</p>
        </div>
      </div>

      <div className="row g-3 mb-4">
        {metricConfig.map((metric) => (
          <div className="col-12 col-sm-6 col-xl-3" key={metric.key}>
            <div className="card dashboard-card h-100">
              <div className="card-body">
                <div className="text-muted small mb-2">{metric.label}</div>
                <div className="dashboard-metric">
                  {metric.isCurrency
                    ? currencyFormatter.format(resumo?.[metric.key] ?? 0)
                    : (resumo?.[metric.key] ?? 0)}
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>

      <div className="row g-3">
        <div className="col-12 col-xl-8">
          <div className="card h-100">
            <div className="card-header bg-white">
              <strong>Creditos e telas por servidor</strong>
            </div>
            <div className="table-responsive">
              <table className="table table-striped table-hover mb-0">
                <thead>
                  <tr>
                    <th>Servidor</th>
                    <th className="text-end">Creditos</th>
                    <th className="text-end">Clientes</th>
                    <th className="text-end">Telas</th>
                    <th className="text-end">Custo estimado</th>
                  </tr>
                </thead>
                <tbody>
                  {telasPorServidor.length === 0 ? (
                    <tr>
                      <td className="text-muted text-center" colSpan="5">
                        Nenhum registro encontrado.
                      </td>
                    </tr>
                  ) : (
                    telasPorServidor.map((item) => (
                      <tr key={item.servidorId}>
                        <td>
                          <div>{item.nomeServidor}</div>
                          <div className="text-muted small">
                            Custo credito {currencyFormatter.format(item.valorCustoCredito ?? 0)}
                          </div>
                        </td>
                        <td className="text-end">{item.quantidadeCreditos ?? 0}</td>
                        <td className="text-end">{item.quantidadeClientes ?? 0}</td>
                        <td className="text-end">{item.quantidadeTelas ?? 0}</td>
                        <td className="text-end">{currencyFormatter.format(item.custoEstimado ?? 0)}</td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <div className="col-12 col-xl-4">
          <div className="card h-100">
            <div className="card-header bg-white">
              <strong>Clientes pendentes</strong>
            </div>
            <div className="list-group list-group-flush">
              {(resumo?.clientesPendentes ?? []).length === 0 ? (
                <div className="list-group-item text-muted">Nenhuma pendencia financeira.</div>
              ) : (
                resumo.clientesPendentes.map((cliente) => (
                  <div className="list-group-item" key={cliente.clienteId}>
                    <div className="d-flex justify-content-between gap-2">
                      <strong>{cliente.nomeCliente || 'Cliente'}</strong>
                      <span>{currencyFormatter.format(cliente.valorEmAberto ?? 0)}</span>
                    </div>
                    <div className="text-muted small">
                      {cliente.quantidadeLancamentos} lancamento(s), vence em{' '}
                      {new Intl.DateTimeFormat('pt-BR').format(new Date(cliente.proximoVencimento))}
                    </div>
                  </div>
                ))
              )}
            </div>
          </div>
        </div>
      </div>
    </section>
  )
}

export default DashboardPage
