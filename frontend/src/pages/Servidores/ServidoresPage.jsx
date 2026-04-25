import { useEffect, useMemo, useState } from 'react'
import {
  createServidor,
  deleteServidor,
  getServidorById,
  getServidores,
  updateServidor,
  updateServidorStatus,
} from '../../api/servidoresApi'

const statusOptions = [
  { value: 1, label: 'Ativo', badge: 'text-bg-success' },
  { value: 2, label: 'Manutencao', badge: 'text-bg-warning' },
  { value: 3, label: 'Inativo', badge: 'text-bg-secondary' },
]

const emptyForm = {
  nome: '',
  descricao: '',
  status: '1',
  limiteClientes: '0',
  usuarioPainel: '',
  senhaPainel: '',
  observacao: '',
  ativo: true,
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

function toFormData(servidor) {
  return {
    nome: servidor?.nome ?? '',
    descricao: servidor?.descricao ?? '',
    status: String(servidor?.status ?? 1),
    limiteClientes: String(servidor?.limiteClientes ?? 0),
    usuarioPainel: servidor?.usuarioPainel ?? '',
    senhaPainel: servidor?.senhaPainel ?? '',
    observacao: servidor?.observacao ?? '',
    ativo: servidor?.ativo ?? true,
  }
}

function buildPayload(formData, includeAtivo) {
  const payload = {
    nome: formData.nome.trim(),
    descricao: formData.descricao.trim() || null,
    status: Number(formData.status),
    limiteClientes: Number(formData.limiteClientes),
    usuarioPainel: formData.usuarioPainel.trim(),
    senhaPainel: formData.senhaPainel.trim(),
    observacao: formData.observacao.trim() || null,
  }

  if (includeAtivo) {
    payload.ativo = formData.ativo
  }

  return payload
}

function getValidationError(formData) {
  const limiteClientes = Number(formData.limiteClientes)
  const status = Number(formData.status)

  if (!formData.nome.trim()) {
    return 'Nome e obrigatorio.'
  }

  if (!Number.isInteger(limiteClientes) || limiteClientes < 0) {
    return 'LimiteClientes deve ser maior ou igual a zero.'
  }

  if (!statusOptions.some((option) => option.value === status)) {
    return 'Status deve ser valido.'
  }

  return ''
}

function ServidoresPage() {
  const [servidores, setServidores] = useState([])
  const [selectedServidor, setSelectedServidor] = useState(null)
  const [formData, setFormData] = useState(emptyForm)
  const [mode, setMode] = useState('create')
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)
  const [isDeleting, setIsDeleting] = useState(false)
  const [isUpdatingStatus, setIsUpdatingStatus] = useState(false)
  const [error, setError] = useState('')
  const [message, setMessage] = useState('')

  const sortedServidores = useMemo(
    () => [...servidores].sort((a, b) => a.nome.localeCompare(b.nome)),
    [servidores],
  )

  async function loadServidores() {
    try {
      setIsLoading(true)
      setError('')

      const data = await getServidores()

      setServidores(data)
    } catch (apiError) {
      setError(apiError.message)
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    loadServidores()
  }, [])

  function handleCreateMode() {
    setMode('create')
    setSelectedServidor(null)
    setFormData(emptyForm)
    setError('')
    setMessage('')
  }

  async function handleViewServidor(id) {
    try {
      setError('')
      setMessage('')

      const servidor = await getServidorById(id)

      setSelectedServidor(servidor)
      setFormData(toFormData(servidor))
      setMode('view')
    } catch (apiError) {
      setError(apiError.message)
    }
  }

  function handleEditMode() {
    setMode('edit')
    setFormData(toFormData(selectedServidor))
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
      const savedServidor = isEdit
        ? await updateServidor(selectedServidor.id, payload)
        : await createServidor(payload)

      await loadServidores()
      setSelectedServidor(savedServidor)
      setFormData(toFormData(savedServidor))
      setMode('view')
      setMessage(isEdit ? 'Servidor atualizado com sucesso.' : 'Servidor cadastrado com sucesso.')
    } catch (apiError) {
      setError(apiError.message)
    } finally {
      setIsSaving(false)
    }
  }

  async function handleDeleteServidor() {
    if (!selectedServidor) {
      return
    }

    const shouldDelete = window.confirm(`Excluir servidor "${selectedServidor.nome}"?`)
    if (!shouldDelete) {
      return
    }

    try {
      setIsDeleting(true)
      setError('')
      setMessage('')

      await deleteServidor(selectedServidor.id)
      await loadServidores()
      handleCreateMode()
      setMessage('Servidor excluido com sucesso.')
    } catch (apiError) {
      setError(apiError.message)
    } finally {
      setIsDeleting(false)
    }
  }

  async function handleStatusChange(event) {
    if (!selectedServidor) {
      return
    }

    const nextStatus = Number(event.target.value)
    if (!statusOptions.some((option) => option.value === nextStatus)) {
      setError('Status deve ser valido.')
      setMessage('')
      return
    }

    try {
      setIsUpdatingStatus(true)
      setError('')
      setMessage('')

      const updatedServidor = await updateServidorStatus(selectedServidor.id, nextStatus)

      await loadServidores()
      setSelectedServidor(updatedServidor)
      setFormData(toFormData(updatedServidor))
      setMode('view')
      setMessage('Status do servidor atualizado com sucesso.')
    } catch (apiError) {
      setError(apiError.message)
    } finally {
      setIsUpdatingStatus(false)
    }
  }

  const isFormReadonly = mode === 'view'
  const panelTitle = mode === 'edit' ? 'Editar servidor' : mode === 'view' ? 'Detalhe do servidor' : 'Novo servidor'

  return (
    <section>
      <div className="d-flex flex-column flex-md-row justify-content-between gap-2 mb-4">
        <div>
          <h1>Servidores</h1>
          <p className="text-muted mb-0">Cadastro e acompanhamento dos servidores.</p>
        </div>
        <div>
          <button className="btn btn-primary" type="button" onClick={handleCreateMode}>
            Novo servidor
          </button>
        </div>
      </div>

      {message && <div className="alert alert-success">{message}</div>}
      {error && <div className="alert alert-danger">{error}</div>}

      <div className="row g-3">
        <div className="col-12 col-xl-7">
          <div className="card h-100">
            <div className="card-header bg-white">
              <strong>Lista de servidores</strong>
            </div>
            <div className="table-responsive">
              <table className="table table-striped table-hover align-middle mb-0">
                <thead>
                  <tr>
                    <th>Nome</th>
                    <th>Status</th>
                    <th>Limite</th>
                    <th>Ativo</th>
                    <th className="text-end">Acoes</th>
                  </tr>
                </thead>
                <tbody>
                  {isLoading ? (
                    <tr>
                      <td className="text-muted text-center" colSpan="5">
                        Carregando servidores...
                      </td>
                    </tr>
                  ) : sortedServidores.length === 0 ? (
                    <tr>
                      <td className="text-muted text-center" colSpan="5">
                        Nenhum servidor encontrado.
                      </td>
                    </tr>
                  ) : (
                    sortedServidores.map((servidor) => (
                      <tr key={servidor.id}>
                        <td>
                          <div>{servidor.nome}</div>
                          <div className="text-muted small">{servidor.descricao || '-'}</div>
                        </td>
                        <td>
                          <span className={`badge ${getStatusBadge(servidor.status)}`}>
                            {getStatusLabel(servidor.status)}
                          </span>
                        </td>
                        <td>{servidor.limiteClientes}</td>
                        <td>
                          <span className={`badge ${servidor.ativo ? 'text-bg-success' : 'text-bg-secondary'}`}>
                            {servidor.ativo ? 'Sim' : 'Nao'}
                          </span>
                        </td>
                        <td className="text-end">
                          <button
                            className="btn btn-sm btn-outline-primary"
                            type="button"
                            onClick={() => handleViewServidor(servidor.id)}
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
              {mode === 'view' && selectedServidor && (
                <div className="btn-group btn-group-sm">
                  <button className="btn btn-outline-primary" type="button" onClick={handleEditMode}>
                    Editar
                  </button>
                  <button
                    className="btn btn-outline-danger"
                    type="button"
                    onClick={handleDeleteServidor}
                    disabled={isDeleting}
                  >
                    {isDeleting ? 'Excluindo...' : 'Excluir'}
                  </button>
                </div>
              )}
            </div>
            <div className="card-body">
              {mode === 'view' && selectedServidor ? (
                <div className="mb-4">
                  <dl className="row mb-3">
                    <dt className="col-sm-5">Identificador</dt>
                    <dd className="col-sm-7 text-break">{selectedServidor.id}</dd>
                  </dl>

                  <label className="form-label" htmlFor="statusRapido">
                    Alterar status
                  </label>
                  <select
                    className="form-select"
                    id="statusRapido"
                    value={selectedServidor.status}
                    onChange={handleStatusChange}
                    disabled={isUpdatingStatus}
                  >
                    {statusOptions.map((option) => (
                      <option key={option.value} value={option.value}>
                        {option.label}
                      </option>
                    ))}
                  </select>
                </div>
              ) : null}

              <form onSubmit={handleSubmit}>
                <div className="mb-3">
                  <label className="form-label" htmlFor="nome">
                    Nome
                  </label>
                  <input
                    className="form-control"
                    id="nome"
                    name="nome"
                    type="text"
                    value={formData.nome}
                    onChange={handleChange}
                    disabled={isFormReadonly}
                    required
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
                    type="text"
                    value={formData.descricao}
                    onChange={handleChange}
                    disabled={isFormReadonly}
                  />
                </div>

                <div className="row g-3">
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
                    <label className="form-label" htmlFor="limiteClientes">
                      Limite clientes
                    </label>
                    <input
                      className="form-control"
                      id="limiteClientes"
                      name="limiteClientes"
                      type="number"
                      min="0"
                      value={formData.limiteClientes}
                      onChange={handleChange}
                      disabled={isFormReadonly}
                    />
                  </div>
                </div>

                <div className="row g-3 mt-0">
                  <div className="col-12 col-md-6">
                    <label className="form-label" htmlFor="usuarioPainel">
                      Usuario painel
                    </label>
                    <input
                      className="form-control"
                      id="usuarioPainel"
                      name="usuarioPainel"
                      type="text"
                      value={formData.usuarioPainel}
                      onChange={handleChange}
                      disabled={isFormReadonly}
                    />
                  </div>

                  <div className="col-12 col-md-6">
                    <label className="form-label" htmlFor="senhaPainel">
                      Senha painel
                    </label>
                    <input
                      className="form-control"
                      id="senhaPainel"
                      name="senhaPainel"
                      type="text"
                      value={formData.senhaPainel}
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
                          setFormData(toFormData(selectedServidor))
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

export default ServidoresPage
