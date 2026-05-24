function ActivityForm({
  form,
  plan,
  destinations,
  selectedDestination,
  error,
  saving,
  onChange,
  onSubmit,
}) {
  const minDate = selectedDestination
    ? selectedDestination.arrivalDate.slice(0, 10)
    : plan.startDate.slice(0, 10)

  const maxDate = selectedDestination
    ? selectedDestination.departureDate.slice(0, 10)
    : plan.endDate.slice(0, 10)

  return (
    <section className="rounded-[2rem] bg-white p-6 shadow-lg">
      <h2 className="text-2xl font-bold text-slate-900">Nova aktivnost</h2>

      <form className="mt-6 space-y-4" onSubmit={onSubmit}>
        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">
            Naziv aktivnosti
          </span>
          <input
            className="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none transition focus:border-amber-500"
            type="text"
            name="name"
            value={form.name}
            onChange={onChange}
            placeholder="Skijanje"
            required
          />
        </label>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">
            Destinacija
          </span>
          <select
            className="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none transition focus:border-amber-500"
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
            <span className="mb-2 block text-sm font-semibold text-slate-700">
              Datum
            </span>
            <input
              className="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none transition focus:border-amber-500"
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
            <span className="mb-2 block text-sm font-semibold text-slate-700">
              Vrijeme
            </span>
            <input
              className="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none transition focus:border-amber-500"
              type="time"
              name="time"
              value={form.time}
              onChange={onChange}
              required
            />
          </label>
        </div>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">
            Lokacija
          </span>
          <input
            className="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none transition focus:border-amber-500"
            type="text"
            name="location"
            value={form.location}
            onChange={onChange}
            placeholder="Staza"
            required
          />
        </label>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">
            Procijenjeni trošak
          </span>
          <input
            className="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none transition focus:border-amber-500"
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
        <span className="mb-2 block text-sm font-semibold text-slate-700">
            Status
        </span>
        <select
            className="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none transition focus:border-amber-500"
            name="status"
            value={form.status}
            onChange={onChange}
        >
            <option value="0">Planned</option>
            <option value="1">Reserved</option>
            <option value="2">Completed</option>
            <option value="3">Cancelled</option>
        </select>
        </label>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">
            Opis
          </span>
          <textarea
            className="min-h-24 w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none transition focus:border-amber-500"
            name="description"
            value={form.description}
            onChange={onChange}
            placeholder="Kratak opis aktivnosti"
          />
        </label>

        {error ? (
          <div className="rounded-2xl border border-rose-300 bg-rose-50 px-4 py-3 text-sm text-rose-700">
            {error}
          </div>
        ) : null}

        <button
          className="w-full rounded-2xl bg-amber-300 px-4 py-3 font-bold text-slate-900 transition hover:bg-amber-200 disabled:cursor-not-allowed disabled:opacity-60"
          type="submit"
          disabled={saving}
        >
          {saving ? 'Čuva se...' : 'Dodaj aktivnost'}
        </button>
      </form>
    </section>
  )
}

export default ActivityForm