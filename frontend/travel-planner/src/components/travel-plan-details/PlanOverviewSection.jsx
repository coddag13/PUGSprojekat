import { useEffect, useState } from 'react'

function PlanOverviewSection({
  plan,
  onSave,
  onDelete,
  saving,
  deleting,
  error,
  allowActions = true,
}) {
  const [isEditing, setIsEditing] = useState(false)
  const [form, setForm] = useState({
    title: '',
    description: '',
    startDate: '',
    endDate: '',
    plannedBudget: '',
    notes: '',
  })

  useEffect(() => {
    setForm({
      title: plan.title,
      description: plan.description,
      startDate: plan.startDate.slice(0, 10),
      endDate: plan.endDate.slice(0, 10),
      plannedBudget: String(plan.plannedBudget),
      notes: plan.notes ?? '',
    })
  }, [plan])

  const handleChange = (event) => {
    const { name, value } = event.target
    setForm((current) => ({ ...current, [name]: value }))
  }

  const handleStartEditing = () => {
    setForm({
      title: plan.title,
      description: plan.description,
      startDate: plan.startDate.slice(0, 10),
      endDate: plan.endDate.slice(0, 10),
      plannedBudget: String(plan.plannedBudget),
      notes: plan.notes ?? '',
    })
    setIsEditing(true)
  }

  const handleCancelEditing = () => {
    setIsEditing(false)
  }

  const validateForm = () => {
    if (!form.title.trim()) {
      return 'Naziv plana je obavezan.'
    }

    if (!form.description.trim()) {
      return 'Opis putovanja je obavezan.'
    }

    if (!form.startDate || !form.endDate) {
      return 'Početni i krajnji datum su obavezni.'
    }

    if (new Date(form.endDate) < new Date(form.startDate)) {
      return 'Krajnji datum ne može biti prije početnog.'
    }

    const budget = Number(form.plannedBudget)
    if (Number.isNaN(budget) || budget < 0) {
      return 'Planirani budžet mora biti pozitivan broj ili nula.'
    }

    return ''
  }

  const handleSubmit = async (event) => {
    event.preventDefault()

    const validationError = validateForm()
    if (validationError) {
      onSave(null, validationError)
      return
    }

    const saved = await onSave({
      title: form.title.trim(),
      description: form.description.trim(),
      startDate: `${form.startDate}T00:00:00`,
      endDate: `${form.endDate}T00:00:00`,
      plannedBudget: Number(form.plannedBudget),
      notes: form.notes.trim(),
    })

    if (saved) {
      setIsEditing(false)
    }
  }

  if (!isEditing) {
    return (
      <section className="glass-panel rounded-[2.3rem] p-6">
        <div className="mb-6 flex flex-wrap items-center justify-between gap-3">
          <div>
            <p className="text-sm uppercase tracking-[0.28em] text-slate-500">Pregled plana</p>
            <h2 className="mt-2 text-3xl font-black tracking-tight text-slate-950">
              Glavne informacije
            </h2>
          </div>

          {allowActions ? (
            <div className="flex flex-wrap gap-3">
              <button
                className="rounded-[1.25rem] border border-slate-300 bg-white/75 px-4 py-2.5 text-sm font-bold text-slate-800 transition hover:bg-white"
                type="button"
                onClick={handleStartEditing}
              >
                Izmijeni plan
              </button>

              <button
                className="rounded-[1.25rem] bg-rose-600 px-4 py-2.5 text-sm font-bold text-white transition hover:bg-rose-700 disabled:cursor-not-allowed disabled:opacity-60"
                type="button"
                onClick={onDelete}
                disabled={deleting}
              >
                {deleting ? 'Briše se...' : 'Obriši plan'}
              </button>
            </div>
          ) : null}
        </div>

        <div className="grid gap-4 md:grid-cols-2">
          <div className="route-card rounded-[1.8rem] p-5">
            <p className="text-sm font-semibold uppercase tracking-[0.24em] text-amber-900">
              Naziv plana
            </p>
            <p className="mt-3 text-xl font-black text-slate-950">{plan.title}</p>
          </div>

          <div className="route-card rounded-[1.8rem] p-5">
            <p className="text-sm font-semibold uppercase tracking-[0.24em] text-sky-900">
              Budžet
            </p>
            <p className="mt-3 text-xl font-black text-slate-950">{plan.plannedBudget} EUR</p>
          </div>

          <div className="route-card rounded-[1.8rem] p-5">
            <p className="text-sm font-semibold uppercase tracking-[0.24em] text-slate-700">
              Početni datum
            </p>
            <p className="mt-3 text-lg font-bold text-slate-900">{plan.startDate.slice(0, 10)}</p>
          </div>

          <div className="route-card rounded-[1.8rem] p-5">
            <p className="text-sm font-semibold uppercase tracking-[0.24em] text-slate-700">
              Krajnji datum
            </p>
            <p className="mt-3 text-lg font-bold text-slate-900">{plan.endDate.slice(0, 10)}</p>
          </div>
        </div>

        <div className="route-card mt-6 rounded-[1.8rem] p-5">
          <p className="text-sm font-semibold uppercase tracking-[0.24em] text-slate-600">Opis</p>
          <p className="mt-3 text-slate-700">{plan.description}</p>
        </div>

        <div className="route-card mt-4 rounded-[1.8rem] p-5">
          <p className="text-sm font-semibold uppercase tracking-[0.24em] text-slate-600">
            Napomene
          </p>
          <p className="mt-3 text-slate-700">{plan.notes || 'Nema dodatnih napomena.'}</p>
        </div>

        {error ? (
          <div className="mt-4 rounded-[1.3rem] border border-rose-300 bg-rose-50 px-4 py-3 text-sm text-rose-700">
            {error}
          </div>
        ) : null}
      </section>
    )
  }

  return (
    <section className="glass-panel rounded-[2.3rem] p-6">
      <div className="mb-6 flex flex-wrap items-center justify-between gap-3">
        <div>
          <p className="text-sm uppercase tracking-[0.28em] text-slate-500">Izmjena plana</p>
          <h2 className="mt-2 text-3xl font-black tracking-tight text-slate-950">
            Ažuriraj informacije
          </h2>
        </div>

        <div className="flex flex-wrap gap-3">
          <button
            className="rounded-[1.25rem] border border-slate-300 bg-white/75 px-4 py-2.5 text-sm font-bold text-slate-800 transition hover:bg-white"
            type="button"
            onClick={handleCancelEditing}
            disabled={saving}
          >
            Otkaži
          </button>

          <button
            className="rounded-[1.25rem] bg-rose-600 px-4 py-2.5 text-sm font-bold text-white transition hover:bg-rose-700 disabled:cursor-not-allowed disabled:opacity-60"
            type="button"
            onClick={onDelete}
            disabled={deleting}
          >
            {deleting ? 'Briše se...' : 'Obriši plan'}
          </button>
        </div>
      </div>

      <form className="space-y-5" onSubmit={handleSubmit}>
        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">Naziv plana</span>
          <input
            className="w-full rounded-[1.3rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
            type="text"
            name="title"
            value={form.title}
            onChange={handleChange}
            required
          />
        </label>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">Opis putovanja</span>
          <textarea
            className="min-h-28 w-full rounded-[1.3rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
            name="description"
            value={form.description}
            onChange={handleChange}
            required
          />
        </label>

        <div className="grid gap-4 md:grid-cols-2">
          <label className="block">
            <span className="mb-2 block text-sm font-semibold text-slate-700">Početni datum</span>
            <input
              className="w-full rounded-[1.3rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
              type="date"
              name="startDate"
              value={form.startDate}
              onChange={handleChange}
              required
            />
          </label>

          <label className="block">
            <span className="mb-2 block text-sm font-semibold text-slate-700">Krajnji datum</span>
            <input
              className="w-full rounded-[1.3rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
              type="date"
              name="endDate"
              value={form.endDate}
              onChange={handleChange}
              min={form.startDate || undefined}
              required
            />
          </label>
        </div>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">Planirani budžet</span>
          <input
            className="w-full rounded-[1.3rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
            type="number"
            name="plannedBudget"
            step="0.01"
            min="0"
            value={form.plannedBudget}
            onChange={handleChange}
            required
          />
        </label>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">Napomene</span>
          <textarea
            className="min-h-24 w-full rounded-[1.3rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
            name="notes"
            value={form.notes}
            onChange={handleChange}
          />
        </label>

        {error ? (
          <div className="rounded-[1.3rem] border border-rose-300 bg-rose-50 px-4 py-3 text-sm text-rose-700">
            {error}
          </div>
        ) : null}

        <button
          className="w-full rounded-[1.35rem] bg-[linear-gradient(90deg,#facc15_0%,#f59e0b_45%,#0f172a_100%)] px-4 py-3.5 font-bold text-white shadow-[0_20px_40px_rgba(15,23,42,0.16)] transition hover:brightness-105 disabled:cursor-not-allowed disabled:opacity-60"
          type="submit"
          disabled={saving}
        >
          {saving ? 'Čuva se...' : 'Sačuvaj izmjene'}
        </button>
      </form>
    </section>
  )
}

export default PlanOverviewSection
