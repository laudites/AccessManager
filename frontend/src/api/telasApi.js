import httpClient from './httpClient'
import { handleApiError, unwrapApiResponse } from './apiResponse'

export async function getTelas(filters = {}) {
  try {
    const response = await httpClient.get('/api/telas', {
      params: {
        clienteId: filters.clienteId || undefined,
        servidorId: filters.servidorId || undefined,
      },
    })

    return unwrapApiResponse(response, [])
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
