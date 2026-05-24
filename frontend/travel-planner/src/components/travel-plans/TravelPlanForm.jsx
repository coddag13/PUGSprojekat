function TravelPlanForm({
  form,
  error,
  saving,
  onChange,
  onSubmit,
}) {
  return (
    <section className="rounded-[2rem] bg-white p-6 shadow-lg">
      <h2 className="text-2xl font-bold text-slate-900">Novi plan putovanja</h2>

      <form className="mt-6 space-y-4" onSubmit={onSubmit}>
        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">
            Naziv plana
          </span>
          <input
            className="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none transition focus:border-amber-500"
            type="text"
            name="title"
            value={form.title}
            onChange={onChange}
            placeholder="Ljetovanje 2026"
            required
          />
        </label>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">
            Opis putovanja
          </span>
          <textarea
            className="min-h-28 w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none transition focus:border-amber-500"
            name="description"
            value={form.description}
            onChange={onChange}
            placeholder="Kratak opis putovanja"
            required
          />
        </label>

        <div className="grid gap-4 md:grid-cols-2">
          <label className="block">
            <span className="mb-2 block text-sm font-semibold text-slate-700">
              Početni datum
            </span>
            <input
              className="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none transition focus:border-amber-500"
              type="date"
              name="startDate"
              value={form.startDate}
              onChange={onChange}
              required
            />
          </label>

          <label className="block">
            <span className="mb-2 block text-sm font-semibold text-slate-700">
              Krajnji datum
            </span>
            <input
              className="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none transition focus:border-amber-500"
              type="date"
              name="endDate"
              value={form.endDate}
              onChange={onChange}
              min={form.startDate || undefined}
              required
            />
          </label>
        </div>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">
            Planirani budžet
          </span>
          <input
            className="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none transition focus:border-amber-500"
            type="number"
            step="0.01"
            min="0"
            name="plannedBudget"
            value={form.plannedBudget}
            onChange={onChange}
            placeholder="0.00"
            required
          />
        </label>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">
            Napomene
          </span>
          <textarea
            className="min-h-24 w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none transition focus:border-amber-500"
            name="notes"
            value={form.notes}
            onChange={onChange}
            placeholder="Dodatne napomene"
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
          {saving ? 'Čuva se...' : 'Kreiraj plan'}
        </button>
      </form>
    </section>
  )
}

export default TravelPlanForm