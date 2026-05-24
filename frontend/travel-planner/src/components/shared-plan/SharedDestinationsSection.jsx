import DestinationCard from '../destinations/DestinationCard'

function SharedDestinationsSection({ destinations }) {
  return (
    <section className="rounded-[2rem] bg-white p-6 shadow-lg">
      <h2 className="mb-5 text-2xl font-bold text-slate-900">Destinacije</h2>

      {destinations.length === 0 ? (
        <div className="rounded-3xl border border-dashed border-slate-300 p-10 text-center text-slate-500">
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
