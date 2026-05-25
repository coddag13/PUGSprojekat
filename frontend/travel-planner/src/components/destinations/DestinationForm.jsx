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
  return (
    <section className="rounded-[2rem] bg-white p-6 shadow-lg">
      <h2 className="text-2xl font-bold text-slate-900">{title}</h2>

      <form className="mt-6 space-y-4" onSubmit={onSubmit}>
        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">
            Naziv destinacije
          </span>
          <input
            className="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none transition focus:border-amber-500"
            type="text"
            name="name"
            value={form.name}
            onChange={onChange}
            placeholder="Budva"
            required
          />
        </label>

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
            placeholder="Crna Gora"
            required
          />
        </label>

        <div className="grid gap-4 md:grid-cols-2">
          <label className="block">
            <span className="mb-2 block text-sm font-semibold text-slate-700">
              Datum dolaska
            </span>
            <input
              className="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none transition focus:border-amber-500"
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
            <span className="mb-2 block text-sm font-semibold text-slate-700">
              Datum odlaska
            </span>
            <input
              className="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none transition focus:border-amber-500"
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
          <span className="mb-2 block text-sm font-semibold text-slate-700">
            Opis
          </span>
          <textarea
            className="min-h-24 w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none transition focus:border-amber-500"
            name="description"
            value={form.description}
            onChange={onChange}
            placeholder="Kratak opis destinacije"
          />
        </label>

        {error ? (
          <div className="rounded-2xl border border-rose-300 bg-rose-50 px-4 py-3 text-sm text-rose-700">
            {error}
          </div>
        ) : null}

        <div className="flex gap-3">
          <button
            className="flex-1 rounded-2xl bg-amber-300 px-4 py-3 font-bold text-slate-900 transition hover:bg-amber-200 disabled:cursor-not-allowed disabled:opacity-60"
            type="submit"
            disabled={saving}
          >
            {saving ? 'Čuva se...' : submitLabel}
          </button>

          {showCancel ? (
            <button
              className="rounded-2xl border border-slate-300 px-4 py-3 font-semibold text-slate-700 transition hover:bg-slate-50"
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