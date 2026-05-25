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
    <main className="relative min-h-screen overflow-hidden bg-[linear-gradient(135deg,#fff4d8_0%,#f6efe3_50%,#d9e9ff_100%)] px-4 py-8">
      <div className="pointer-events-none absolute inset-0 opacity-70">
        <div className="absolute left-[-8rem] top-[-6rem] h-72 w-72 rounded-full bg-amber-200/40 blur-3xl" />
        <div className="absolute bottom-[-8rem] right-[-6rem] h-80 w-80 rounded-full bg-sky-200/50 blur-3xl" />
      </div>

      <div className="relative mx-auto grid max-w-6xl gap-8 lg:grid-cols-[1.15fr_0.85fr]">
        <section className="rounded-[2.25rem] border border-white/70 bg-white/70 p-8 shadow-[0_25px_80px_rgba(15,23,42,0.12)] backdrop-blur-xl md:p-12">
          <p className="inline-flex rounded-full bg-amber-200 px-4 py-1 text-sm font-semibold text-amber-950 shadow-sm">
            Travel Planner
          </p>

          <h1 className="mt-6 max-w-2xl text-5xl font-black leading-[0.95] tracking-tight text-slate-900 md:text-6xl">
            Organizuj putovanje pregledno, jasno i bez stresa.
          </h1>

          <p className="mt-6 max-w-xl text-lg leading-8 text-slate-600">
            Na jednom mjestu prati destinacije, aktivnosti, troškove, budžet i sve važne detalje
            puta.
          </p>

          <div className="mt-10 grid gap-4 md:grid-cols-3">
            <div className="rounded-[1.75rem] bg-slate-900 p-5 text-white shadow-lg">
              <p className="text-xs uppercase tracking-[0.3em] text-slate-300">Planovi</p>
              <p className="mt-3 text-2xl font-bold">Putovanja</p>
            </div>

            <div className="rounded-[1.75rem] bg-amber-300 p-5 text-slate-950 shadow-lg">
              <p className="text-xs uppercase tracking-[0.3em] text-amber-950/70">Budžet</p>
              <p className="mt-3 text-2xl font-bold">Troškovi</p>
            </div>

            <div className="rounded-[1.75rem] bg-sky-200 p-5 text-slate-950 shadow-lg">
              <p className="text-xs uppercase tracking-[0.3em] text-sky-950/70">Raspored</p>
              <p className="mt-3 text-2xl font-bold">Aktivnosti</p>
            </div>
          </div>
        </section>

        <section className="rounded-[2.25rem] bg-slate-950 p-8 text-white shadow-[0_30px_90px_rgba(15,23,42,0.28)] md:p-10">
          <p className="text-sm uppercase tracking-[0.3em] text-amber-300">Prijava</p>
          <h2 className="mt-4 text-4xl font-black tracking-tight">Dobro došao nazad</h2>
          <p className="mt-3 text-slate-300">
            Prijavi se i nastavi sa planiranjem svog narednog putovanja.
          </p>

          <form className="mt-10 space-y-5" onSubmit={handleSubmit}>
            <label className="block">
              <span className="mb-2 block text-sm font-medium text-slate-200">Email adresa</span>
              <input
                className="w-full rounded-[1.4rem] border border-slate-700 bg-white/5 px-4 py-3 text-white outline-none transition placeholder:text-slate-500 focus:border-amber-300 focus:bg-white/10"
                type="email"
                name="email"
                value={form.email}
                onChange={handleChange}
                placeholder="unesi@email.com"
                required
              />
            </label>

            <label className="block">
              <span className="mb-2 block text-sm font-medium text-slate-200">Lozinka</span>
              <input
                className="w-full rounded-[1.4rem] border border-slate-700 bg-white/5 px-4 py-3 text-white outline-none transition placeholder:text-slate-500 focus:border-amber-300 focus:bg-white/10"
                type="password"
                name="password"
                value={form.password}
                onChange={handleChange}
                placeholder="Unesi lozinku"
                required
              />
            </label>

            {error ? (
              <div className="rounded-[1.4rem] border border-rose-400/30 bg-rose-500/10 px-4 py-3 text-sm text-rose-200">
                {error}
              </div>
            ) : null}

            <button
              className="w-full rounded-[1.4rem] bg-amber-300 px-4 py-3.5 font-bold text-slate-950 shadow-lg transition hover:bg-amber-200 disabled:cursor-not-allowed disabled:opacity-60"
              type="submit"
              disabled={loading}
            >
              {loading ? 'Prijava je u toku...' : 'Uloguj se'}
            </button>
          </form>

          <p className="mt-7 text-sm text-slate-300">
            Nemaš nalog?{' '}
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
