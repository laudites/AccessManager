import httpClient from './httpClient'
import { handleApiError, unwrapApiResponse } from './apiResponse'

export async function getClientes() {
  try {
    const response = await httpClient.get('/api/clientes')

    return unwrapApiResponse(response, [])
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
