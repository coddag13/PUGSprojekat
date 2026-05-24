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
    <main className="min-h-screen bg-[linear-gradient(160deg,#fdf2f8_0%,#eff6ff_50%,#fef3c7_100%)] px-4 py-10">
      <div className="mx-auto max-w-3xl rounded-[2rem] border border-white/60 bg-white/75 p-8 shadow-2xl backdrop-blur md:p-10">
        <p className="mb-3 inline-block rounded-full bg-sky-200 px-4 py-1 text-sm font-semibold text-sky-950">
          Novi nalog
        </p>
        <h1 className="text-4xl font-black tracking-tight text-slate-900">
          Registracija za Travel Planner
        </h1>
        <p className="mt-3 text-slate-700">
          Controlled forme, jedan state objekat i jasan tok: input → handler → state → API.
        </p>

        <form className="mt-8 grid gap-5 md:grid-cols-2" onSubmit={handleSubmit}>
          <label className="block">
            <span className="mb-2 block text-sm font-medium text-slate-700">Ime</span>
            <input
              className="w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 outline-none transition focus:border-sky-500"
              type="text"
              name="firstName"
              value={form.firstName}
              onChange={handleChange}
              required
            />
          </label>

          <label className="block">
            <span className="mb-2 block text-sm font-medium text-slate-700">Prezime</span>
            <input
              className="w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 outline-none transition focus:border-sky-500"
              type="text"
              name="lastName"
              value={form.lastName}
              onChange={handleChange}
              required
            />
          </label>

          <label className="block md:col-span-2">
            <span className="mb-2 block text-sm font-medium text-slate-700">Email</span>
            <input
              className="w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 outline-none transition focus:border-sky-500"
              type="email"
              name="email"
              value={form.email}
              onChange={handleChange}
              required
            />
          </label>

          <label className="block md:col-span-2">
            <span className="mb-2 block text-sm font-medium text-slate-700">Lozinka</span>
            <input
              className="w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 outline-none transition focus:border-sky-500"
              type="password"
              name="password"
              value={form.password}
              onChange={handleChange}
              required
            />
          </label>

          {error ? (
            <div className="rounded-2xl border border-rose-300 bg-rose-50 px-4 py-3 text-sm text-rose-700 md:col-span-2">
              {error}
            </div>
          ) : null}

          <button
            className="rounded-2xl bg-slate-900 px-4 py-3 font-bold text-white transition hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-60 md:col-span-2"
            type="submit"
            disabled={loading}
          >
            {loading ? 'Registracija u toku...' : 'Kreiraj nalog'}
          </button>
        </form>

        <p className="mt-6 text-sm text-slate-600">
          Već imaš nalog?{' '}
          <Link className="font-semibold text-sky-700 hover:text-sky-900" to="/login">
            Idi na login
          </Link>
        </p>
      </div>
    </main>
  )
}

export default RegisterPage