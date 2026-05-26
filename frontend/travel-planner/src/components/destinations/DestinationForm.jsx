import { createFormValidationHandlers } from '../../utils/formValidation'

function DestinationForm({
  form,
  plan,
  error,
  saving,
  submitLabel,
  title,
  onChange,
  onSubmit,
  onCancel,
  showCancel = false,
}) {
  const formValidation = createFormValidationHandlers()

  return (
    <section className="glass-panel rounded-[2.2rem] p-6">
      <div className="mb-6 flex items-start justify-between gap-3">
        <div>
          <p className="text-sm uppercase tracking-[0.28em] text-sky-700">Ruta</p>
          <h2 className="mt-2 text-3xl font-black tracking-tight text-slate-950">{title}</h2>
        </div>

        <div className="rounded-[1.35rem] bg-[linear-gradient(135deg,#e0f2fe_0%,#fef3c7_100%)] px-4 py-3 text-sm font-semibold text-slate-900 shadow-sm">
          Stanica putovanja
        </div>
      </div>

      <form className="space-y-4" onSubmit={onSubmit} {...formValidation}>
        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">Naziv destinacije</span>
          <input
            className="w-full rounded-[1.3rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
            type="text"
            name="name"
            value={form.name}
            onChange={onChange}
            placeholder="Budva"
            required
          />
        </label>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">Lokacija</span>
          <input
            className="w-full rounded-[1.3rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
            type="text"
            name="location"
            value={form.location}
            onChange={onChange}
            placeholder="Crna Gora"
            required
          />
        </label>

        <div className="grid gap-4 md:grid-cols-2">
          <label className="block">
            <span className="mb-2 block text-sm font-semibold text-slate-700">Datum dolaska</span>
            <input
              className="w-full rounded-[1.3rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
              type="date"
              name="arrivalDate"
              value={form.arrivalDate}
              onChange={onChange}
              min={plan.startDate.slice(0, 10)}
              max={plan.endDate.slice(0, 10)}
              required
            />
          </label>

          <label className="block">
            <span className="mb-2 block text-sm font-semibold text-slate-700">Datum odlaska</span>
            <input
              className="w-full rounded-[1.3rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
              type="date"
              name="departureDate"
              value={form.departureDate}
              onChange={onChange}
              min={form.arrivalDate || plan.startDate.slice(0, 10)}
              max={plan.endDate.slice(0, 10)}
              required
            />
          </label>
        </div>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">Opis</span>
          <textarea
            className="min-h-24 w-full rounded-[1.3rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
            name="description"
            value={form.description}
            onChange={onChange}
            placeholder="Kratak opis destinacije"
          />
        </label>

        {error ? (
          <div className="rounded-[1.3rem] border border-rose-300 bg-rose-50 px-4 py-3 text-sm text-rose-700">
            {error}
          </div>
        ) : null}

        <div className="flex gap-3">
          <button
            className="flex-1 rounded-[1.35rem] bg-[linear-gradient(90deg,#38bdf8_0%,#facc15_100%)] px-4 py-3.5 font-bold text-slate-950 shadow-[0_18px_36px_rgba(15,23,42,0.14)] transition hover:brightness-105 disabled:cursor-not-allowed disabled:opacity-60"
            type="submit"
            disabled={saving}
          >
            {saving ? 'Čuva se...' : submitLabel}
          </button>

          {showCancel ? (
            <button
              className="rounded-[1.35rem] border border-slate-300 bg-white/75 px-4 py-3 font-semibold text-slate-700 transition hover:bg-white"
              type="button"
              onClick={onCancel}
              disabled={saving}
            >
              Otkaži
            </button>
          ) : null}
        </div>
      </form>
    </section>
  )
}

export default DestinationForm
