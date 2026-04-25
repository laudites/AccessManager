import { Link } from 'react-router-dom'

function LoginPage() {
  return (
    <main className="login-page">
      <section className="login-panel">
        <h1>AccessManager</h1>
        <p className="text-muted mb-4">Base inicial do frontend.</p>
        <Link className="btn btn-primary" to="/dashboard">
          Entrar
        </Link>
      </section>
    </main>
  )
}

export default LoginPage
