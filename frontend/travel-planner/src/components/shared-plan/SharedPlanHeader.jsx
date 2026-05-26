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
    <section className="dark-signal-panel rounded-[2.5rem] px-6 py-8 text-white shadow-xl">
      <div className="flex flex-wrap items-start justify-between gap-5">
        <div className="max-w-3xl">
          <div className="flex flex-wrap items-center gap-3">
            <span className="signal-chip inline-flex rounded-full px-4 py-1.5 text-xs font-semibold uppercase tracking-[0.32em] text-amber-200">
              Dijeljeni plan
            </span>
            <span className={`rounded-full px-4 py-1.5 text-xs font-bold uppercase tracking-[0.28em] ${accessClass}`}>
              {ACCESS_LABELS[accessType] ?? 'Nepoznato'}
            </span>
          </div>

          <h1 className="travel-heading mt-6 text-4xl font-black md:text-6xl">{plan.title}</h1>
          <p className="mt-5 max-w-2xl text-lg leading-8 text-slate-300">{plan.description}</p>
        </div>

        <div className="grid gap-4 md:grid-cols-3 xl:grid-cols-1 xl:w-[30rem]">
          <div className="route-kpi rounded-[1.8rem] bg-white/6 px-6 py-5 backdrop-blur-sm">
            <p className="text-xs uppercase tracking-[0.3em] text-slate-300">Budžet</p>
            <p className="mt-3 text-3xl font-black text-amber-300">{plan.plannedBudget} EUR</p>
          </div>

          <div className="route-kpi rounded-[1.8rem] bg-white/6 px-6 py-5 backdrop-blur-sm">
            <p className="text-xs uppercase tracking-[0.3em] text-slate-300">Početak</p>
            <p className="mt-3 text-lg font-bold text-white">{plan.startDate.slice(0, 10)}</p>
          </div>

          <div className="route-kpi rounded-[1.8rem] bg-white/6 px-6 py-5 backdrop-blur-sm">
            <p className="text-xs uppercase tracking-[0.3em] text-slate-300">Kraj</p>
            <p className="mt-3 text-lg font-bold text-white">{plan.endDate.slice(0, 10)}</p>
          </div>
        </div>
      </div>
    </section>
  )
}

export default SharedPlanHeader
