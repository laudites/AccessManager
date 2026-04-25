function HistoricoPage() {
  return (
    <section>
      <div className="d-flex flex-column flex-md-row justify-content-between gap-2 mb-4">
        <div>
          <h1>Historico</h1>
          <p className="text-muted mb-0">Consulta de historico tecnico.</p>
        </div>
      </div>

      <div className="alert alert-info">
        O backend registra historico tecnico internamente durante renovacoes e trocas de servidor, mas ainda nao expoe
        endpoint para consulta desses registros.
      </div>

      <div className="card">
        <div className="card-header bg-white">
          <strong>Endpoint futuro esperado</strong>
        </div>
        <div className="card-body">
          <p className="mb-3">
            Para habilitar a listagem nesta tela, o backend precisa expor uma rota real retornando
            <code className="mx-1">RenovacaoTelaHistorico</code> em um envelope
            <code className="ms-1">ApiResponse&lt;T&gt;</code>.
          </p>

          <div className="table-responsive">
            <table className="table table-sm mb-0">
              <thead>
                <tr>
                  <th>Rota sugerida</th>
                  <th>Uso</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td>
                    <code>GET /api/historico-tecnico</code>
                  </td>
                  <td>Listar historico tecnico.</td>
                </tr>
                <tr>
                  <td>
                    <code>GET /api/historico-tecnico?clienteId=&#123;id&#125;</code>
                  </td>
                  <td>Filtrar por cliente.</td>
                </tr>
                <tr>
                  <td>
                    <code>GET /api/historico-tecnico?telaClienteId=&#123;id&#125;</code>
                  </td>
                  <td>Filtrar por tela.</td>
                </tr>
              </tbody>
            </table>
          </div>

          <p className="text-muted small mb-0 mt-3">
            Nenhum dado mockado ou consumo de endpoint inexistente foi adicionado.
          </p>
        </div>
      </div>
    </section>
  )
}

export default HistoricoPage
