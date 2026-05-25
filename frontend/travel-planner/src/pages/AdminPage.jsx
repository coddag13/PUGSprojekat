import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import {
  deleteAdminTravelPlan,
  deleteAdminUser,
  getAdminTravelPlans,
  getAdminUsers,
  updateAdminUserRole,
} from '../services/adminService'

function AdminPage() {
  const { user, logout } = useAuth()
  const [users, setUsers] = useState([])
  const [plans, setPlans] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')
  const [updatingUserId, setUpdatingUserId] = useState(null)
  const [deletingUserId, setDeletingUserId] = useState(null)
  const [deletingPlanId, setDeletingPlanId] = useState(null)

  const loadAdminData = async () => {
    setLoading(true)
    setError('')

    try {
      const [usersData, plansData] = await Promise.all([getAdminUsers(), getAdminTravelPlans()])
      setUsers(usersData)
      setPlans(plansData)
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadAdminData()
  }, [])

  const handleRoleChange = async (targetUser, nextRole) => {
    setError('')
    setSuccess('')
    setUpdatingUserId(targetUser.id)

    try {
      await updateAdminUserRole(targetUser.id, { role: nextRole })
      setSuccess('Uloga korisnika je uspješno ažurirana.')
      await loadAdminData()
    } catch (err) {
      setError(err.message)
    } finally {
      setUpdatingUserId(null)
    }
  }

  const handleDeleteUser = async (targetUser) => {
    const confirmed = window.confirm(
      `Da li sigurno želiš da obrišeš korisnika ${targetUser.firstName} ${targetUser.lastName}?`,
    )

    if (!confirmed) {
      return
    }

    setError('')
    setSuccess('')
    setDeletingUserId(targetUser.id)

    try {
      await deleteAdminUser(targetUser.id)
      setSuccess('Korisnički nalog je obrisan.')
      await loadAdminData()
    } catch (err) {
      setError(err.message)
    } finally {
      setDeletingUserId(null)
    }
  }

  const handleDeletePlan = async (plan) => {
    const confirmed = window.confirm(`Da li sigurno želiš da obrišeš plan "${plan.title}"?`)

    if (!confirmed) {
      return
    }

    setError('')
    setSuccess('')
    setDeletingPlanId(plan.id)

    try {
      await deleteAdminTravelPlan(plan.id)
      setSuccess('Plan putovanja je obrisan.')
      await loadAdminData()
    } catch (err) {
      setError(err.message)
    } finally {
      setDeletingPlanId(null)
    }
  }

  return (
    <main className="min-h-screen bg-[linear-gradient(180deg,#fff8ee_0%,#eef6ff_100%)] px-4 py-8">
      <div className="mx-auto max-w-7xl space-y-6">
        <header className="relative overflow-hidden rounded-[2.25rem] bg-slate-950 px-6 py-7 text-white shadow-[0_30px_80px_rgba(15,23,42,0.28)]">
          <div className="pointer-events-none absolute right-[-3rem] top-[-3rem] h-40 w-40 rounded-full bg-amber-300/20 blur-3xl" />
          <div className="pointer-events-none absolute bottom-[-4rem] left-[-2rem] h-44 w-44 rounded-full bg-sky-300/15 blur-3xl" />

          <div className="relative flex flex-wrap items-start justify-between gap-4">
            <div>
              <Link
                className="inline-flex rounded-2xl border border-white/20 px-4 py-2 text-sm font-semibold text-white transition hover:bg-white/10"
                to="/travel-plans"
              >
                Nazad na planove
              </Link>
              <p className="mt-6 text-sm uppercase tracking-[0.3em] text-amber-300">Administracija</p>
              <h1 className="mt-3 text-4xl font-black tracking-tight md:text-5xl">
                Pregled sistema
              </h1>
              <p className="mt-3 max-w-2xl text-slate-300">
                Pregledaj korisničke naloge i planove putovanja na jednom mjestu.
              </p>
            </div>

            <button
              className="rounded-2xl border border-white/15 px-5 py-3 font-semibold text-white transition hover:bg-white/10"
              type="button"
              onClick={logout}
            >
              Odjavi se
            </button>
          </div>
        </header>

        <section className="grid gap-4 md:grid-cols-3">
          <div className="rounded-[2rem] border border-white/70 bg-white/85 p-5 shadow-[0_18px_50px_rgba(15,23,42,0.08)]">
            <p className="text-sm uppercase tracking-[0.2em] text-slate-500">Korisnici</p>
            <p className="mt-3 text-3xl font-black text-slate-900">{users.length}</p>
          </div>
          <div className="rounded-[2rem] border border-white/70 bg-white/85 p-5 shadow-[0_18px_50px_rgba(15,23,42,0.08)]">
            <p className="text-sm uppercase tracking-[0.2em] text-slate-500">Planovi</p>
            <p className="mt-3 text-3xl font-black text-slate-900">{plans.length}</p>
          </div>
          <div className="rounded-[2rem] border border-white/70 bg-white/85 p-5 shadow-[0_18px_50px_rgba(15,23,42,0.08)]">
            <p className="text-sm uppercase tracking-[0.2em] text-slate-500">Administratori</p>
            <p className="mt-3 text-3xl font-black text-slate-900">
              {users.filter((item) => item.role === 'Admin').length}
            </p>
          </div>
        </section>

        {error ? (
          <div className="rounded-2xl border border-rose-300 bg-rose-50 px-4 py-3 text-sm text-rose-700">
            {error}
          </div>
        ) : null}

        {success ? (
          <div className="rounded-2xl border border-emerald-300 bg-emerald-50 px-4 py-3 text-sm text-emerald-700">
            {success}
          </div>
        ) : null}

        <div className="grid gap-6 xl:grid-cols-[1.1fr_0.9fr]">
          <section className="rounded-[2rem] border border-white/70 bg-white/85 p-6 shadow-[0_18px_50px_rgba(15,23,42,0.08)]">
            <div className="mb-5 flex items-center justify-between">
              <h2 className="text-2xl font-black text-slate-900">Korisnički nalozi</h2>
              <button
                className="rounded-2xl border border-slate-300 px-4 py-2 text-sm font-semibold text-slate-700 transition hover:bg-slate-50"
                type="button"
                onClick={loadAdminData}
              >
                Osvježi
              </button>
            </div>

            {loading ? (
              <div className="rounded-3xl border border-dashed border-slate-300 p-10 text-center text-slate-500">
                Učitavanje korisnika...
              </div>
            ) : (
              <div className="grid gap-4">
                {users.map((item) => (
                  <article
                    key={item.id}
                    className="rounded-3xl border border-slate-200 bg-[linear-gradient(135deg,#fffaf0_0%,#f8fbff_100%)] p-5 shadow-sm"
                  >
                    <div className="flex flex-wrap items-start justify-between gap-3">
                      <div>
                        <h3 className="text-lg font-bold text-slate-900">
                          {item.firstName} {item.lastName}
                        </h3>
                        <p className="mt-2 text-slate-700">{item.email}</p>
                        <div className="mt-3 flex flex-wrap gap-3 text-sm">
                          <span className="rounded-full bg-sky-100 px-3 py-1 text-sky-900">
                            Uloga: {item.role === 'Admin' ? 'Administrator' : 'Korisnik'}
                          </span>
                          <span className="rounded-full bg-amber-100 px-3 py-1 text-amber-900">
                            Planovi: {item.travelPlansCount}
                          </span>
                        </div>
                      </div>

                      <button
                        className="rounded-2xl bg-rose-600 px-4 py-2 text-sm font-bold text-white transition hover:bg-rose-700 disabled:cursor-not-allowed disabled:opacity-60"
                        type="button"
                        onClick={() => handleDeleteUser(item)}
                        disabled={deletingUserId === item.id}
                      >
                        {deletingUserId === item.id ? 'Briše se...' : 'Obriši nalog'}
                      </button>
                    </div>

                    <div className="mt-5 grid gap-3 md:grid-cols-[1fr_auto] md:items-end">
                      <label className="block">
                        <span className="mb-2 block text-sm font-semibold text-slate-700">
                          Promijeni ulogu
                        </span>
                        <select
                          className="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none transition focus:border-amber-500"
                          value={item.role}
                          onChange={(event) => handleRoleChange(item, event.target.value)}
                          disabled={updatingUserId === item.id}
                        >
                          <option value="User">Korisnik</option>
                          <option value="Admin">Administrator</option>
                        </select>
                      </label>
                    </div>
                  </article>
                ))}
              </div>
            )}
          </section>

          <section className="rounded-[2rem] border border-white/70 bg-white/85 p-6 shadow-[0_18px_50px_rgba(15,23,42,0.08)]">
            <div className="mb-5 flex items-center justify-between">
              <h2 className="text-2xl font-black text-slate-900">Planovi u sistemu</h2>
              <button
                className="rounded-2xl border border-slate-300 px-4 py-2 text-sm font-semibold text-slate-700 transition hover:bg-slate-50"
                type="button"
                onClick={loadAdminData}
              >
                Osvježi
              </button>
            </div>

            {loading ? (
              <div className="rounded-3xl border border-dashed border-slate-300 p-10 text-center text-slate-500">
                Učitavanje planova...
              </div>
            ) : (
              <div className="grid gap-4">
                {plans.map((plan) => (
                  <article
                    key={plan.id}
                    className="rounded-3xl border border-slate-200 bg-[linear-gradient(135deg,#fffaf0_0%,#f8fbff_100%)] p-5 shadow-sm"
                  >
                    <div className="flex flex-wrap items-start justify-between gap-3">
                      <div>
                        <h3 className="text-lg font-bold text-slate-900">{plan.title}</h3>
                        <p className="mt-2 text-slate-700">{plan.description}</p>
                        <div className="mt-3 flex flex-wrap gap-3 text-sm">
                          <span className="rounded-full bg-sky-100 px-3 py-1 text-sky-900">
                            {plan.startDate.slice(0, 10)} - {plan.endDate.slice(0, 10)}
                          </span>
                          <span className="rounded-full bg-amber-100 px-3 py-1 text-amber-900">
                            {plan.plannedBudget} EUR
                          </span>
                        </div>
                        <p className="mt-3 text-sm text-slate-600">
                          Vlasnik: {plan.ownerName} ({plan.ownerEmail})
                        </p>
                      </div>

                      <div className="flex flex-wrap gap-3">
                        <Link
                          className="rounded-2xl border border-slate-300 px-4 py-2 text-sm font-semibold text-slate-700 transition hover:bg-slate-50"
                          to={`/travel-plans/${plan.id}`}
                        >
                          Otvori plan
                        </Link>

                        <button
                          className="rounded-2xl bg-rose-600 px-4 py-2 text-sm font-bold text-white transition hover:bg-rose-700 disabled:cursor-not-allowed disabled:opacity-60"
                          type="button"
                          onClick={() => handleDeletePlan(plan)}
                          disabled={deletingPlanId === plan.id}
                        >
                          {deletingPlanId === plan.id ? 'Briše se...' : 'Obriši plan'}
                        </button>
                      </div>
                    </div>
                  </article>
                ))}
              </div>
            )}
          </section>
        </div>
      </div>
    </main>
  )
}

export default AdminPage
