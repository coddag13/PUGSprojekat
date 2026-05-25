const ACCESS_LABELS = {
  0: 'Pregled',
  1: 'Uređivanje',
}

const ACCESS_CLASSES = {
  0: 'bg-sky-100 text-sky-900',
  1: 'bg-amber-100 text-amber-900',
}

function SharedPlanHeader({ plan, accessType }) {
  const accessClass = ACCESS_CLASSES[accessType] ?? 'bg-slate-100 text-slate-900'

  return (
    <section className="rounded-[2rem] bg-slate-950 px-6 py-6 text-white shadow-xl">
      <div className="flex flex-wrap items-start justify-between gap-4">
        <div className="max-w-3xl">
          <p className="text-sm uppercase tracking-[0.25em] text-amber-300">Dijeljeni plan</p>
          <h1 className="mt-2 text-4xl font-black tracking-tight">{plan.title}</h1>
          <p className="mt-3 text-slate-300">{plan.description}</p>
        </div>

        <div className="flex flex-col items-end gap-3">
          <span className={`rounded-full px-4 py-2 text-sm font-bold ${accessClass}`}>
            {ACCESS_LABELS[accessType] ?? 'Nepoznato'}
          </span>
          <div className="rounded-3xl bg-white/10 px-5 py-4 text-right">
            <p className="text-sm uppercase tracking-[0.2em] text-slate-300">Budžet</p>
            <p className="mt-2 text-3xl font-bold text-amber-300">{plan.plannedBudget} EUR</p>
          </div>
        </div>
      </div>

      <div className="mt-6 flex flex-wrap gap-3 text-sm">
        <span className="rounded-full bg-amber-100 px-4 py-2 font-semibold text-slate-900">
          Početak: {plan.startDate.slice(0, 10)}
        </span>
        <span className="rounded-full bg-sky-100 px-4 py-2 font-semibold text-slate-900">
          Kraj: {plan.endDate.slice(0, 10)}
        </span>
      </div>
    </section>
  )
}

export default SharedPlanHeader
