import httpClient from './httpClient'
import { handleApiError, unwrapApiResponse } from './apiResponse'

export async function getLancamentosFinanceiros(filters = {}) {
  try {
    const response = await httpClient.get('/api/lancamentos-financeiros', {
      params: {
        clienteId: filters.clienteId || undefined,
        telaClienteId: filters.telaClienteId || undefined,
        statusFinanceiro: filters.statusFinanceiro || undefined,
        mes: filters.mes || undefined,
        ano: filters.ano || undefined,
      },
    })

    return unwrapApiResponse(response, [])
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

    return unwrapApiResponse(response, [])
  } catch (error) {
    handleApiError(error)
  }
}

export async function getLancamentosAtrasados() {
  try {
    const response = await httpClient.get('/api/lancamentos-financeiros/atrasados')

    return unwrapApiResponse(response, [])
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

export async function gerarLancamentosPendentes(dataReferencia) {
  try {
    const response = await httpClient.post('/api/lancamentos-financeiros/gerar-pendentes', {
      dataReferencia: dataReferencia || null,
    })

    return unwrapApiResponse(response, [])
  } catch (error) {
    handleApiError(error)
  }
}
