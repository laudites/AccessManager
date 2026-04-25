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

export async function getClientes() {
  try {
    const response = await httpClient.get('/api/clientes')

    return unwrapApiResponse(response) ?? []
  } catch (error) {
    handleApiError(error)
  }
}

export async function getClienteById(id) {
  try {
    const response = await httpClient.get(`/api/clientes/${id}`)

    return unwrapApiResponse(response)
  } catch (error) {
    handleApiError(error)
  }
}

export async function createCliente(cliente) {
  try {
    const response = await httpClient.post('/api/clientes', cliente)

    return unwrapApiResponse(response)
  } catch (error) {
    handleApiError(error)
  }
}

export async function updateCliente(id, cliente) {
  try {
    const response = await httpClient.put(`/api/clientes/${id}`, cliente)

    return unwrapApiResponse(response)
  } catch (error) {
    handleApiError(error)
  }
}

export async function deleteCliente(id) {
  try {
    const response = await httpClient.delete(`/api/clientes/${id}`)

    return unwrapApiResponse(response)
  } catch (error) {
    handleApiError(error)
  }
}
