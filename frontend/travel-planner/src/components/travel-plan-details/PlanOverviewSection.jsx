function PlanOverviewSection({ plan }) {
  return (
    <section className="rounded-[2rem] bg-white p-6 shadow-lg">
      <h2 className="text-2xl font-bold text-slate-900">Pregled plana</h2>

      <div className="mt-6 grid gap-4 md:grid-cols-2">
        <div className="rounded-3xl bg-amber-50 p-5">
          <p className="text-sm font-semibold uppercase tracking-[0.2em] text-amber-900">
            Naziv plana
          </p>
          <p className="mt-3 text-xl font-bold text-slate-900">{plan.title}</p>
        </div>

        <div className="rounded-3xl bg-sky-50 p-5">
          <p className="text-sm font-semibold uppercase tracking-[0.2em] text-sky-900">
            Budžet
          </p>
          <p className="mt-3 text-xl font-bold text-slate-900">{plan.plannedBudget} EUR</p>
        </div>

        <div className="rounded-3xl bg-slate-50 p-5">
          <p className="text-sm font-semibold uppercase tracking-[0.2em] text-slate-700">
            Početni datum
          </p>
          <p className="mt-3 text-lg font-bold text-slate-900">{plan.startDate.slice(0, 10)}</p>
        </div>

        <div className="rounded-3xl bg-slate-50 p-5">
          <p className="text-sm font-semibold uppercase tracking-[0.2em] text-slate-700">
            Krajnji datum
          </p>
          <p className="mt-3 text-lg font-bold text-slate-900">{plan.endDate.slice(0, 10)}</p>
        </div>
      </div>

      <div className="mt-6 rounded-3xl border border-slate-200 p-5">
        <p className="text-sm font-semibold uppercase tracking-[0.2em] text-slate-600">
          Opis
        </p>
        <p className="mt-3 text-slate-700">{plan.description}</p>
      </div>

      <div className="mt-4 rounded-3xl border border-slate-200 p-5">
        <p className="text-sm font-semibold uppercase tracking-[0.2em] text-slate-600">
          Napomene
        </p>
        <p className="mt-3 text-slate-700">{plan.notes || 'Nema dodatnih napomena.'}</p>
      </div>
    </section>
  )
}

export default PlanOverviewSection