import httpClient from './httpClient'

export async function getDashboardResumo() {
  const response = await httpClient.get('/api/dashboard/resumo')

  return response.data.data
}

export async function getTelasPorServidor() {
  const response = await httpClient.get('/api/dashboard/telas-por-servidor')

  return response.data.data
}
