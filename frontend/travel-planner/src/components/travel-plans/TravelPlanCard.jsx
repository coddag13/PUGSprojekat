import { useNavigate } from 'react-router-dom'

function TravelPlanCard({ plan }) {
  const navigate = useNavigate()

  const handleOpen = () => {
    navigate(`/travel-plans/${plan.id}`)
  }

  return (
    <article className="rounded-3xl border border-slate-200 bg-[linear-gradient(135deg,#fffaf0_0%,#f8fbff_100%)] p-5 shadow-sm">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <h3 className="text-xl font-bold text-slate-900">{plan.title}</h3>
          <p className="mt-2 text-slate-700">{plan.description}</p>
        </div>
        <div className="rounded-2xl bg-slate-900 px-4 py-2 text-sm font-semibold text-white">
          {plan.plannedBudget} EUR
        </div>
      </div>

      <div className="mt-4 flex flex-wrap gap-3 text-sm text-slate-600">
        <span className="rounded-full bg-amber-100 px-3 py-1">{plan.startDate.slice(0, 10)}</span>
        <span className="rounded-full bg-sky-100 px-3 py-1">{plan.endDate.slice(0, 10)}</span>
      </div>

      {plan.notes ? (
        <p className="mt-4 rounded-2xl bg-white/80 px-4 py-3 text-sm text-slate-600">
          {plan.notes}
        </p>
      ) : null}

      <div className="mt-5 flex justify-end">
        <button
          className="rounded-2xl bg-amber-300 px-4 py-2 text-sm font-bold text-slate-900 transition hover:bg-amber-200"
          type="button"
          onClick={handleOpen}
        >
          Otvori detalje
        </button>
      </div>
    </article>
  )
}

export default TravelPlanCard