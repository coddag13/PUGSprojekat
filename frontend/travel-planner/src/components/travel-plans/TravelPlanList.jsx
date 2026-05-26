import TravelPlanCard from './TravelPlanCard'

function TravelPlanList({ plans, loading, onRefresh }) {
  return (
    <section className="glass-panel rounded-[2.2rem] p-6">
      <div className="mb-6 flex items-start justify-between gap-4">
        <div>
          <p className="text-sm uppercase tracking-[0.28em] text-sky-700">Pregled rute</p>
          <h2 className="mt-2 text-3xl font-black tracking-tight text-slate-950">Moji planovi</h2>
          <p className="mt-3 text-sm leading-6 text-slate-600">
            Svako putovanje ima svoj prostor za budžet, raspored aktivnosti i detalje puta.
          </p>
        </div>

        <button
          className="rounded-[1.3rem] border border-slate-300 bg-white/70 px-4 py-2.5 text-sm font-semibold text-slate-700 transition hover:bg-white"
          type="button"
          onClick={onRefresh}
        >
          Osvježi
        </button>
      </div>

      {loading ? (
        <div className="rounded-[1.8rem] border border-dashed border-slate-300 bg-white/50 p-10 text-center text-slate-500">
          Učitavanje planova...
        </div>
      ) : plans.length === 0 ? (
        <div className="rounded-[1.8rem] border border-dashed border-slate-300 bg-white/50 p-10 text-center text-slate-500">
          Još nema kreiranih planova.
        </div>
      ) : (
        <div className="grid gap-4">
          {plans.map((plan) => (
            <TravelPlanCard key={plan.id} plan={plan} />
          ))}
        </div>
      )}
    </section>
  )
}

export default TravelPlanList
