import { useEffect, useMemo, useState } from 'react'
import { getClientes } from '../../api/clientesApi'
import {
  createLancamentoFinanceiro,
  deleteLancamentoFinanceiro,
  getLancamentoFinanceiroById,
  getLancamentosAtrasados,
  getLancamentosFinanceiros,
  getLancamentosPendentes,
  marcarLancamentoPago,
  updateLancamentoFinanceiro,
} from '../../api/financeiroApi'
import { getTelas } from '../../api/telasApi'

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

const currencyFormatter = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
})

const emptyForm = {
  clienteId: '',
  telaClienteId: '',
  competenciaReferencia: '',
  descricao: '',
  valor: '',
  dataVencimentoFinanceiro: '',
  statusFinanceiro: '1',
  observacao: '',
}

function toDateInputValue(value) {
  if (!value) {
    return ''
  }

  return value.slice(0, 10)
}

function formatDate(value) {
  if (!value) {
    return '-'
  }

  return new Intl.DateTimeFormat('pt-BR').format(new Date(value))
}

function getStatusOption(status) {
  const numericStatus = Number(status)

  return statusOptions.find((option) => option.value === numericStatus)
}

function getStatusLabel(status) {
  return getStatusOption(status)?.label ?? 'Desconhecido'
}

function getStatusBadge(status) {
  return getStatusOption(status)?.badge ?? 'text-bg-secondary'
}

function getNameById(items, id) {
  return items.find((item) => item.id === id)?.nome ?? '-'
}

function getTelaNameById(telas, id) {
  return telas.find((tela) => tela.id === id)?.nomeIdentificacao ?? '-'
}

function toFormData(lancamento) {
  return {
    clienteId: lancamento?.clienteId ?? '',
    telaClienteId: lancamento?.telaClienteId ?? '',
    competenciaReferencia: toDateInputValue(lancamento?.competenciaReferencia),
    descricao: lancamento?.descricao ?? '',
    valor: lancamento?.valor === undefined ? '' : String(lancamento.valor),
    dataVencimentoFinanceiro: toDateInputValue(lancamento?.dataVencimentoFinanceiro),
    statusFinanceiro: String(lancamento?.statusFinanceiro ?? 1),
    observacao: lancamento?.observacao ?? '',
  }
}

function buildPayload(formData) {
  return {
    clienteId: formData.clienteId || null,
    telaClienteId: formData.telaClienteId || null,
    competenciaReferencia: formData.competenciaReferencia || null,
    descricao: formData.descricao.trim(),
    valor: Number(formData.valor),
    dataVencimentoFinanceiro: formData.dataVencimentoFinanceiro || null,
    statusFinanceiro: Number(formData.statusFinanceiro),
    observacao: formData.observacao.trim() || null,
  }
}

function getValidationError(formData) {
  const valor = Number(formData.valor)
  const statusFinanceiro = Number(formData.statusFinanceiro)

  if (!formData.clienteId) {
    return 'ClienteId e obrigatorio.'
  }

  if (!formData.telaClienteId) {
    return 'TelaClienteId e obrigatorio.'
  }

  if (!formData.competenciaReferencia) {
    return 'CompetenciaReferencia e obrigatoria.'
  }

  if (!Number.isFinite(valor) || valor <= 0) {
    return 'Valor deve ser maior que zero.'
  }

  if (!formData.dataVencimentoFinanceiro) {
    return 'DataVencimentoFinanceiro e obrigatoria.'
  }

  if (!statusOptions.some((option) => option.value === statusFinanceiro)) {
    return 'StatusFinanceiro deve ser valido.'
  }

  return ''
}

function FinanceiroPage() {
  const [lancamentos, setLancamentos] = useState([])
  const [clientes, setClientes] = useState([])
  const [telas, setTelas] = useState([])
  const [selectedLancamento, setSelectedLancamento] = useState(null)
  const [formData, setFormData] = useState(emptyForm)
  const [filters, setFilters] = useState({ clienteId: '', telaClienteId: '', statusFinanceiro: '' })
  const [viewMode, setViewMode] = useState('todos')
  const [mode, setMode] = useState('create')
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)
  const [isDeleting, setIsDeleting] = useState(false)
  const [isMarkingPaid, setIsMarkingPaid] = useState(false)
  const [error, setError] = useState('')
  const [message, setMessage] = useState('')

  const filteredFormTelas = useMemo(() => {
    if (!formData.clienteId) {
      return telas
    }

    return telas.filter((tela) => tela.clienteId === formData.clienteId)
  }, [formData.clienteId, telas])

  const filteredFilterTelas = useMemo(() => {
    if (!filters.clienteId) {
      return telas
    }

    return telas.filter((tela) => tela.clienteId === filters.clienteId)
  }, [filters.clienteId, telas])

  const sortedLancamentos = useMemo(
    () =>
      [...lancamentos].sort(
        (a, b) => new Date(b.dataVencimentoFinanceiro) - new Date(a.dataVencimentoFinanceiro),
      ),
    [lancamentos],
  )

  async function loadReferences() {
    const [clientesData, telasData] = await Promise.all([getClientes(), getTelas()])

    setClientes(clientesData)
    setTelas(telasData)
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

    setFormData((current) => {
      const nextForm = {
        ...current,
        [name]: value,
      }

      if (name === 'clienteId' && current.clienteId !== value) {
        nextForm.telaClienteId = ''
      }

      return nextForm
    })
  }

  function handleFilterChange(event) {
    const { name, value } = event.target

    setFilters((current) => {
      const nextFilters = {
        ...current,
        [name]: value,
      }

      if (name === 'clienteId' && current.clienteId !== value) {
        nextFilters.telaClienteId = ''
      }

      return nextFilters
    })
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
    const emptyFilters = { clienteId: '', telaClienteId: '', statusFinanceiro: '' }

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
      setMessage(isEdit ? 'Lancamento atualizado com sucesso.' : 'Lancamento cadastrado com sucesso.')
    } catch (apiError) {
      setError(apiError.message)
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
      setMessage('Lancamento excluido com sucesso.')
    } catch (apiError) {
      setError(apiError.message)
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
      setMessage('Lancamento marcado como pago com sucesso.')
    } catch (apiError) {
      setError(apiError.message)
    } finally {
      setIsMarkingPaid(false)
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
          <p className="text-muted mb-0">Lancamentos financeiros manuais.</p>
        </div>
        <div>
          <button className="btn btn-primary" type="button" onClick={handleCreateMode}>
            Novo lancamento
          </button>
        </div>
      </div>

      {message && <div className="alert alert-success">{message}</div>}
      {error && <div className="alert alert-danger">{error}</div>}

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
              <label className="form-label" htmlFor="filtroTelaCliente">
                Tela
              </label>
              <select
                className="form-select"
                id="filtroTelaCliente"
                name="telaClienteId"
                value={filters.telaClienteId}
                onChange={handleFilterChange}
              >
                <option value="">Todas</option>
                {filteredFilterTelas.map((tela) => (
                  <option key={tela.id} value={tela.id}>
                    {tela.nomeIdentificacao}
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

            <div className="col-12 col-md-3 d-flex gap-2">
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
                    <th>Tela</th>
                    <th>Status</th>
                    <th>Vencimento</th>
                    <th className="text-end">Valor</th>
                    <th className="text-end">Acoes</th>
                  </tr>
                </thead>
                <tbody>
                  {isLoading ? (
                    <tr>
                      <td className="text-muted text-center" colSpan="7">
                        Carregando lancamentos...
                      </td>
                    </tr>
                  ) : sortedLancamentos.length === 0 ? (
                    <tr>
                      <td className="text-muted text-center" colSpan="7">
                        Nenhum lancamento encontrado.
                      </td>
                    </tr>
                  ) : (
                    sortedLancamentos.map((lancamento) => (
                      <tr key={lancamento.id}>
                        <td>
                          <div>{lancamento.descricao || '-'}</div>
                          <div className="text-muted small">{formatDate(lancamento.competenciaReferencia)}</div>
                        </td>
                        <td>{getNameById(clientes, lancamento.clienteId)}</td>
                        <td>{getTelaNameById(telas, lancamento.telaClienteId)}</td>
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
                <div className="row g-3">
                  <div className="col-12 col-md-6">
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

                  <div className="col-12 col-md-6">
                    <label className="form-label" htmlFor="telaClienteId">
                      Tela
                    </label>
                    <select
                      className="form-select"
                      id="telaClienteId"
                      name="telaClienteId"
                      value={formData.telaClienteId}
                      onChange={handleChange}
                      disabled={isFormReadonly}
                      required
                    >
                      <option value="">Selecione</option>
                      {filteredFormTelas.map((tela) => (
                        <option key={tela.id} value={tela.id}>
                          {tela.nomeIdentificacao}
                        </option>
                      ))}
                    </select>
                  </div>
                </div>

                <div className="mb-3 mt-3">
                  <label className="form-label" htmlFor="descricao">
                    Descricao
                  </label>
                  <input
                    className="form-control"
                    id="descricao"
                    name="descricao"
                    type="text"
                    value={formData.descricao}
                    onChange={handleChange}
                    disabled={isFormReadonly}
                  />
                </div>

                <div className="row g-3">
                  <div className="col-12 col-md-6">
                    <label className="form-label" htmlFor="competenciaReferencia">
                      Competencia
                    </label>
                    <input
                      className="form-control"
                      id="competenciaReferencia"
                      name="competenciaReferencia"
                      type="date"
                      value={formData.competenciaReferencia}
                      onChange={handleChange}
                      disabled={isFormReadonly}
                      required
                    />
                  </div>

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
                </div>

                <div className="row g-3 mt-0">
                  <div className="col-12 col-md-6">
                    <label className="form-label" htmlFor="valor">
                      Valor
                    </label>
                    <input
                      className="form-control"
                      id="valor"
                      name="valor"
                      type="number"
                      min="0.01"
                      step="0.01"
                      value={formData.valor}
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
                    rows="3"
                    value={formData.observacao}
                    onChange={handleChange}
                    disabled={isFormReadonly}
                  />
                </div>

                {mode !== 'view' && (
                  <div className="d-flex gap-2">
                    <button className="btn btn-primary" type="submit" disabled={isSaving}>
                      {isSaving ? 'Salvando...' : 'Salvar'}
                    </button>
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
