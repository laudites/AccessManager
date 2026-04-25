import httpClient from './httpClient'

function getApiErrors(responseData) {
  if (Array.isArray(responseData?.errors) && responseData.errors.length > 0) {
    return responseData.errors.join(' ')
  }

  return 'Nao foi possivel concluir a operacao.'
}

function unwrapApiResponse(response) {
  if (response.data?.success === false) {
    throw new Error(getApiErrors(response.data))
  }

  return response.data?.data
}

function handleApiError(error) {
  if (error.response?.data) {
    throw new Error(getApiErrors(error.response.data))
  }

  throw new Error('Nao foi possivel conectar com a API.')
}

export async function getLancamentosFinanceiros(filters = {}) {
  try {
    const response = await httpClient.get('/api/lancamentos-financeiros', {
      params: {
        clienteId: filters.clienteId || undefined,
        telaClienteId: filters.telaClienteId || undefined,
        statusFinanceiro: filters.statusFinanceiro || undefined,
      },
    })

    return unwrapApiResponse(response) ?? []
  } catch (error) {
    handleApiError(error)
  }
}

export async function getLancamentoFinanceiroById(id) {
  try {
    const response = await httpClient.get(`/api/lancamentos-financeiros/${id}`)

    return unwrapApiResponse(response)
  } catch (error) {
    handleApiError(error)
  }
}

export async function getLancamentosPendentes() {
  try {
    const response = await httpClient.get('/api/lancamentos-financeiros/pendentes')

    return unwrapApiResponse(response) ?? []
  } catch (error) {
    handleApiError(error)
  }
}

export async function getLancamentosAtrasados() {
  try {
    const response = await httpClient.get('/api/lancamentos-financeiros/atrasados')

    return unwrapApiResponse(response) ?? []
  } catch (error) {
    handleApiError(error)
  }
}

export async function createLancamentoFinanceiro(lancamento) {
  try {
    const response = await httpClient.post('/api/lancamentos-financeiros', lancamento)

    return unwrapApiResponse(response)
  } catch (error) {
    handleApiError(error)
  }
}

export async function updateLancamentoFinanceiro(id, lancamento) {
  try {
    const response = await httpClient.put(`/api/lancamentos-financeiros/${id}`, lancamento)

    return unwrapApiResponse(response)
  } catch (error) {
    handleApiError(error)
  }
}

export async function deleteLancamentoFinanceiro(id) {
  try {
    const response = await httpClient.delete(`/api/lancamentos-financeiros/${id}`)

    return unwrapApiResponse(response)
  } catch (error) {
    handleApiError(error)
  }
}

export async function marcarLancamentoPago(id) {
  try {
    const response = await httpClient.patch(`/api/lancamentos-financeiros/${id}/marcar-pago`)

    return unwrapApiResponse(response)
  } catch (error) {
    handleApiError(error)
  }
}
