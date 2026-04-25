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

export async function getTelas(filters = {}) {
  try {
    const response = await httpClient.get('/api/telas', {
      params: {
        clienteId: filters.clienteId || undefined,
        servidorId: filters.servidorId || undefined,
      },
    })

    return unwrapApiResponse(response) ?? []
  } catch (error) {
    handleApiError(error)
  }
}

export async function getTelaById(id) {
  try {
    const response = await httpClient.get(`/api/telas/${id}`)

    return unwrapApiResponse(response)
  } catch (error) {
    handleApiError(error)
  }
}

export async function createTela(tela) {
  try {
    const response = await httpClient.post('/api/telas', tela)

    return unwrapApiResponse(response)
  } catch (error) {
    handleApiError(error)
  }
}

export async function updateTela(id, tela) {
  try {
    const response = await httpClient.put(`/api/telas/${id}`, tela)

    return unwrapApiResponse(response)
  } catch (error) {
    handleApiError(error)
  }
}

export async function deleteTela(id) {
  try {
    const response = await httpClient.delete(`/api/telas/${id}`)

    return unwrapApiResponse(response)
  } catch (error) {
    handleApiError(error)
  }
}

export async function renovarTela(id, payload) {
  try {
    const response = await httpClient.post(`/api/telas/${id}/renovar`, payload)

    return unwrapApiResponse(response)
  } catch (error) {
    handleApiError(error)
  }
}

export async function trocarServidor(id, payload) {
  try {
    const response = await httpClient.post(`/api/telas/${id}/trocar-servidor`, payload)

    return unwrapApiResponse(response)
  } catch (error) {
    handleApiError(error)
  }
}
