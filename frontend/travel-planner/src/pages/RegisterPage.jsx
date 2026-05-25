import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

function RegisterPage() {
  const navigate = useNavigate()
  const { register } = useAuth()

  const [form, setForm] = useState({
    firstName: '',
    lastName: '',
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
      await register(form)
      navigate('/travel-plans')
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  return (
    <main className="relative min-h-screen overflow-hidden bg-[linear-gradient(145deg,#fdf3d7_0%,#f8f6f2_45%,#deebff_100%)] px-4 py-8">
      <div className="pointer-events-none absolute inset-0 opacity-70">
        <div className="absolute right-[-6rem] top-[-5rem] h-72 w-72 rounded-full bg-sky-200/50 blur-3xl" />
        <div className="absolute bottom-[-6rem] left-[-5rem] h-80 w-80 rounded-full bg-amber-200/40 blur-3xl" />
      </div>

      <div className="relative mx-auto max-w-4xl rounded-[2.4rem] border border-white/70 bg-white/78 p-8 shadow-[0_30px_90px_rgba(15,23,42,0.12)] backdrop-blur-xl md:p-12">
        <div className="flex flex-wrap items-start justify-between gap-4">
          <div>
            <p className="inline-flex rounded-full bg-sky-200 px-4 py-1 text-sm font-semibold text-sky-950">
              Novi nalog
            </p>
            <h1 className="mt-5 text-4xl font-black tracking-tight text-slate-900 md:text-5xl">
              Kreiraj svoj profil za planiranje putovanja
            </h1>
            <p className="mt-4 max-w-2xl text-lg leading-8 text-slate-600">
              Napravi nalog i sačuvaj sve planove, troškove, aktivnosti i važne detalje na jednom
              mjestu.
            </p>
          </div>

          <div className="rounded-[1.75rem] bg-slate-950 px-5 py-4 text-white shadow-lg">
            <p className="text-xs uppercase tracking-[0.3em] text-amber-300">Brz početak</p>
            <p className="mt-3 text-lg font-bold">Nalog otvara cijelu aplikaciju</p>
          </div>
        </div>

        <form className="mt-10 grid gap-5 md:grid-cols-2" onSubmit={handleSubmit}>
          <label className="block">
            <span className="mb-2 block text-sm font-medium text-slate-700">Ime</span>
            <input
              className="w-full rounded-[1.4rem] border border-slate-300 bg-white px-4 py-3 outline-none transition focus:border-sky-500 focus:shadow-[0_0_0_4px_rgba(186,230,253,0.35)]"
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
              className="w-full rounded-[1.4rem] border border-slate-300 bg-white px-4 py-3 outline-none transition focus:border-sky-500 focus:shadow-[0_0_0_4px_rgba(186,230,253,0.35)]"
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
              className="w-full rounded-[1.4rem] border border-slate-300 bg-white px-4 py-3 outline-none transition focus:border-sky-500 focus:shadow-[0_0_0_4px_rgba(186,230,253,0.35)]"
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
              className="w-full rounded-[1.4rem] border border-slate-300 bg-white px-4 py-3 outline-none transition focus:border-sky-500 focus:shadow-[0_0_0_4px_rgba(186,230,253,0.35)]"
              type="password"
              name="password"
              value={form.password}
              onChange={handleChange}
              placeholder="Unesi lozinku"
              required
            />
          </label>

          {error ? (
            <div className="rounded-[1.4rem] border border-rose-300 bg-rose-50 px-4 py-3 text-sm text-rose-700 md:col-span-2">
              {error}
            </div>
          ) : null}

          <button
            className="rounded-[1.4rem] bg-slate-950 px-4 py-3.5 font-bold text-white shadow-lg transition hover:bg-slate-900 disabled:cursor-not-allowed disabled:opacity-60 md:col-span-2"
            type="submit"
            disabled={loading}
          >
            {loading ? 'Registracija je u toku...' : 'Kreiraj nalog'}
          </button>
        </form>

        <p className="mt-7 text-sm text-slate-600">
          Već imaš nalog?{' '}
          <Link className="font-semibold text-sky-700 hover:text-sky-900" to="/login">
            Idi na prijavu
          </Link>
        </p>
      </div>
    </main>
  )
}

export default RegisterPage
