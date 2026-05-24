import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

function LoginPage() {
  const navigate = useNavigate()
  const { login } = useAuth()

  const [form, setForm] = useState({
    email: '',
    password: '',
  })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  const handleChange = (event) => {
    const { name, value } = event.target
    setForm((current) => ({ ...current, [name]: value }))
  }

  const handleSubmit = async (event) => {
    event.preventDefault()
    setError('')
    setLoading(true)

    try {
      await login(form)
      navigate('/travel-plans')
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  return (
    <main className="min-h-screen bg-[linear-gradient(135deg,#fff4d8_0%,#f3efe7_55%,#dbeafe_100%)] px-4 py-10">
      <div className="mx-auto grid max-w-6xl gap-8 lg:grid-cols-[1.1fr_0.9fr]">
        <section className="rounded-[2rem] border border-white/60 bg-white/70 p-8 shadow-xl backdrop-blur md:p-12">
          <p className="mb-3 inline-block rounded-full bg-amber-200 px-4 py-1 text-sm font-semibold text-amber-950">
            Travel Planner
          </p>
          <h1 className="max-w-xl text-5xl font-black tracking-tight text-slate-900">
            Uloguj se i pretvori plan putovanja u pravu malu komandu.
          </h1>
          <p className="mt-5 max-w-xl text-lg text-slate-700">
            Frontend koristimo kroz komponente, forme, Context i servis sloj, baš kako smo radili na vjezbama.
          </p>
          <div className="mt-10 grid gap-4 md:grid-cols-3">
            <div className="rounded-3xl bg-slate-900 p-5 text-white">
              <p className="text-sm uppercase tracking-[0.2em] text-slate-300">Plan</p>
              <p className="mt-2 text-2xl font-bold">Putovanja</p>
            </div>
            <div className="rounded-3xl bg-amber-300 p-5 text-slate-900">
              <p className="text-sm uppercase tracking-[0.2em] text-amber-900">Troskovi</p>
              <p className="mt-2 text-2xl font-bold">Budzet</p>
            </div>
            <div className="rounded-3xl bg-sky-200 p-5 text-slate-900">
              <p className="text-sm uppercase tracking-[0.2em] text-sky-900">Dani</p>
              <p className="mt-2 text-2xl font-bold">Aktivnosti</p>
            </div>
          </div>
        </section>

        <section className="rounded-[2rem] bg-slate-950 p-8 text-white shadow-2xl md:p-10">
          <h2 className="text-3xl font-bold">Login</h2>
          <p className="mt-2 text-slate-300">Unesi nalog koji si registrovao kroz backend.</p>

          <form className="mt-8 space-y-5" onSubmit={handleSubmit}>
            <label className="block">
              <span className="mb-2 block text-sm font-medium text-slate-200">Email</span>
              <input
                className="w-full rounded-2xl border border-slate-700 bg-slate-900 px-4 py-3 text-white outline-none transition focus:border-amber-300"
                type="email"
                name="email"
                value={form.email}
                onChange={handleChange}
                placeholder="danilo@test.com"
                required
              />
            </label>

            <label className="block">
              <span className="mb-2 block text-sm font-medium text-slate-200">Lozinka</span>
              <input
                className="w-full rounded-2xl border border-slate-700 bg-slate-900 px-4 py-3 text-white outline-none transition focus:border-amber-300"
                type="password"
                name="password"
                value={form.password}
                onChange={handleChange}
                placeholder="test123"
                required
              />
            </label>

            {error ? (
              <div className="rounded-2xl border border-rose-400/40 bg-rose-500/10 px-4 py-3 text-sm text-rose-200">
                {error}
              </div>
            ) : null}

            <button
              className="w-full rounded-2xl bg-amber-300 px-4 py-3 font-bold text-slate-950 transition hover:bg-amber-200 disabled:cursor-not-allowed disabled:opacity-60"
              type="submit"
              disabled={loading}
            >
              {loading ? 'Prijavljivanje...' : 'Uloguj se'}
            </button>
          </form>

          <p className="mt-6 text-sm text-slate-300">
            Nemash nalog?{' '}
            <Link className="font-semibold text-amber-300 hover:text-amber-200" to="/register">
              Registruj se
            </Link>
          </p>
        </section>
      </div>
    </main>
  )
}

export default LoginPage