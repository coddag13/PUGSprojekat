import TravelPlanCard from './TravelPlanCard'

function TravelPlanList({ plans, loading, onRefresh }) {
  return (
    <section className="rounded-[2rem] border border-white/70 bg-white/85 p-6 shadow-[0_20px_60px_rgba(15,23,42,0.08)] backdrop-blur-sm">
      <div className="mb-5 flex items-center justify-between">
        <div>
          <p className="text-sm uppercase tracking-[0.25em] text-sky-700">Pregled</p>
          <h2 className="mt-2 text-2xl font-black text-slate-900">Moji planovi</h2>
        </div>

        <button
          className="rounded-2xl border border-slate-300 px-4 py-2 text-sm font-semibold text-slate-700 transition hover:bg-slate-50"
          type="button"
          onClick={onRefresh}
        >
          Osvježi
        </button>
      </div>

      {loading ? (
        <div className="rounded-3xl border border-dashed border-slate-300 p-10 text-center text-slate-500">
          Učitavanje planova...
        </div>
      ) : plans.length === 0 ? (
        <div className="rounded-3xl border border-dashed border-slate-300 p-10 text-center text-slate-500">
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
