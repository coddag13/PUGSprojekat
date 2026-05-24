import { Link } from 'react-router-dom'

function PlanDetailsHeader({ plan }) {
  return (
    <section className="rounded-[2rem] bg-slate-950 px-6 py-6 text-white shadow-xl">
      <Link
        to="/travel-plans"
        className="inline-flex rounded-full border border-white/20 px-4 py-2 text-sm font-semibold text-white transition hover:bg-white/10"
      >
        Nazad na planove
      </Link>

      <div className="mt-6 flex flex-col gap-5 lg:flex-row lg:items-start lg:justify-between">
        <div className="max-w-3xl">
          <p className="text-sm uppercase tracking-[0.25em] text-amber-300">Detalji plana</p>
          <h1 className="mt-2 text-4xl font-black tracking-tight">{plan.title}</h1>
          <p className="mt-3 text-slate-300">{plan.description}</p>
        </div>

        <div className="rounded-3xl bg-white/10 px-5 py-4 text-right">
          <p className="text-sm uppercase tracking-[0.2em] text-slate-300">Budžet</p>
          <p className="mt-2 text-3xl font-bold text-amber-300">{plan.plannedBudget} EUR</p>
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

export default PlanDetailsHeader