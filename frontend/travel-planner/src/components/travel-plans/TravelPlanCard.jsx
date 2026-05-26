import { useNavigate } from 'react-router-dom'

function TravelPlanCard({ plan }) {
  const navigate = useNavigate()

  const handleOpen = () => {
    navigate(`/travel-plans/${plan.id}`)
  }

  return (
    <article className="route-card rounded-[2rem] p-5">
      <div className="flex flex-wrap items-start justify-between gap-4">
        <div className="max-w-2xl">
          <div className="flex flex-wrap items-center gap-2">
            <span className="rounded-full bg-slate-950 px-3 py-1 text-xs font-semibold uppercase tracking-[0.3em] text-amber-300">
              Aktivni plan
            </span>
            <span className="rounded-full bg-white/75 px-3 py-1 text-xs font-semibold text-slate-600">
              {plan.startDate.slice(0, 10)} - {plan.endDate.slice(0, 10)}
            </span>
          </div>

          <h3 className="mt-4 text-2xl font-black tracking-tight text-slate-950">{plan.title}</h3>
          <p className="mt-3 text-slate-700">{plan.description}</p>
        </div>

        <div className="rounded-[1.5rem] bg-slate-950 px-5 py-4 text-right text-white shadow-lg">
          <p className="text-xs uppercase tracking-[0.32em] text-slate-300">Budžet</p>
          <p className="mt-2 text-2xl font-black text-amber-300">{plan.plannedBudget} EUR</p>
        </div>
      </div>

      <div className="mt-5 grid gap-3 md:grid-cols-[1fr_auto] md:items-end">
        {plan.notes ? (
          <p className="rounded-[1.5rem] border border-white/50 bg-white/70 px-4 py-3 text-sm leading-6 text-slate-600">
            {plan.notes}
          </p>
        ) : (
          <p className="rounded-[1.5rem] border border-dashed border-slate-300 bg-white/50 px-4 py-3 text-sm text-slate-500">
            Nema dodatnih napomena za ovaj plan.
          </p>
        )}

        <button
          className="rounded-[1.35rem] bg-[linear-gradient(90deg,#facc15_0%,#f59e0b_55%,#0f172a_100%)] px-5 py-3 text-sm font-bold text-white shadow-[0_18px_36px_rgba(15,23,42,0.14)] transition hover:brightness-105"
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
