import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { createEmptyRegisterForm } from '../models'
import { createFormValidationHandlers } from '../utils/formValidation'

function RegisterPage() {
  const navigate = useNavigate()
  const { register } = useAuth()
  const formValidation = createFormValidationHandlers()

  const [form, setForm] = useState(createEmptyRegisterForm)
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
      await register(form)
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
        <div className="absolute right-[-8rem] top-[-6rem] h-80 w-80 rounded-full bg-sky-300/26 blur-3xl" />
        <div className="absolute bottom-[-8rem] left-[-6rem] h-72 w-72 rounded-full bg-amber-200/26 blur-3xl" />
      </div>

      <div className="relative mx-auto max-w-6xl rounded-[2.6rem] border border-white/40 bg-slate-950/94 p-3 shadow-[0_40px_120px_rgba(15,23,42,0.28)]">
        <div className="grid gap-0 overflow-hidden rounded-[2.2rem] lg:grid-cols-[0.9fr_1.1fr]">
          <section className="dark-signal-panel p-8 text-white md:p-10">
            <span className="signal-chip inline-flex rounded-full px-4 py-1.5 text-sm font-semibold text-sky-200">
              Novi nalog
            </span>

            <h1 className="travel-heading mt-8 text-5xl font-black md:text-6xl">
              Otvori svoj putni kokpit i sačuvaj svaki plan na jednom mjestu.
            </h1>

            <p className="mt-6 text-lg leading-8 text-slate-300">
              Kreiraj profil i odmah dobijaš prostor za destinacije, aktivnosti, budžet, listu
              stvari i dijeljenje plana sa drugima.
            </p>

            <div className="mt-12 space-y-4">
              <div className="rounded-[1.6rem] border border-white/10 bg-white/5 p-4">
                <p className="text-sm font-semibold text-white">Jedan pregled za cijelo putovanje</p>
                <p className="mt-2 text-sm leading-6 text-slate-300">
                  Svi detalji puta, od prvog datuma do posljednje stavke budžeta.
                </p>
              </div>

              <div className="rounded-[1.6rem] border border-white/10 bg-white/5 p-4">
                <p className="text-sm font-semibold text-white">Pametno planiranje po danima</p>
                <p className="mt-2 text-sm leading-6 text-slate-300">
                  Aktivnosti i raspored ostaju jasni čak i kada plan postane bogat detaljima.
                </p>
              </div>

              <div className="rounded-[1.6rem] border border-white/10 bg-white/5 p-4">
                <p className="text-sm font-semibold text-white">Budžet pod kontrolom</p>
                <p className="mt-2 text-sm leading-6 text-slate-300">
                  Troškovi, procjene i preostali budžet uvijek su vidljivi i pregledni.
                </p>
              </div>
            </div>
          </section>

          <section className="glass-panel rounded-[2rem] p-8 md:p-10">
            <div className="flex flex-wrap items-center justify-between gap-3">
              <div>
                <p className="text-sm uppercase tracking-[0.32em] text-sky-700">Registracija</p>
                <h2 className="mt-3 text-4xl font-black tracking-tight text-slate-950">
                  Kreiraj profil
                </h2>
              </div>

              <div className="rounded-[1.4rem] bg-[linear-gradient(135deg,#fef3c7_0%,#e0f2fe_100%)] px-4 py-3 text-sm font-semibold text-slate-900 shadow-sm">
                Spremno za prvo putovanje
              </div>
            </div>

            <form
              className="mt-8 grid gap-5 md:grid-cols-2"
              onSubmit={handleSubmit}
              {...formValidation}
            >
              <label className="block">
                <span className="mb-2 block text-sm font-medium text-slate-700">Ime</span>
                <input
                  className="w-full rounded-[1.35rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-sky-500 focus:shadow-[0_0_0_4px_rgba(186,230,253,0.35)]"
                  type="text"
                  name="firstName"
                  value={form.firstName}
                  onChange={handleChange}
                  placeholder="Unesi ime"
                  required
                />
              </label>

              <label className="block">
                <span className="mb-2 block text-sm font-medium text-slate-700">Prezime</span>
                <input
                  className="w-full rounded-[1.35rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-sky-500 focus:shadow-[0_0_0_4px_rgba(186,230,253,0.35)]"
                  type="text"
                  name="lastName"
                  value={form.lastName}
                  onChange={handleChange}
                  placeholder="Unesi prezime"
                  required
                />
              </label>

              <label className="block md:col-span-2">
                <span className="mb-2 block text-sm font-medium text-slate-700">Email adresa</span>
                <input
                  className="w-full rounded-[1.35rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-sky-500 focus:shadow-[0_0_0_4px_rgba(186,230,253,0.35)]"
                  type="email"
                  name="email"
                  value={form.email}
                  onChange={handleChange}
                  placeholder="unesi@email.com"
                  required
                />
              </label>

              <label className="block md:col-span-2">
                <span className="mb-2 block text-sm font-medium text-slate-700">Lozinka</span>
                <input
                  className="w-full rounded-[1.35rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-sky-500 focus:shadow-[0_0_0_4px_rgba(186,230,253,0.35)]"
                  type="password"
                  name="password"
                  value={form.password}
                  onChange={handleChange}
                  placeholder="Unesi lozinku"
                  required
                />
              </label>

              {error ? (
                <div className="rounded-[1.3rem] border border-rose-300 bg-rose-50 px-4 py-3 text-sm text-rose-700 md:col-span-2">
                  {error}
                </div>
              ) : null}

              <button
                className="rounded-[1.35rem] bg-slate-950 px-4 py-3.5 font-bold text-white shadow-[0_18px_40px_rgba(15,23,42,0.16)] transition hover:bg-slate-900 disabled:cursor-not-allowed disabled:opacity-60 md:col-span-2"
                type="submit"
                disabled={loading}
              >
                {loading ? 'Registracija je u toku...' : 'Kreiraj nalog'}
              </button>
            </form>

            <p className="mt-7 text-sm text-slate-600">
              Već imaš nalog?{' '}
              <Link className="font-semibold text-sky-700 hover:text-sky-900" to="/login">
                Vrati se na prijavu
              </Link>
            </p>
          </section>
        </div>
      </div>
    </main>
  )
}

export default RegisterPage
