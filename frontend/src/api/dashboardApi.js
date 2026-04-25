import httpClient from './httpClient'
import { unwrapApiResponse } from './apiResponse'

export async function getDashboardResumo() {
  const response = await httpClient.get('/api/dashboard/resumo')

  return unwrapApiResponse(response)
}

export async function getTelasPorServidor() {
  const response = await httpClient.get('/api/dashboard/telas-por-servidor')

  return unwrapApiResponse(response, [])
}
