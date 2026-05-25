import { Link } from 'react-router-dom'

function PlanDetailsHeader({ plan }) {
  return (
    <section className="relative overflow-hidden rounded-[2.25rem] bg-slate-950 px-6 py-7 text-white shadow-[0_30px_80px_rgba(15,23,42,0.28)]">
      <div className="pointer-events-none absolute right-[-3rem] top-[-3rem] h-40 w-40 rounded-full bg-amber-300/20 blur-3xl" />
      <div className="pointer-events-none absolute bottom-[-4rem] left-[-2rem] h-44 w-44 rounded-full bg-sky-300/15 blur-3xl" />

      <Link
        to="/travel-plans"
        className="relative inline-flex rounded-full border border-white/20 px-4 py-2 text-sm font-semibold text-white transition hover:bg-white/10"
      >
        Nazad na planove
      </Link>

      <div className="relative mt-6 flex flex-col gap-5 lg:flex-row lg:items-start lg:justify-between">
        <div className="max-w-3xl">
          <p className="text-sm uppercase tracking-[0.28em] text-amber-300">Detalji plana</p>
          <h1 className="mt-3 text-4xl font-black tracking-tight md:text-5xl">{plan.title}</h1>
          <p className="mt-4 max-w-2xl text-slate-300">{plan.description}</p>
        </div>

        <div className="rounded-[1.9rem] bg-white/10 px-6 py-5 text-right backdrop-blur-sm">
          <p className="text-xs uppercase tracking-[0.3em] text-slate-300">Budžet</p>
          <p className="mt-3 text-3xl font-black text-amber-300">{plan.plannedBudget} EUR</p>
        </div>
      </div>

      <div className="relative mt-7 flex flex-wrap gap-3 text-sm">
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

export default PlanDetailsHeader
