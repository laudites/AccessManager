import { useEffect, useMemo, useState } from 'react'
import { getClientes } from '../../api/clientesApi'
import { getServidores } from '../../api/servidoresApi'
import {
  createTela,
  deleteTela,
  getTelaById,
  getTelas,
  renovarTela,
  trocarServidor,
  updateTela,
} from '../../api/telasApi'

const statusOptions = [
  { value: 1, label: 'Ativo', badge: 'text-bg-success' },
  { value: 2, label: 'Vencendo', badge: 'text-bg-warning' },
  { value: 3, label: 'Vencido', badge: 'text-bg-danger' },
  { value: 4, label: 'Suspenso', badge: 'text-bg-secondary' },
  { value: 5, label: 'Cancelado', badge: 'text-bg-dark' },
]

const currencyFormatter = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
})

const emptyForm = {
  clienteId: '',
  nomeIdentificacao: '',
  servidorId: '',
  usuarioTela: '',
  senhaTela: '',
  valorAcordado: '0',
  dataVencimentoTecnico: '',
  status: '1',
  marcaTv: '',
  appUtilizado: '',
  macOuIdApp: '',
  chaveSecundaria: '',
  observacao: '',
  ativo: true,
}

const emptyRenovacaoForm = {
  novaDataVencimentoTecnico: '',
  valorAcordadoNovo: '',
  observacao: '',
}

const emptyTrocaServidorForm = {
  servidorNovoId: '',
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

function toFormData(tela) {
  return {
    clienteId: tela?.clienteId ?? '',
    nomeIdentificacao: tela?.nomeIdentificacao ?? '',
    servidorId: tela?.servidorId ?? '',
    usuarioTela: tela?.usuarioTela ?? '',
    senhaTela: tela?.senhaTela ?? '',
    valorAcordado: String(tela?.valorAcordado ?? 0),
    dataVencimentoTecnico: toDateInputValue(tela?.dataVencimentoTecnico),
    status: String(tela?.status ?? 1),
    marcaTv: tela?.marcaTv ?? '',
    appUtilizado: tela?.appUtilizado ?? '',
    macOuIdApp: tela?.macOuIdApp ?? '',
    chaveSecundaria: tela?.chaveSecundaria ?? '',
    observacao: tela?.observacao ?? '',
    ativo: tela?.ativo ?? true,
  }
}

function buildPayload(formData, includeAtivo) {
  const payload = {
    clienteId: formData.clienteId || null,
    nomeIdentificacao: formData.nomeIdentificacao.trim(),
    servidorId: formData.servidorId || null,
    usuarioTela: formData.usuarioTela.trim(),
    senhaTela: formData.senhaTela.trim(),
    valorAcordado: Number(formData.valorAcordado),
    dataVencimentoTecnico: formData.dataVencimentoTecnico || null,
    status: Number(formData.status),
    marcaTv: formData.marcaTv.trim(),
    appUtilizado: formData.appUtilizado.trim(),
    macOuIdApp: formData.macOuIdApp.trim() || null,
    chaveSecundaria: formData.chaveSecundaria.trim() || null,
    observacao: formData.observacao.trim() || null,
  }

  if (includeAtivo) {
    payload.ativo = formData.ativo
  }

  return payload
}

function getValidationError(formData) {
  const valorAcordado = Number(formData.valorAcordado)
  const status = Number(formData.status)

  if (!formData.clienteId) {
    return 'ClienteId e obrigatorio.'
  }

  if (!formData.servidorId) {
    return 'ServidorId e obrigatorio.'
  }

  if (!formData.nomeIdentificacao.trim()) {
    return 'NomeIdentificacao e obrigatorio.'
  }

  if (!formData.usuarioTela.trim()) {
    return 'UsuarioTela e obrigatorio.'
  }

  if (!formData.senhaTela.trim()) {
    return 'SenhaTela e obrigatoria.'
  }

  if (!Number.isFinite(valorAcordado) || valorAcordado < 0) {
    return 'ValorAcordado deve ser maior ou igual a zero.'
  }

  if (!formData.dataVencimentoTecnico) {
    return 'DataVencimentoTecnico e obrigatoria.'
  }

  if (!statusOptions.some((option) => option.value === status)) {
    return 'Status deve ser valido.'
  }

  return ''
}

function getNameById(items, id) {
  return items.find((item) => item.id === id)?.nome ?? '-'
}

function TelasPage() {
  const [telas, setTelas] = useState([])
  const [clientes, setClientes] = useState([])
  const [servidores, setServidores] = useState([])
  const [selectedTela, setSelectedTela] = useState(null)
  const [formData, setFormData] = useState(emptyForm)
  const [renovacaoForm, setRenovacaoForm] = useState(emptyRenovacaoForm)
  const [trocaServidorForm, setTrocaServidorForm] = useState(emptyTrocaServidorForm)
  const [filters, setFilters] = useState({ clienteId: '', servidorId: '' })
  const [mode, setMode] = useState('create')
  const [technicalAction, setTechnicalAction] = useState('')
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)
  const [isDeleting, setIsDeleting] = useState(false)
  const [isTechnicalOperationLoading, setIsTechnicalOperationLoading] = useState(false)
  const [error, setError] = useState('')
  const [message, setMessage] = useState('')

  const sortedTelas = useMemo(
    () => [...telas].sort((a, b) => a.nomeIdentificacao.localeCompare(b.nomeIdentificacao)),
    [telas],
  )

  async function loadReferences() {
    const [clientesData, servidoresData] = await Promise.all([getClientes(), getServidores()])

    setClientes(clientesData)
    setServidores(servidoresData)
  }

  async function loadTelas(nextFilters = filters) {
    try {
      setIsLoading(true)
      setError('')

      const data = await getTelas(nextFilters)

      setTelas(data)
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
        const data = await getTelas(filters)
        setTelas(data)
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
    setSelectedTela(null)
    setFormData(emptyForm)
    setTechnicalAction('')
    setRenovacaoForm(emptyRenovacaoForm)
    setTrocaServidorForm(emptyTrocaServidorForm)
    setError('')
    setMessage('')
  }

  async function handleViewTela(id) {
    try {
      setError('')
      setMessage('')

      const tela = await getTelaById(id)

      setSelectedTela(tela)
      setFormData(toFormData(tela))
      setMode('view')
      setTechnicalAction('')
      setRenovacaoForm(emptyRenovacaoForm)
      setTrocaServidorForm(emptyTrocaServidorForm)
    } catch (apiError) {
      setError(apiError.message)
    }
  }

  function handleEditMode() {
    setMode('edit')
    setFormData(toFormData(selectedTela))
    setTechnicalAction('')
    setError('')
    setMessage('')
  }

  function handleChange(event) {
    const { name, type, checked, value } = event.target

    setFormData((current) => ({
      ...current,
      [name]: type === 'checkbox' ? checked : value,
    }))
  }

  function handleFilterChange(event) {
    const { name, value } = event.target

    setFilters((current) => ({
      ...current,
      [name]: value,
    }))
  }

  function handleRenovacaoChange(event) {
    const { name, value } = event.target

    setRenovacaoForm((current) => ({
      ...current,
      [name]: value,
    }))
  }

  function handleTrocaServidorChange(event) {
    const { name, value } = event.target

    setTrocaServidorForm((current) => ({
      ...current,
      [name]: value,
    }))
  }

  function openRenovacaoForm() {
    if (!selectedTela) {
      return
    }

    setMode('view')
    setTechnicalAction('renovacao')
    setRenovacaoForm({
      novaDataVencimentoTecnico: toDateInputValue(selectedTela.dataVencimentoTecnico),
      valorAcordadoNovo: '',
      observacao: '',
    })
    setTrocaServidorForm(emptyTrocaServidorForm)
    setError('')
    setMessage('')
  }

  function openTrocaServidorForm() {
    if (!selectedTela) {
      return
    }

    setMode('view')
    setTechnicalAction('trocaServidor')
    setTrocaServidorForm({
      servidorNovoId: '',
      observacao: '',
    })
    setRenovacaoForm(emptyRenovacaoForm)
    setError('')
    setMessage('')
  }

  function closeTechnicalAction() {
    setTechnicalAction('')
    setRenovacaoForm(emptyRenovacaoForm)
    setTrocaServidorForm(emptyTrocaServidorForm)
    setError('')
  }

  async function handleApplyFilters(event) {
    event.preventDefault()
    setMessage('')

    await loadTelas(filters)
  }

  async function handleClearFilters() {
    const emptyFilters = { clienteId: '', servidorId: '' }

    setFilters(emptyFilters)
    setMessage('')
    await loadTelas(emptyFilters)
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
      const payload = buildPayload(formData, isEdit)
      const savedTela = isEdit
        ? await updateTela(selectedTela.id, payload)
        : await createTela(payload)

      await loadTelas(filters)
      setSelectedTela(savedTela)
      setFormData(toFormData(savedTela))
      setMode('view')
      setMessage(isEdit ? 'Tela atualizada com sucesso.' : 'Tela cadastrada com sucesso.')
    } catch (apiError) {
      setError(apiError.message)
    } finally {
      setIsSaving(false)
    }
  }

  async function handleDeleteTela() {
    if (!selectedTela) {
      return
    }

    const shouldDelete = window.confirm(`Excluir tela "${selectedTela.nomeIdentificacao}"?`)
    if (!shouldDelete) {
      return
    }

    try {
      setIsDeleting(true)
      setError('')
      setMessage('')

      await deleteTela(selectedTela.id)
      await loadTelas(filters)
      handleCreateMode()
      setMessage('Tela excluida com sucesso.')
    } catch (apiError) {
      setError(apiError.message)
    } finally {
      setIsDeleting(false)
    }
  }

  async function refreshAfterTechnicalOperation(updatedTela, successMessage) {
    await loadTelas(filters)
    setSelectedTela(updatedTela)
    setFormData(toFormData(updatedTela))
    setMode('view')
    setTechnicalAction('')
    setRenovacaoForm(emptyRenovacaoForm)
    setTrocaServidorForm(emptyTrocaServidorForm)
    setMessage(successMessage)
  }

  async function handleRenovarTela(event) {
    event.preventDefault()

    if (!selectedTela) {
      return
    }

    if (!renovacaoForm.novaDataVencimentoTecnico) {
      setError('Nova data de vencimento tecnico e obrigatoria.')
      setMessage('')
      return
    }

    const valorAcordadoNovo =
      renovacaoForm.valorAcordadoNovo === '' ? null : Number(renovacaoForm.valorAcordadoNovo)

    if (valorAcordadoNovo !== null && (!Number.isFinite(valorAcordadoNovo) || valorAcordadoNovo < 0)) {
      setError('Valor acordado novo deve ser vazio ou maior ou igual a zero.')
      setMessage('')
      return
    }

    try {
      setIsTechnicalOperationLoading(true)
      setError('')
      setMessage('')

      const updatedTela = await renovarTela(selectedTela.id, {
        novaDataVencimentoTecnico: renovacaoForm.novaDataVencimentoTecnico,
        valorAcordadoNovo,
        observacao: renovacaoForm.observacao.trim() || null,
      })

      await refreshAfterTechnicalOperation(updatedTela, 'Tela renovada com sucesso.')
    } catch (apiError) {
      setError(apiError.message)
    } finally {
      setIsTechnicalOperationLoading(false)
    }
  }

  async function handleTrocarServidor(event) {
    event.preventDefault()

    if (!selectedTela) {
      return
    }

    if (!trocaServidorForm.servidorNovoId) {
      setError('Novo servidor e obrigatorio.')
      setMessage('')
      return
    }

    try {
      setIsTechnicalOperationLoading(true)
      setError('')
      setMessage('')

      const updatedTela = await trocarServidor(selectedTela.id, {
        servidorNovoId: trocaServidorForm.servidorNovoId,
        observacao: trocaServidorForm.observacao.trim() || null,
      })

      await refreshAfterTechnicalOperation(updatedTela, 'Servidor da tela alterado com sucesso.')
    } catch (apiError) {
      setError(apiError.message)
    } finally {
      setIsTechnicalOperationLoading(false)
    }
  }

  const isFormReadonly = mode === 'view'
  const panelTitle = mode === 'edit' ? 'Editar tela' : mode === 'view' ? 'Detalhe da tela' : 'Nova tela'

  return (
    <section>
      <div className="d-flex flex-column flex-md-row justify-content-between gap-2 mb-4">
        <div>
          <h1>Telas</h1>
          <p className="text-muted mb-0">Cadastro e consulta de telas de clientes.</p>
        </div>
        <div>
          <button className="btn btn-primary" type="button" onClick={handleCreateMode}>
            Nova tela
          </button>
        </div>
      </div>

      {message && <div className="alert alert-success">{message}</div>}
      {error && <div className="alert alert-danger">{error}</div>}

      <form className="card mb-3" onSubmit={handleApplyFilters}>
        <div className="card-body">
          <div className="row g-3 align-items-end">
            <div className="col-12 col-md-5">
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

            <div className="col-12 col-md-5">
              <label className="form-label" htmlFor="filtroServidor">
                Servidor
              </label>
              <select
                className="form-select"
                id="filtroServidor"
                name="servidorId"
                value={filters.servidorId}
                onChange={handleFilterChange}
              >
                <option value="">Todos</option>
                {servidores.map((servidor) => (
                  <option key={servidor.id} value={servidor.id}>
                    {servidor.nome}
                  </option>
                ))}
              </select>
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
              <strong>Lista de telas</strong>
            </div>
            <div className="table-responsive">
              <table className="table table-striped table-hover align-middle mb-0">
                <thead>
                  <tr>
                    <th>Identificacao</th>
                    <th>Cliente</th>
                    <th>Servidor</th>
                    <th>Status</th>
                    <th>Vencimento</th>
                    <th className="text-end">Acoes</th>
                  </tr>
                </thead>
                <tbody>
                  {isLoading ? (
                    <tr>
                      <td className="text-muted text-center" colSpan="6">
                        Carregando telas...
                      </td>
                    </tr>
                  ) : sortedTelas.length === 0 ? (
                    <tr>
                      <td className="text-muted text-center" colSpan="6">
                        Nenhuma tela encontrada.
                      </td>
                    </tr>
                  ) : (
                    sortedTelas.map((tela) => (
                      <tr key={tela.id}>
                        <td>
                          <div>{tela.nomeIdentificacao}</div>
                          <div className="text-muted small">{currencyFormatter.format(tela.valorAcordado ?? 0)}</div>
                        </td>
                        <td>{getNameById(clientes, tela.clienteId)}</td>
                        <td>{getNameById(servidores, tela.servidorId)}</td>
                        <td>
                          <span className={`badge ${getStatusBadge(tela.status)}`}>
                            {getStatusLabel(tela.status)}
                          </span>
                        </td>
                        <td>{formatDate(tela.dataVencimentoTecnico)}</td>
                        <td className="text-end">
                          <button
                            className="btn btn-sm btn-outline-primary"
                            type="button"
                            onClick={() => handleViewTela(tela.id)}
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
              {mode === 'view' && selectedTela && (
                <div className="btn-group btn-group-sm">
                  <button className="btn btn-outline-success" type="button" onClick={openRenovacaoForm}>
                    Renovar
                  </button>
                  <button className="btn btn-outline-secondary" type="button" onClick={openTrocaServidorForm}>
                    Trocar servidor
                  </button>
                  <button className="btn btn-outline-primary" type="button" onClick={handleEditMode}>
                    Editar
                  </button>
                  <button
                    className="btn btn-outline-danger"
                    type="button"
                    onClick={handleDeleteTela}
                    disabled={isDeleting}
                  >
                    {isDeleting ? 'Excluindo...' : 'Excluir'}
                  </button>
                </div>
              )}
            </div>
            <div className="card-body">
              {mode === 'view' && selectedTela ? (
                <div className="mb-4">
                  <dl className="row mb-0">
                    <dt className="col-sm-5">Data inicio</dt>
                    <dd className="col-sm-7">{formatDate(selectedTela.dataInicio)}</dd>
                    <dt className="col-sm-5">Identificador</dt>
                    <dd className="col-sm-7 text-break">{selectedTela.id}</dd>
                  </dl>
                </div>
              ) : null}

              {mode === 'view' && technicalAction === 'renovacao' && (
                <form className="border rounded p-3 mb-4" onSubmit={handleRenovarTela}>
                  <h2 className="h6 mb-3">Renovacao tecnica</h2>
                  <div className="mb-3">
                    <label className="form-label" htmlFor="novaDataVencimentoTecnico">
                      Nova data de vencimento
                    </label>
                    <input
                      className="form-control"
                      id="novaDataVencimentoTecnico"
                      name="novaDataVencimentoTecnico"
                      type="date"
                      value={renovacaoForm.novaDataVencimentoTecnico}
                      onChange={handleRenovacaoChange}
                      required
                    />
                  </div>

                  <div className="mb-3">
                    <label className="form-label" htmlFor="valorAcordadoNovo">
                      Valor acordado novo
                    </label>
                    <input
                      className="form-control"
                      id="valorAcordadoNovo"
                      name="valorAcordadoNovo"
                      type="number"
                      min="0"
                      step="0.01"
                      value={renovacaoForm.valorAcordadoNovo}
                      onChange={handleRenovacaoChange}
                    />
                  </div>

                  <div className="mb-3">
                    <label className="form-label" htmlFor="observacaoRenovacao">
                      Observacao
                    </label>
                    <textarea
                      className="form-control"
                      id="observacaoRenovacao"
                      name="observacao"
                      rows="2"
                      value={renovacaoForm.observacao}
                      onChange={handleRenovacaoChange}
                    />
                  </div>

                  <div className="d-flex gap-2">
                    <button className="btn btn-success" type="submit" disabled={isTechnicalOperationLoading}>
                      {isTechnicalOperationLoading ? 'Renovando...' : 'Confirmar renovacao'}
                    </button>
                    <button className="btn btn-outline-secondary" type="button" onClick={closeTechnicalAction}>
                      Cancelar
                    </button>
                  </div>
                </form>
              )}

              {mode === 'view' && technicalAction === 'trocaServidor' && (
                <form className="border rounded p-3 mb-4" onSubmit={handleTrocarServidor}>
                  <h2 className="h6 mb-3">Troca de servidor</h2>
                  <div className="mb-3">
                    <label className="form-label" htmlFor="servidorNovoId">
                      Novo servidor
                    </label>
                    <select
                      className="form-select"
                      id="servidorNovoId"
                      name="servidorNovoId"
                      value={trocaServidorForm.servidorNovoId}
                      onChange={handleTrocaServidorChange}
                      required
                    >
                      <option value="">Selecione</option>
                      {servidores.map((servidor) => (
                        <option key={servidor.id} value={servidor.id}>
                          {servidor.nome}
                        </option>
                      ))}
                    </select>
                  </div>

                  <div className="mb-3">
                    <label className="form-label" htmlFor="observacaoTrocaServidor">
                      Observacao
                    </label>
                    <textarea
                      className="form-control"
                      id="observacaoTrocaServidor"
                      name="observacao"
                      rows="2"
                      value={trocaServidorForm.observacao}
                      onChange={handleTrocaServidorChange}
                    />
                  </div>

                  <div className="d-flex gap-2">
                    <button className="btn btn-secondary" type="submit" disabled={isTechnicalOperationLoading}>
                      {isTechnicalOperationLoading ? 'Trocando...' : 'Confirmar troca'}
                    </button>
                    <button className="btn btn-outline-secondary" type="button" onClick={closeTechnicalAction}>
                      Cancelar
                    </button>
                  </div>
                </form>
              )}

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
                    <label className="form-label" htmlFor="servidorId">
                      Servidor
                    </label>
                    <select
                      className="form-select"
                      id="servidorId"
                      name="servidorId"
                      value={formData.servidorId}
                      onChange={handleChange}
                      disabled={isFormReadonly}
                      required
                    >
                      <option value="">Selecione</option>
                      {servidores.map((servidor) => (
                        <option key={servidor.id} value={servidor.id}>
                          {servidor.nome}
                        </option>
                      ))}
                    </select>
                  </div>
                </div>

                <div className="mb-3 mt-3">
                  <label className="form-label" htmlFor="nomeIdentificacao">
                    Nome identificacao
                  </label>
                  <input
                    className="form-control"
                    id="nomeIdentificacao"
                    name="nomeIdentificacao"
                    type="text"
                    value={formData.nomeIdentificacao}
                    onChange={handleChange}
                    disabled={isFormReadonly}
                    required
                  />
                </div>

                <div className="row g-3">
                  <div className="col-12 col-md-6">
                    <label className="form-label" htmlFor="usuarioTela">
                      Usuario tela
                    </label>
                    <input
                      className="form-control"
                      id="usuarioTela"
                      name="usuarioTela"
                      type="text"
                      value={formData.usuarioTela}
                      onChange={handleChange}
                      disabled={isFormReadonly}
                      required
                    />
                  </div>

                  <div className="col-12 col-md-6">
                    <label className="form-label" htmlFor="senhaTela">
                      Senha tela
                    </label>
                    <input
                      className="form-control"
                      id="senhaTela"
                      name="senhaTela"
                      type="text"
                      value={formData.senhaTela}
                      onChange={handleChange}
                      disabled={isFormReadonly}
                      required
                    />
                  </div>
                </div>

                <div className="row g-3 mt-0">
                  <div className="col-12 col-md-6">
                    <label className="form-label" htmlFor="valorAcordado">
                      Valor acordado
                    </label>
                    <input
                      className="form-control"
                      id="valorAcordado"
                      name="valorAcordado"
                      type="number"
                      min="0"
                      step="0.01"
                      value={formData.valorAcordado}
                      onChange={handleChange}
                      disabled={isFormReadonly}
                    />
                  </div>

                  <div className="col-12 col-md-6">
                    <label className="form-label" htmlFor="dataVencimentoTecnico">
                      Vencimento tecnico
                    </label>
                    <input
                      className="form-control"
                      id="dataVencimentoTecnico"
                      name="dataVencimentoTecnico"
                      type="date"
                      value={formData.dataVencimentoTecnico}
                      onChange={handleChange}
                      disabled={isFormReadonly}
                      required
                    />
                  </div>
                </div>

                <div className="row g-3 mt-0">
                  <div className="col-12 col-md-6">
                    <label className="form-label" htmlFor="status">
                      Status
                    </label>
                    <select
                      className="form-select"
                      id="status"
                      name="status"
                      value={formData.status}
                      onChange={handleChange}
                      disabled={isFormReadonly}
                    >
                      {statusOptions.map((option) => (
                        <option key={option.value} value={option.value}>
                          {option.label}
                        </option>
                      ))}
                    </select>
                  </div>

                  <div className="col-12 col-md-6">
                    <label className="form-label" htmlFor="marcaTv">
                      Marca TV
                    </label>
                    <input
                      className="form-control"
                      id="marcaTv"
                      name="marcaTv"
                      type="text"
                      value={formData.marcaTv}
                      onChange={handleChange}
                      disabled={isFormReadonly}
                    />
                  </div>
                </div>

                <div className="mb-3 mt-3">
                  <label className="form-label" htmlFor="appUtilizado">
                    App utilizado
                  </label>
                  <input
                    className="form-control"
                    id="appUtilizado"
                    name="appUtilizado"
                    type="text"
                    value={formData.appUtilizado}
                    onChange={handleChange}
                    disabled={isFormReadonly}
                  />
                </div>

                <div className="row g-3">
                  <div className="col-12 col-md-6">
                    <label className="form-label" htmlFor="macOuIdApp">
                      MAC/ID app
                    </label>
                    <input
                      className="form-control"
                      id="macOuIdApp"
                      name="macOuIdApp"
                      type="text"
                      value={formData.macOuIdApp}
                      onChange={handleChange}
                      disabled={isFormReadonly}
                    />
                  </div>

                  <div className="col-12 col-md-6">
                    <label className="form-label" htmlFor="chaveSecundaria">
                      Chave secundaria
                    </label>
                    <input
                      className="form-control"
                      id="chaveSecundaria"
                      name="chaveSecundaria"
                      type="text"
                      value={formData.chaveSecundaria}
                      onChange={handleChange}
                      disabled={isFormReadonly}
                    />
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

                {(mode === 'edit' || mode === 'view') && (
                  <div className="form-check form-switch mb-3">
                    <input
                      className="form-check-input"
                      id="ativo"
                      name="ativo"
                      type="checkbox"
                      checked={formData.ativo}
                      onChange={handleChange}
                      disabled={isFormReadonly}
                    />
                    <label className="form-check-label" htmlFor="ativo">
                      Ativo
                    </label>
                  </div>
                )}

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
                          setFormData(toFormData(selectedTela))
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

export default TelasPage
