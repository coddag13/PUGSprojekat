function DestinationCard({ destination }) {
  return (
    <article className="rounded-3xl border border-slate-200 bg-[linear-gradient(135deg,#fffaf0_0%,#f8fbff_100%)] p-5 shadow-sm">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <h3 className="text-xl font-bold text-slate-900">{destination.name}</h3>
          <p className="mt-2 text-slate-700">{destination.location}</p>
        </div>
      </div>

      <div className="mt-4 flex flex-wrap gap-3 text-sm text-slate-600">
        <span className="rounded-full bg-amber-100 px-3 py-1">
          Dolazak: {destination.arrivalDate.slice(0, 10)}
        </span>
        <span className="rounded-full bg-sky-100 px-3 py-1">
          Odlazak: {destination.departureDate.slice(0, 10)}
        </span>
      </div>

      {destination.description ? (
        <p className="mt-4 rounded-2xl bg-white/80 px-4 py-3 text-sm text-slate-600">
          {destination.description}
        </p>
      ) : null}
    </article>
  )
}

export default DestinationCard