import { useEffect, useMemo, useState } from 'react'
import {
  createCliente,
  deleteCliente,
  getClienteById,
  getClientes,
  updateCliente,
} from '../../api/clientesApi'
import FeedbackAlert from '../../components/FeedbackAlert'
import LoadingButton from '../../components/LoadingButton'

const emptyForm = {
  nome: '',
  telefone: '',
  observacao: '',
  diaPagamentoPreferido: '',
  ativo: true,
}

function toFormData(cliente) {
  return {
    nome: cliente?.nome ?? '',
    telefone: cliente?.telefone ?? '',
    observacao: cliente?.observacao ?? '',
    diaPagamentoPreferido: cliente?.diaPagamentoPreferido ?? '',
    ativo: cliente?.ativo ?? true,
  }
}

function buildPayload(formData, includeAtivo) {
  const payload = {
    nome: formData.nome.trim(),
    telefone: formData.telefone.trim(),
    observacao: formData.observacao.trim() || null,
    diaPagamentoPreferido:
      formData.diaPagamentoPreferido === ''
        ? null
        : Number(formData.diaPagamentoPreferido),
  }

  if (includeAtivo) {
    payload.ativo = formData.ativo
  }

  return payload
}

function getValidationError(formData) {
  if (!formData.nome.trim()) {
    return 'Campo obrigatorio: Nome.'
  }

  if (formData.diaPagamentoPreferido !== '') {
    const diaPagamento = Number(formData.diaPagamentoPreferido)

    if (!Number.isInteger(diaPagamento) || diaPagamento < 1 || diaPagamento > 31) {
      return 'Dia de pagamento preferido deve ser vazio ou entre 1 e 31.'
    }
  }

  return ''
}

function formatDate(value) {
  if (!value) {
    return '-'
  }

  return new Intl.DateTimeFormat('pt-BR').format(new Date(value))
}

function ClientesPage() {
  const [clientes, setClientes] = useState([])
  const [selectedCliente, setSelectedCliente] = useState(null)
  const [formData, setFormData] = useState(emptyForm)
  const [mode, setMode] = useState('create')
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)
  const [isDeleting, setIsDeleting] = useState(false)
  const [error, setError] = useState('')
  const [message, setMessage] = useState('')

  const sortedClientes = useMemo(
    () => [...clientes].sort((a, b) => a.nome.localeCompare(b.nome)),
    [clientes],
  )

  async function loadClientes() {
    try {
      setIsLoading(true)
      setError('')

      const data = await getClientes()

      setClientes(data)
    } catch (apiError) {
      setError(apiError.message)
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    loadClientes()
  }, [])

  function handleCreateMode() {
    setMode('create')
    setSelectedCliente(null)
    setFormData(emptyForm)
    setError('')
    setMessage('')
  }

  async function handleViewCliente(id) {
    try {
      setError('')
      setMessage('')

      const cliente = await getClienteById(id)

      setSelectedCliente(cliente)
      setFormData(toFormData(cliente))
      setMode('view')
    } catch (apiError) {
      setError(apiError.message)
    }
  }

  function handleEditMode() {
    setMode('edit')
    setFormData(toFormData(selectedCliente))
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
      const savedCliente = isEdit
        ? await updateCliente(selectedCliente.id, payload)
        : await createCliente(payload)

      await loadClientes()
      setSelectedCliente(savedCliente)
      setFormData(toFormData(savedCliente))
      setMode('view')
      setMessage('Salvo com sucesso.')
    } catch (apiError) {
      setError(`Erro ao salvar. ${apiError.message}`)
    } finally {
      setIsSaving(false)
    }
  }

  async function handleDeleteCliente() {
    if (!selectedCliente) {
      return
    }

    const shouldDelete = window.confirm(`Excluir cliente "${selectedCliente.nome}"?`)
    if (!shouldDelete) {
      return
    }

    try {
      setIsDeleting(true)
      setError('')
      setMessage('')

      await deleteCliente(selectedCliente.id)
      await loadClientes()
      handleCreateMode()
      setMessage('Excluido com sucesso.')
    } catch (apiError) {
      setError(`Erro ao excluir. ${apiError.message}`)
    } finally {
      setIsDeleting(false)
    }
  }

  const isFormReadonly = mode === 'view'
  const panelTitle = mode === 'edit' ? 'Editar cliente' : mode === 'view' ? 'Detalhe do cliente' : 'Novo cliente'

  return (
    <section>
      <div className="d-flex flex-column flex-md-row justify-content-between gap-2 mb-4">
        <div>
          <h1>Clientes</h1>
          <p className="text-muted mb-0">Cadastro e consulta de clientes.</p>
        </div>
        <div>
          <button className="btn btn-primary" type="button" onClick={handleCreateMode}>
            Novo cliente
          </button>
        </div>
      </div>

      <FeedbackAlert message={message} />
      <FeedbackAlert message={error} type="danger" />

      <div className="row g-3">
        <div className="col-12 col-xl-7">
          <div className="card h-100">
            <div className="card-header bg-white">
              <strong>Lista de clientes</strong>
            </div>
            <div className="table-responsive">
              <table className="table table-striped table-hover align-middle mb-0">
                <thead>
                  <tr>
                    <th>Nome</th>
                    <th>Telefone</th>
                    <th>Dia pagamento</th>
                    <th>Status</th>
                    <th className="text-end">Acoes</th>
                  </tr>
                </thead>
                <tbody>
                  {isLoading ? (
                    <tr>
                      <td className="text-muted text-center" colSpan="5">
                        Carregando clientes...
                      </td>
                    </tr>
                  ) : sortedClientes.length === 0 ? (
                    <tr>
                      <td className="text-muted text-center" colSpan="5">
                        Nenhum cliente encontrado.
                      </td>
                    </tr>
                  ) : (
                    sortedClientes.map((cliente) => (
                      <tr key={cliente.id}>
                        <td>{cliente.nome}</td>
                        <td>{cliente.telefone || '-'}</td>
                        <td>{cliente.diaPagamentoPreferido ?? '-'}</td>
                        <td>
                          <span className={`badge ${cliente.ativo ? 'text-bg-success' : 'text-bg-secondary'}`}>
                            {cliente.ativo ? 'Ativo' : 'Inativo'}
                          </span>
                        </td>
                        <td className="text-end">
                          <button
                            className="btn btn-sm btn-outline-primary"
                            type="button"
                            onClick={() => handleViewCliente(cliente.id)}
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
              {mode === 'view' && selectedCliente && (
                <div className="btn-group btn-group-sm">
                  <button className="btn btn-outline-primary" type="button" onClick={handleEditMode}>
                    Editar
                  </button>
                  <button
                    className="btn btn-outline-danger"
                    type="button"
                    onClick={handleDeleteCliente}
                    disabled={isDeleting}
                  >
                    {isDeleting ? 'Excluindo...' : 'Excluir'}
                  </button>
                </div>
              )}
            </div>
            <div className="card-body">
              {mode === 'view' && selectedCliente ? (
                <div className="mb-4">
                  <dl className="row mb-0">
                    <dt className="col-sm-5">Data cadastro</dt>
                    <dd className="col-sm-7">{formatDate(selectedCliente.dataCadastro)}</dd>
                    <dt className="col-sm-5">Identificador</dt>
                    <dd className="col-sm-7 text-break">{selectedCliente.id}</dd>
                  </dl>
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
                    placeholder="Nome do cliente"
                    type="text"
                    value={formData.nome}
                    onChange={handleChange}
                    disabled={isFormReadonly}
                    required
                  />
                </div>

                <div className="mb-3">
                  <label className="form-label" htmlFor="telefone">
                    Telefone
                  </label>
                  <input
                    className="form-control"
                    id="telefone"
                    name="telefone"
                    placeholder="Telefone de contato"
                    type="text"
                    value={formData.telefone}
                    onChange={handleChange}
                    disabled={isFormReadonly}
                  />
                </div>

                <div className="mb-3">
                  <label className="form-label" htmlFor="diaPagamentoPreferido">
                    Dia pagamento preferido
                  </label>
                  <input
                    className="form-control"
                    id="diaPagamentoPreferido"
                    name="diaPagamentoPreferido"
                    placeholder="1 a 31"
                    type="number"
                    min="1"
                    max="31"
                    value={formData.diaPagamentoPreferido}
                    onChange={handleChange}
                    disabled={isFormReadonly}
                  />
                </div>

                <div className="mb-3">
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
                    <LoadingButton isLoading={isSaving} loadingText="Salvando..." type="submit">
                      Salvar
                    </LoadingButton>
                    {mode === 'edit' && (
                      <button
                        className="btn btn-outline-secondary"
                        type="button"
                        onClick={() => {
                          setMode('view')
                          setFormData(toFormData(selectedCliente))
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

export default ClientesPage
