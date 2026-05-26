import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { createEmptyLoginForm } from '../models'
import { createFormValidationHandlers } from '../utils/formValidation'

function LoginPage() {
  const navigate = useNavigate()
  const { login } = useAuth()
  const formValidation = createFormValidationHandlers()

  const [form, setForm] = useState(createEmptyLoginForm)
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
    <main className="travel-shell relative min-h-screen overflow-hidden px-4 py-8">
      <div className="pointer-events-none absolute inset-0 opacity-90">
        <div className="absolute left-[-8rem] top-[-7rem] h-80 w-80 rounded-full bg-amber-200/30 blur-3xl" />
        <div className="absolute right-[-5rem] top-[6rem] h-72 w-72 rounded-full bg-sky-300/25 blur-3xl" />
        <div className="absolute bottom-[-10rem] left-[24%] h-96 w-96 rounded-full bg-slate-950/10 blur-3xl" />
      </div>

      <div className="relative mx-auto grid max-w-7xl gap-8 lg:grid-cols-[1.05fr_0.95fr]">
        <section className="dark-signal-panel rounded-[2.5rem] p-8 text-white md:p-12">
          <div className="flex flex-wrap items-center gap-4">
            <span className="signal-chip inline-flex rounded-full px-4 py-1.5 text-sm font-semibold text-amber-200">
              Planer putovanja
            </span>
            <span className="rounded-full border border-white/10 bg-white/5 px-4 py-1.5 text-xs uppercase tracking-[0.35em] text-sky-200">
              Pametna ruta
            </span>
          </div>

          <div className="mt-10 max-w-3xl">
            <p className="text-sm uppercase tracking-[0.35em] text-amber-300">Putna komanda</p>
            <h1 className="travel-heading mt-5 text-5xl font-black text-white md:text-7xl">
              Planiraj rutu, budžet i dane putovanja iz jednog mjesta.
            </h1>
            <p className="mt-6 max-w-2xl text-lg leading-8 text-slate-300">
              Organizuj destinacije, prati aktivnosti po danima i zadrži potpun pregled nad
              troškovima bez rasutih bilješki i izgubljenih detalja.
            </p>
          </div>

          <div className="mt-12 grid gap-4 md:grid-cols-3">
            <div className="route-kpi rounded-[1.8rem] bg-white/6 p-5 backdrop-blur-sm">
              <p className="text-xs uppercase tracking-[0.35em] text-slate-300">Mreža</p>
              <p className="mt-3 text-2xl font-black">Destinacije</p>
              <p className="mt-2 text-sm text-slate-300">Sve stanice puta u jednom pregledu.</p>
            </div>

            <div className="route-kpi rounded-[1.8rem] bg-amber-300/90 p-5 text-slate-950">
              <p className="text-xs uppercase tracking-[0.35em] text-amber-950/70">Budžet</p>
              <p className="mt-3 text-2xl font-black">Troškovi</p>
              <p className="mt-2 text-sm text-amber-950/80">Kontrola svake stavke i plana.</p>
            </div>

            <div className="route-kpi rounded-[1.8rem] bg-sky-300/85 p-5 text-slate-950">
              <p className="text-xs uppercase tracking-[0.35em] text-sky-950/70">Raspored</p>
              <p className="mt-3 text-2xl font-black">Aktivnosti</p>
              <p className="mt-2 text-sm text-sky-950/80">Dan po dan, pregledno i precizno.</p>
            </div>
          </div>

          <div className="mt-12 grid gap-4 md:grid-cols-3">
            <div className="rounded-[1.6rem] border border-white/10 bg-white/5 p-4">
              <p className="text-sm font-semibold text-white">Pametan pregled</p>
              <p className="mt-2 text-sm leading-6 text-slate-300">
                Plan, budžet, checklista i dijeljenje plana povezani na jednom mjestu.
              </p>
            </div>

            <div className="rounded-[1.6rem] border border-white/10 bg-white/5 p-4">
              <p className="text-sm font-semibold text-white">Jasan ritam dana</p>
              <p className="mt-2 text-sm leading-6 text-slate-300">
                Aktivnosti grupisane po datumu i spremne za brzu izmjenu statusa.
              </p>
            </div>

            <div className="rounded-[1.6rem] border border-white/10 bg-white/5 p-4">
              <p className="text-sm font-semibold text-white">Spremno za dijeljenje</p>
              <p className="mt-2 text-sm leading-6 text-slate-300">
                QR i link pristup omogućavaju da plan lako podijeliš sa ekipom.
              </p>
            </div>
          </div>
        </section>

        <section className="glass-panel rounded-[2.5rem] p-8 md:p-10">
          <div className="rounded-[2rem] border border-slate-200/80 bg-slate-950 p-3 shadow-[0_24px_60px_rgba(15,23,42,0.18)]">
            <div className="grid grid-cols-2 gap-2 rounded-[1.4rem] border border-white/5 bg-white/[0.03] p-1.5">
              <div className="rounded-[1rem] bg-[linear-gradient(90deg,rgba(251,191,36,0.12),rgba(56,189,248,0.14))] px-4 py-3 text-center text-sm font-semibold text-white">
                Prijava
              </div>
              <Link
                className="rounded-[1rem] px-4 py-3 text-center text-sm font-semibold text-slate-400 transition hover:bg-white/5 hover:text-white"
                to="/register"
              >
                Registracija
              </Link>
            </div>

            <div className="mt-8 rounded-[1.8rem] border border-white/6 bg-black/18 p-6 text-white md:p-8">
              <p className="text-sm uppercase tracking-[0.32em] text-amber-300">Pristup nalogu</p>
              <h2 className="mt-4 text-4xl font-black tracking-tight">Uloguj se u kontrolni centar</h2>
              <p className="mt-3 max-w-lg text-slate-300">
                Nastavi sa planiranjem naredne rute, uređivanjem aktivnosti i pregledom troškova.
              </p>

              <form className="mt-9 space-y-5" onSubmit={handleSubmit} {...formValidation}>
                <label className="block">
                  <span className="mb-2 block text-sm font-medium text-slate-200">Email adresa</span>
                  <input
                    className="w-full rounded-[1.35rem] border border-slate-700 bg-white/5 px-4 py-3 text-white outline-none transition placeholder:text-slate-500 focus:border-amber-300 focus:bg-white/10"
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
                    className="w-full rounded-[1.35rem] border border-slate-700 bg-white/5 px-4 py-3 text-white outline-none transition placeholder:text-slate-500 focus:border-amber-300 focus:bg-white/10"
                    type="password"
                    name="password"
                    value={form.password}
                    onChange={handleChange}
                    placeholder="Unesi lozinku"
                    required
                  />
                </label>

                {error ? (
                  <div className="rounded-[1.3rem] border border-rose-400/30 bg-rose-500/10 px-4 py-3 text-sm text-rose-200">
                    {error}
                  </div>
                ) : null}

                <button
                  className="w-full rounded-[1.35rem] bg-[linear-gradient(90deg,#facc15_0%,#f59e0b_28%,#7dd3fc_100%)] px-4 py-3.5 font-bold text-slate-950 shadow-[0_20px_40px_rgba(251,191,36,0.2)] transition hover:brightness-105 disabled:cursor-not-allowed disabled:opacity-60"
                  type="submit"
                  disabled={loading}
                >
                  {loading ? 'Prijava je u toku...' : 'Uloguj se'}
                </button>
              </form>

              <p className="mt-7 text-sm text-slate-300">
                Nemaš nalog?{' '}
                <Link className="font-semibold text-amber-300 hover:text-amber-200" to="/register">
                  Kreiraj profil
                </Link>
              </p>
            </div>
          </div>
        </section>
      </div>
    </main>
  )
}

export default LoginPage
