export function getApiErrors(responseData) {
  if (Array.isArray(responseData?.errors) && responseData.errors.length > 0) {
    return responseData.errors.join(' ')
  }

  return 'Erro ao concluir a operacao.'
}

export function unwrapApiResponse(response, fallbackData) {
  if (response.data?.success === false) {
    throw new Error(getApiErrors(response.data))
  }

  return response.data?.data ?? fallbackData
}

export function handleApiError(error) {
  if (error.response?.data) {
    throw new Error(getApiErrors(error.response.data))
  }

  throw new Error('Erro ao conectar com a API.')
}
