import { NavLink, Outlet } from 'react-router-dom'

const navigationItems = [
  { to: '/dashboard', label: 'Dashboard' },
  { to: '/clientes', label: 'Clientes' },
  { to: '/servidores', label: 'Servidores' },
  { to: '/telas', label: 'Telas' },
  { to: '/financeiro', label: 'Financeiro' },
  { to: '/historico', label: 'Historico' },
]

function MainLayout() {
  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="sidebar-brand">AccessManager</div>
        <nav className="nav nav-pills flex-column gap-1" aria-label="Navegacao principal">
          {navigationItems.map((item) => (
            <NavLink key={item.to} to={item.to} className="nav-link" end>
              {item.label}
            </NavLink>
          ))}
        </nav>
      </aside>

      <div className="content-shell">
        <header className="topbar">
          <span className="text-muted">Gestao interna de acessos</span>
        </header>
        <main className="content">
          <Outlet />
        </main>
      </div>
    </div>
  )
}

export default MainLayout
