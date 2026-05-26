import { Link } from 'react-router-dom'

function PlanDetailsHeader({ plan }) {
  return (
    <section className="dark-signal-panel relative overflow-hidden rounded-[2.5rem] px-6 py-8 text-white shadow-[0_30px_80px_rgba(15,23,42,0.28)]">
      <div className="pointer-events-none absolute left-[10%] top-[28%] h-px w-40 bg-gradient-to-r from-transparent via-amber-300/45 to-transparent" />
      <div className="pointer-events-none absolute right-[12%] top-[22%] h-px w-28 bg-gradient-to-r from-transparent via-sky-300/45 to-transparent" />

      <Link
        to="/travel-plans"
        className="relative inline-flex rounded-full border border-white/20 bg-white/5 px-4 py-2 text-sm font-semibold text-white transition hover:bg-white/10"
      >
        Nazad na planove
      </Link>

      <div className="relative mt-7 flex flex-col gap-6 xl:flex-row xl:items-start xl:justify-between">
        <div className="max-w-3xl">
          <div className="flex flex-wrap items-center gap-3">
            <span className="signal-chip inline-flex rounded-full px-4 py-1.5 text-xs font-semibold uppercase tracking-[0.32em] text-amber-200">
              Detalji plana
            </span>
            <span className="rounded-full border border-white/10 bg-white/5 px-4 py-1.5 text-xs uppercase tracking-[0.32em] text-slate-300">
              Aktivna ruta
            </span>
          </div>

          <h1 className="travel-heading mt-6 text-4xl font-black md:text-6xl">{plan.title}</h1>
          <p className="mt-5 max-w-2xl text-lg leading-8 text-slate-300">{plan.description}</p>
        </div>

        <div className="grid gap-4 md:grid-cols-3 xl:w-[30rem] xl:grid-cols-1">
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

export default PlanDetailsHeader
