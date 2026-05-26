import DestinationCard from '../destinations/DestinationCard'

function SharedDestinationsSection({ destinations }) {
  return (
    <section className="glass-panel rounded-[2.2rem] p-6">
      <div className="mb-5">
        <p className="text-sm uppercase tracking-[0.28em] text-slate-500">Dijeljena ruta</p>
        <h2 className="mt-2 text-3xl font-black tracking-tight text-slate-950">Destinacije</h2>
      </div>

      {destinations.length === 0 ? (
        <div className="rounded-[1.8rem] border border-dashed border-slate-300 bg-white/50 p-10 text-center text-slate-500">
          Nema dostupnih destinacija.
        </div>
      ) : (
        <div className="grid gap-4">
          {destinations.map((destination) => (
            <DestinationCard key={destination.id} destination={destination} />
          ))}
        </div>
      )}
    </section>
  )
}

export default SharedDestinationsSection
