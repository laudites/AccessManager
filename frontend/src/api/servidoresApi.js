import httpClient from './httpClient'
import { handleApiError, unwrapApiResponse } from './apiResponse'

export async function getServidores() {
  try {
    const response = await httpClient.get('/api/servidores')

    return unwrapApiResponse(response, [])
  } catch (error) {
    handleApiError(error)
  }
}

export async function getServidorById(id) {
  try {
    const response = await httpClient.get(`/api/servidores/${id}`)

    return unwrapApiResponse(response)
  } catch (error) {
    handleApiError(error)
  }
}

export async function createServidor(servidor) {
  try {
    const response = await httpClient.post('/api/servidores', servidor)

    return unwrapApiResponse(response)
  } catch (error) {
    handleApiError(error)
  }
}

export async function updateServidor(id, servidor) {
  try {
    const response = await httpClient.put(`/api/servidores/${id}`, servidor)

    return unwrapApiResponse(response)
  } catch (error) {
    handleApiError(error)
  }
}

export async function deleteServidor(id) {
  try {
    const response = await httpClient.delete(`/api/servidores/${id}`)

    return unwrapApiResponse(response)
  } catch (error) {
    handleApiError(error)
  }
}

export async function updateServidorStatus(id, status) {
  try {
    const response = await httpClient.patch(`/api/servidores/${id}/status`, { status })

    return unwrapApiResponse(response)
  } catch (error) {
    handleApiError(error)
  }
}
