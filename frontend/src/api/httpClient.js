import axios from 'axios'

const httpClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5025',
  headers: {
    'Content-Type': 'application/json',
  },
})

export default httpClient
