import { createFormValidationHandlers } from '../../utils/formValidation'

function ActivityForm({
  form,
  plan,
  destinations,
  selectedDestination,
  error,
  saving,
  title,
  submitLabel,
  onChange,
  onSubmit,
  onCancel,
  showCancel = false,
}) {
  const formValidation = createFormValidationHandlers()
  const minDate = selectedDestination
    ? selectedDestination.arrivalDate.slice(0, 10)
    : plan.startDate.slice(0, 10)

  const maxDate = selectedDestination
    ? selectedDestination.departureDate.slice(0, 10)
    : plan.endDate.slice(0, 10)

  return (
    <section className="glass-panel rounded-[2.2rem] p-6">
      <div className="mb-6 flex items-start justify-between gap-3">
        <div>
          <p className="text-sm uppercase tracking-[0.28em] text-amber-700">Raspored</p>
          <h2 className="mt-2 text-3xl font-black tracking-tight text-slate-950">{title}</h2>
        </div>

        <div className="rounded-[1.35rem] bg-[linear-gradient(135deg,#fef3c7_0%,#e0f2fe_100%)] px-4 py-3 text-sm font-semibold text-slate-900 shadow-sm">
          Dnevni tok puta
        </div>
      </div>

      <form className="space-y-4" onSubmit={onSubmit} {...formValidation}>
        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">Naziv aktivnosti</span>
          <input
            className="w-full rounded-[1.3rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
            type="text"
            name="name"
            value={form.name}
            onChange={onChange}
            placeholder="Skijanje"
            required
          />
        </label>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">Destinacija</span>
          <select
            className="w-full rounded-[1.3rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
            name="destinationId"
            value={form.destinationId}
            onChange={onChange}
          >
            <option value="">Bez vezane destinacije</option>
            {destinations.map((destination) => (
              <option key={destination.id} value={destination.id}>
                {destination.name}
              </option>
            ))}
          </select>
        </label>

        <div className="grid gap-4 md:grid-cols-2">
          <label className="block">
            <span className="mb-2 block text-sm font-semibold text-slate-700">Datum</span>
            <input
              className="w-full rounded-[1.3rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
              type="date"
              name="date"
              value={form.date}
              onChange={onChange}
              min={minDate}
              max={maxDate}
              required
            />
          </label>

          <label className="block">
            <span className="mb-2 block text-sm font-semibold text-slate-700">Vrijeme</span>
            <input
              className="w-full rounded-[1.3rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
              type="time"
              name="time"
              value={form.time}
              onChange={onChange}
              required
            />
          </label>
        </div>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">Lokacija</span>
          <input
            className="w-full rounded-[1.3rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
            type="text"
            name="location"
            value={form.location}
            onChange={onChange}
            placeholder="Staza"
            required
          />
        </label>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">Procijenjeni trošak</span>
          <input
            className="w-full rounded-[1.3rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
            type="number"
            name="estimatedCost"
            step="0.01"
            min="0"
            value={form.estimatedCost}
            onChange={onChange}
            placeholder="0.00"
            required
          />
        </label>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">Status</span>
          <select
            className="w-full rounded-[1.3rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
            name="status"
            value={form.status}
            onChange={onChange}
          >
            <option value="0">Planirano</option>
            <option value="1">Rezervisano</option>
            <option value="2">Završeno</option>
            <option value="3">Otkazano</option>
          </select>
        </label>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">Opis</span>
          <textarea
            className="min-h-24 w-full rounded-[1.3rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
            name="description"
            value={form.description}
            onChange={onChange}
            placeholder="Kratak opis aktivnosti"
          />
        </label>

        {error ? (
          <div className="rounded-[1.3rem] border border-rose-300 bg-rose-50 px-4 py-3 text-sm text-rose-700">
            {error}
          </div>
        ) : null}

        <div className="flex gap-3">
          <button
            className="flex-1 rounded-[1.35rem] bg-[linear-gradient(90deg,#facc15_0%,#f59e0b_45%,#0f172a_100%)] px-4 py-3.5 font-bold text-white shadow-[0_20px_40px_rgba(15,23,42,0.16)] transition hover:brightness-105 disabled:cursor-not-allowed disabled:opacity-60"
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

export default ActivityForm
