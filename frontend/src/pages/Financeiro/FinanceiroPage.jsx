import { useEffect, useMemo, useState } from 'react'
import { getClientes } from '../../api/clientesApi'
import {
  createLancamentoFinanceiro,
  deleteLancamentoFinanceiro,
  gerarLancamentosPendentes,
  getLancamentoFinanceiroById,
  getLancamentosAtrasados,
  getLancamentosFinanceiros,
  getLancamentosPendentes,
  marcarLancamentoPago,
  updateLancamentoFinanceiro,
} from '../../api/financeiroApi'
import FeedbackAlert from '../../components/FeedbackAlert'
import LoadingButton from '../../components/LoadingButton'

const statusOptions = [
  { value: 1, label: 'Pendente', badge: 'text-bg-warning' },
  { value: 2, label: 'Pago', badge: 'text-bg-success' },
  { value: 3, label: 'Atrasado', badge: 'text-bg-danger' },
  { value: 4, label: 'Cancelado', badge: 'text-bg-secondary' },
]

const viewOptions = [
  { key: 'todos', label: 'Todos' },
  { key: 'pendentes', label: 'Pendentes' },
  { key: 'atrasados', label: 'Atrasados' },
]

const monthOptions = [
  { value: 1, label: 'Janeiro' },
  { value: 2, label: 'Fevereiro' },
  { value: 3, label: 'Marco' },
  { value: 4, label: 'Abril' },
  { value: 5, label: 'Maio' },
  { value: 6, label: 'Junho' },
  { value: 7, label: 'Julho' },
  { value: 8, label: 'Agosto' },
  { value: 9, label: 'Setembro' },
  { value: 10, label: 'Outubro' },
  { value: 11, label: 'Novembro' },
  { value: 12, label: 'Dezembro' },
]

const currencyFormatter = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
})

const emptyForm = {
  clienteId: '',
  descricao: '',
  dataVencimentoFinanceiro: '',
  statusFinanceiro: '1',
  observacao: '',
}

const currentDate = new Date()
const emptyFilters = {
  clienteId: '',
  statusFinanceiro: '',
  mes: String(currentDate.getMonth() + 1),
  ano: String(currentDate.getFullYear()),
}

function toDateInputValue(value) {
  return value ? value.slice(0, 10) : ''
}

function formatDate(value) {
  return value ? new Intl.DateTimeFormat('pt-BR').format(new Date(value)) : '-'
}

function getStatusOption(status) {
  return statusOptions.find((option) => option.value === Number(status))
}

function getStatusLabel(status) {
  return getStatusOption(status)?.label ?? 'Desconhecido'
}

function getStatusBadge(status) {
  return getStatusOption(status)?.badge ?? 'text-bg-secondary'
}

function getClienteById(clientes, id) {
  return clientes.find((cliente) => cliente.id === id)
}

function toFormData(lancamento) {
  return {
    clienteId: lancamento?.clienteId ?? '',
    descricao: lancamento?.descricao ?? '',
    dataVencimentoFinanceiro: toDateInputValue(lancamento?.dataVencimentoFinanceiro),
    statusFinanceiro: String(lancamento?.statusFinanceiro ?? 1),
    observacao: lancamento?.observacao ?? '',
  }
}

function buildPayload(formData) {
  return {
    clienteId: formData.clienteId || null,
    descricao: formData.descricao.trim(),
    dataVencimentoFinanceiro: formData.dataVencimentoFinanceiro || null,
    statusFinanceiro: Number(formData.statusFinanceiro),
    observacao: formData.observacao.trim() || null,
  }
}

function getValidationError(formData) {
  const statusFinanceiro = Number(formData.statusFinanceiro)

  if (!formData.clienteId) {
    return 'Campo obrigatorio: Cliente.'
  }

  if (!formData.dataVencimentoFinanceiro) {
    return 'Campo obrigatorio: Vencimento financeiro.'
  }

  if (!statusOptions.some((option) => option.value === statusFinanceiro)) {
    return 'StatusFinanceiro deve ser valido.'
  }

  return ''
}

function FinanceiroPage() {
  const [lancamentos, setLancamentos] = useState([])
  const [clientes, setClientes] = useState([])
  const [selectedLancamento, setSelectedLancamento] = useState(null)
  const [formData, setFormData] = useState(emptyForm)
  const [filters, setFilters] = useState(emptyFilters)
  const [viewMode, setViewMode] = useState('todos')
  const [mode, setMode] = useState('create')
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)
  const [isDeleting, setIsDeleting] = useState(false)
  const [isMarkingPaid, setIsMarkingPaid] = useState(false)
  const [isGenerating, setIsGenerating] = useState(false)
  const [error, setError] = useState('')
  const [message, setMessage] = useState('')

  const selectedCliente = useMemo(
    () => getClienteById(clientes, formData.clienteId),
    [clientes, formData.clienteId],
  )

  const sortedLancamentos = useMemo(
    () =>
      [...lancamentos].sort(
        (a, b) => new Date(b.dataVencimentoFinanceiro) - new Date(a.dataVencimentoFinanceiro),
      ),
    [lancamentos],
  )

  async function loadReferences() {
    setClientes(await getClientes())
  }

  async function loadLancamentos(nextViewMode = viewMode, nextFilters = filters) {
    try {
      setIsLoading(true)
      setError('')

      let data
      if (nextViewMode === 'pendentes') {
        data = await getLancamentosPendentes()
      } else if (nextViewMode === 'atrasados') {
        data = await getLancamentosAtrasados()
      } else {
        data = await getLancamentosFinanceiros(nextFilters)
      }

      setLancamentos(data)
    } catch (apiError) {
      setError(apiError.message)
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    async function loadPage() {
      try {
        setIsLoading(true)
        setError('')

        await loadReferences()
        const data = await getLancamentosFinanceiros(filters)
        setLancamentos(data)
      } catch (apiError) {
        setError(apiError.message)
      } finally {
        setIsLoading(false)
      }
    }

    loadPage()
  }, [])

  function handleCreateMode() {
    setMode('create')
    setSelectedLancamento(null)
    setFormData(emptyForm)
    setError('')
    setMessage('')
  }

  async function handleViewLancamento(id) {
    try {
      setError('')
      setMessage('')

      const lancamento = await getLancamentoFinanceiroById(id)

      setSelectedLancamento(lancamento)
      setFormData(toFormData(lancamento))
      setMode('view')
    } catch (apiError) {
      setError(apiError.message)
    }
  }

  function handleEditMode() {
    setMode('edit')
    setFormData(toFormData(selectedLancamento))
    setError('')
    setMessage('')
  }

  function handleChange(event) {
    const { name, value } = event.target

    setFormData((current) => ({
      ...current,
      [name]: value,
    }))
  }

  function handleFilterChange(event) {
    const { name, value } = event.target

    setFilters((current) => ({
      ...current,
      [name]: value,
    }))
  }

  async function handleViewModeChange(nextViewMode) {
    setViewMode(nextViewMode)
    setMessage('')

    await loadLancamentos(nextViewMode, filters)
  }

  async function handleApplyFilters(event) {
    event.preventDefault()
    setViewMode('todos')
    setMessage('')

    await loadLancamentos('todos', filters)
  }

  async function handleClearFilters() {
    setFilters(emptyFilters)
    setViewMode('todos')
    setMessage('')
    await loadLancamentos('todos', emptyFilters)
  }

  async function handleSubmit(event) {
    event.preventDefault()

    const validationError = getValidationError(formData)
    if (validationError) {
      setError(validationError)
      setMessage('')
      return
    }

    try {
      setIsSaving(true)
      setError('')
      setMessage('')

      const isEdit = mode === 'edit'
      const payload = buildPayload(formData)
      const savedLancamento = isEdit
        ? await updateLancamentoFinanceiro(selectedLancamento.id, payload)
        : await createLancamentoFinanceiro(payload)

      await loadLancamentos(viewMode, filters)
      setSelectedLancamento(savedLancamento)
      setFormData(toFormData(savedLancamento))
      setMode('view')
      setMessage('Salvo com sucesso.')
    } catch (apiError) {
      setError(`Erro ao salvar. ${apiError.message}`)
    } finally {
      setIsSaving(false)
    }
  }

  async function handleDeleteLancamento() {
    if (!selectedLancamento) {
      return
    }

    const shouldDelete = window.confirm(`Excluir lancamento "${selectedLancamento.descricao}"?`)
    if (!shouldDelete) {
      return
    }

    try {
      setIsDeleting(true)
      setError('')
      setMessage('')

      await deleteLancamentoFinanceiro(selectedLancamento.id)
      await loadLancamentos(viewMode, filters)
      handleCreateMode()
      setMessage('Excluido com sucesso.')
    } catch (apiError) {
      setError(`Erro ao excluir. ${apiError.message}`)
    } finally {
      setIsDeleting(false)
    }
  }

  async function handleMarcarPago() {
    if (!selectedLancamento) {
      return
    }

    try {
      setIsMarkingPaid(true)
      setError('')
      setMessage('')

      const updatedLancamento = await marcarLancamentoPago(selectedLancamento.id)

      await loadLancamentos(viewMode, filters)
      setSelectedLancamento(updatedLancamento)
      setFormData(toFormData(updatedLancamento))
      setMode('view')
      setMessage('Salvo com sucesso.')
    } catch (apiError) {
      setError(`Erro ao salvar. ${apiError.message}`)
    } finally {
      setIsMarkingPaid(false)
    }
  }

  async function handleGerarPendentes() {
    try {
      setIsGenerating(true)
      setError('')
      setMessage('')

      const gerados = await gerarLancamentosPendentes()

      await loadLancamentos(viewMode, filters)
      setMessage(`${gerados.length} lancamento(s) pendente(s) gerado(s).`)
    } catch (apiError) {
      setError(`Erro ao gerar pendentes. ${apiError.message}`)
    } finally {
      setIsGenerating(false)
    }
  }

  const isFormReadonly = mode === 'view'
  const panelTitle =
    mode === 'edit' ? 'Editar lancamento' : mode === 'view' ? 'Detalhe do lancamento' : 'Novo lancamento'

  return (
    <section>
      <div className="d-flex flex-column flex-md-row justify-content-between gap-2 mb-4">
        <div>
          <h1>Financeiro</h1>
          <p className="text-muted mb-0">Lancamentos financeiros por cliente.</p>
        </div>
        <div className="d-flex gap-2">
          <button
            className="btn btn-outline-primary"
            type="button"
            onClick={handleGerarPendentes}
            disabled={isGenerating}
          >
            {isGenerating ? 'Gerando...' : 'Gerar pendentes'}
          </button>
          <button className="btn btn-primary" type="button" onClick={handleCreateMode}>
            Novo lancamento
          </button>
        </div>
      </div>

      <FeedbackAlert message={message} />
      <FeedbackAlert message={error} type="danger" />

      <div className="btn-group mb-3" role="group" aria-label="Visualizacao financeira">
        {viewOptions.map((option) => (
          <button
            className={`btn ${viewMode === option.key ? 'btn-primary' : 'btn-outline-primary'}`}
            key={option.key}
            type="button"
            onClick={() => handleViewModeChange(option.key)}
            disabled={isLoading}
          >
            {option.label}
          </button>
        ))}
      </div>

      <form className="card mb-3" onSubmit={handleApplyFilters}>
        <div className="card-body">
          <div className="row g-3 align-items-end">
            <div className="col-12 col-md-3">
              <label className="form-label" htmlFor="filtroCliente">
                Cliente
              </label>
              <select
                className="form-select"
                id="filtroCliente"
                name="clienteId"
                value={filters.clienteId}
                onChange={handleFilterChange}
              >
                <option value="">Todos</option>
                {clientes.map((cliente) => (
                  <option key={cliente.id} value={cliente.id}>
                    {cliente.nome}
                  </option>
                ))}
              </select>
            </div>

            <div className="col-12 col-md-3">
              <label className="form-label" htmlFor="filtroStatusFinanceiro">
                Status
              </label>
              <select
                className="form-select"
                id="filtroStatusFinanceiro"
                name="statusFinanceiro"
                value={filters.statusFinanceiro}
                onChange={handleFilterChange}
              >
                <option value="">Todos</option>
                {statusOptions.map((status) => (
                  <option key={status.value} value={status.value}>
                    {status.label}
                  </option>
                ))}
              </select>
            </div>

            <div className="col-6 col-md-2">
              <label className="form-label" htmlFor="filtroMes">
                Mes
              </label>
              <select
                className="form-select"
                id="filtroMes"
                name="mes"
                value={filters.mes}
                onChange={handleFilterChange}
              >
                <option value="">Todos</option>
                {monthOptions.map((month) => (
                  <option key={month.value} value={month.value}>
                    {month.label}
                  </option>
                ))}
              </select>
            </div>

            <div className="col-6 col-md-2">
              <label className="form-label" htmlFor="filtroAno">
                Ano
              </label>
              <input
                className="form-control"
                id="filtroAno"
                min="1"
                name="ano"
                type="number"
                value={filters.ano}
                onChange={handleFilterChange}
              />
            </div>

            <div className="col-12 col-md-2 d-flex gap-2">
              <button className="btn btn-outline-primary w-100" type="submit" disabled={isLoading}>
                Filtrar
              </button>
              <button
                className="btn btn-outline-secondary"
                type="button"
                onClick={handleClearFilters}
                disabled={isLoading}
              >
                Limpar
              </button>
            </div>
          </div>
        </div>
      </form>

      <div className="row g-3">
        <div className="col-12 col-xl-7">
          <div className="card h-100">
            <div className="card-header bg-white">
              <strong>Lista de lancamentos</strong>
            </div>
            <div className="table-responsive">
              <table className="table table-striped table-hover align-middle mb-0">
                <thead>
                  <tr>
                    <th>Descricao</th>
                    <th>Cliente</th>
                    <th>Status</th>
                    <th>Vencimento</th>
                    <th className="text-end">Valor</th>
                    <th className="text-end">Acoes</th>
                  </tr>
                </thead>
                <tbody>
                  {isLoading ? (
                    <tr>
                      <td className="text-muted text-center" colSpan="6">
                        Carregando lancamentos...
                      </td>
                    </tr>
                  ) : sortedLancamentos.length === 0 ? (
                    <tr>
                      <td className="text-muted text-center" colSpan="6">
                        Nenhum lancamento encontrado.
                      </td>
                    </tr>
                  ) : (
                    sortedLancamentos.map((lancamento) => (
                      <tr key={lancamento.id}>
                        <td>
                          <div>{lancamento.descricao || '-'}</div>
                          <div className="text-muted small">Competencia {formatDate(lancamento.competenciaReferencia)}</div>
                        </td>
                        <td>{lancamento.clienteNome || getClienteById(clientes, lancamento.clienteId)?.nome || '-'}</td>
                        <td>
                          <span className={`badge ${getStatusBadge(lancamento.statusFinanceiro)}`}>
                            {getStatusLabel(lancamento.statusFinanceiro)}
                          </span>
                        </td>
                        <td>{formatDate(lancamento.dataVencimentoFinanceiro)}</td>
                        <td className="text-end">{currencyFormatter.format(lancamento.valor ?? 0)}</td>
                        <td className="text-end">
                          <button
                            className="btn btn-sm btn-outline-primary"
                            type="button"
                            onClick={() => handleViewLancamento(lancamento.id)}
                          >
                            Ver
                          </button>
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <div className="col-12 col-xl-5">
          <div className="card h-100">
            <div className="card-header bg-white d-flex justify-content-between align-items-center gap-2">
              <strong>{panelTitle}</strong>
              {mode === 'view' && selectedLancamento && (
                <div className="btn-group btn-group-sm">
                  <button
                    className="btn btn-outline-success"
                    type="button"
                    onClick={handleMarcarPago}
                    disabled={isMarkingPaid || selectedLancamento.statusFinanceiro === 2}
                  >
                    {isMarkingPaid ? 'Marcando...' : 'Marcar pago'}
                  </button>
                  <button className="btn btn-outline-primary" type="button" onClick={handleEditMode}>
                    Editar
                  </button>
                  <button
                    className="btn btn-outline-danger"
                    type="button"
                    onClick={handleDeleteLancamento}
                    disabled={isDeleting}
                  >
                    {isDeleting ? 'Excluindo...' : 'Excluir'}
                  </button>
                </div>
              )}
            </div>
            <div className="card-body">
              {mode === 'view' && selectedLancamento ? (
                <div className="mb-4">
                  <dl className="row mb-0">
                    <dt className="col-sm-5">Competencia</dt>
                    <dd className="col-sm-7">{formatDate(selectedLancamento.competenciaReferencia)}</dd>
                    <dt className="col-sm-5">Data pagamento</dt>
                    <dd className="col-sm-7">{formatDate(selectedLancamento.dataPagamento)}</dd>
                    <dt className="col-sm-5">Data criacao</dt>
                    <dd className="col-sm-7">{formatDate(selectedLancamento.dataCriacao)}</dd>
                    <dt className="col-sm-5">Identificador</dt>
                    <dd className="col-sm-7 text-break">{selectedLancamento.id}</dd>
                  </dl>
                </div>
              ) : null}

              <form onSubmit={handleSubmit}>
                <div className="mb-3">
                  <label className="form-label" htmlFor="clienteId">
                    Cliente
                  </label>
                  <select
                    className="form-select"
                    id="clienteId"
                    name="clienteId"
                    value={formData.clienteId}
                    onChange={handleChange}
                    disabled={isFormReadonly}
                    required
                  >
                    <option value="">Selecione</option>
                    {clientes.map((cliente) => (
                      <option key={cliente.id} value={cliente.id}>
                        {cliente.nome}
                      </option>
                    ))}
                  </select>
                </div>

                <div className="mb-3">
                  <label className="form-label">Valor agrupado das telas ativas</label>
                  <input
                    className="form-control"
                    type="text"
                    value={currencyFormatter.format(
                      mode === 'view'
                        ? selectedLancamento?.valor ?? 0
                        : selectedCliente?.valorTotalTelas ?? 0,
                    )}
                    readOnly
                  />
                </div>

                <div className="mb-3">
                  <label className="form-label" htmlFor="descricao">
                    Descricao
                  </label>
                  <input
                    className="form-control"
                    id="descricao"
                    name="descricao"
                    placeholder="Descricao do lancamento"
                    type="text"
                    value={formData.descricao}
                    onChange={handleChange}
                    disabled={isFormReadonly}
                  />
                </div>

                <div className="row g-3">
                  <div className="col-12 col-md-6">
                    <label className="form-label" htmlFor="dataVencimentoFinanceiro">
                      Vencimento financeiro
                    </label>
                    <input
                      className="form-control"
                      id="dataVencimentoFinanceiro"
                      name="dataVencimentoFinanceiro"
                      type="date"
                      value={formData.dataVencimentoFinanceiro}
                      onChange={handleChange}
                      disabled={isFormReadonly}
                      required
                    />
                  </div>

                  <div className="col-12 col-md-6">
                    <label className="form-label" htmlFor="statusFinanceiro">
                      Status
                    </label>
                    <select
                      className="form-select"
                      id="statusFinanceiro"
                      name="statusFinanceiro"
                      value={formData.statusFinanceiro}
                      onChange={handleChange}
                      disabled={isFormReadonly}
                    >
                      {statusOptions.map((status) => (
                        <option key={status.value} value={status.value}>
                          {status.label}
                        </option>
                      ))}
                    </select>
                  </div>
                </div>

                <div className="mb-3 mt-3">
                  <label className="form-label" htmlFor="observacao">
                    Observacao
                  </label>
                  <textarea
                    className="form-control"
                    id="observacao"
                    name="observacao"
                    placeholder="Observacoes internas"
                    rows="3"
                    value={formData.observacao}
                    onChange={handleChange}
                    disabled={isFormReadonly}
                  />
                </div>

                {mode !== 'view' && (
                  <div className="d-flex gap-2">
                    <LoadingButton isLoading={isSaving} loadingText="Salvando..." type="submit">
                      Salvar
                    </LoadingButton>
                    {mode === 'edit' && (
                      <button
                        className="btn btn-outline-secondary"
                        type="button"
                        onClick={() => {
                          setMode('view')
                          setFormData(toFormData(selectedLancamento))
                          setError('')
                        }}
                      >
                        Cancelar
                      </button>
                    )}
                  </div>
                )}
              </form>
            </div>
          </div>
        </div>
      </div>
    </section>
  )
}

export default FinanceiroPage
