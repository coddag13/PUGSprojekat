function TravelPlanForm({ form, error, saving, onChange, onSubmit }) {
  return (
    <section className="rounded-[2rem] border border-white/70 bg-white/85 p-6 shadow-[0_20px_60px_rgba(15,23,42,0.08)] backdrop-blur-sm">
      <div className="mb-6 flex items-center justify-between gap-3">
        <div>
          <p className="text-sm uppercase tracking-[0.25em] text-amber-700">Novi plan</p>
          <h2 className="mt-2 text-2xl font-black text-slate-900">Kreiraj novo putovanje</h2>
        </div>
        <div className="rounded-2xl bg-amber-100 px-4 py-2 text-sm font-semibold text-amber-900">
          Brzo dodavanje
        </div>
      </div>

      <form className="space-y-4" onSubmit={onSubmit}>
        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">Naziv plana</span>
          <input
            className="w-full rounded-[1.3rem] border border-slate-300 bg-white px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
            type="text"
            name="title"
            value={form.title}
            onChange={onChange}
            placeholder="Ljetovanje 2026"
            required
          />
        </label>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">Opis putovanja</span>
          <textarea
            className="min-h-28 w-full rounded-[1.3rem] border border-slate-300 bg-white px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
            name="description"
            value={form.description}
            onChange={onChange}
            placeholder="Kratak opis putovanja"
            required
          />
        </label>

        <div className="grid gap-4 md:grid-cols-2">
          <label className="block">
            <span className="mb-2 block text-sm font-semibold text-slate-700">Početni datum</span>
            <input
              className="w-full rounded-[1.3rem] border border-slate-300 bg-white px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
              type="date"
              name="startDate"
              value={form.startDate}
              onChange={onChange}
              required
            />
          </label>

          <label className="block">
            <span className="mb-2 block text-sm font-semibold text-slate-700">Krajnji datum</span>
            <input
              className="w-full rounded-[1.3rem] border border-slate-300 bg-white px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
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
          <span className="mb-2 block text-sm font-semibold text-slate-700">Planirani budžet</span>
          <input
            className="w-full rounded-[1.3rem] border border-slate-300 bg-white px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
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
          <span className="mb-2 block text-sm font-semibold text-slate-700">Napomene</span>
          <textarea
            className="min-h-24 w-full rounded-[1.3rem] border border-slate-300 bg-white px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
            name="notes"
            value={form.notes}
            onChange={onChange}
            placeholder="Dodatne napomene"
          />
        </label>

        {error ? (
          <div className="rounded-[1.3rem] border border-rose-300 bg-rose-50 px-4 py-3 text-sm text-rose-700">
            {error}
          </div>
        ) : null}

        <button
          className="w-full rounded-[1.3rem] bg-amber-300 px-4 py-3.5 font-bold text-slate-900 shadow-lg transition hover:bg-amber-200 disabled:cursor-not-allowed disabled:opacity-60"
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
