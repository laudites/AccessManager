import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import MainLayout from '../layouts/MainLayout'
import LoginPage from '../pages/Login/LoginPage'
import DashboardPage from '../pages/Dashboard/DashboardPage'
import ClientesPage from '../pages/Clientes/ClientesPage'
import ServidoresPage from '../pages/Servidores/ServidoresPage'
import TelasPage from '../pages/Telas/TelasPage'
import FinanceiroPage from '../pages/Financeiro/FinanceiroPage'
import HistoricoPage from '../pages/Historico/HistoricoPage'

function AppRoutes() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<LoginPage />} />
        <Route element={<MainLayout />}>
          <Route path="/dashboard" element={<DashboardPage />} />
          <Route path="/clientes" element={<ClientesPage />} />
          <Route path="/servidores" element={<ServidoresPage />} />
          <Route path="/telas" element={<TelasPage />} />
          <Route path="/financeiro" element={<FinanceiroPage />} />
          <Route path="/historico" element={<HistoricoPage />} />
        </Route>
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </BrowserRouter>
  )
}

export default AppRoutes
