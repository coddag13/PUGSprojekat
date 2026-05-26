function DestinationCard({ destination, onEdit, onDelete, isDeleting }) {
  return (
    <article className="route-card rounded-[2rem] p-5">
      <div className="flex flex-wrap items-start justify-between gap-4">
        <div className="max-w-2xl">
          <div className="flex flex-wrap items-center gap-2">
            <span className="rounded-full bg-slate-950 px-3 py-1 text-xs font-semibold uppercase tracking-[0.3em] text-sky-200">
              Destinacija
            </span>
            <span className="rounded-full bg-white/75 px-3 py-1 text-xs font-semibold text-slate-600">
              {destination.location}
            </span>
          </div>

          <h3 className="mt-4 text-2xl font-black tracking-tight text-slate-950">
            {destination.name}
          </h3>
        </div>

        {onEdit || onDelete ? (
          <div className="flex gap-3">
            {onEdit ? (
              <button
                className="rounded-[1.25rem] border border-slate-300 bg-white/75 px-4 py-2.5 text-sm font-semibold text-slate-700 transition hover:bg-white"
                type="button"
                onClick={() => onEdit(destination)}
              >
                Izmijeni
              </button>
            ) : null}

            {onDelete ? (
              <button
                className="rounded-[1.25rem] bg-rose-600 px-4 py-2.5 text-sm font-bold text-white transition hover:bg-rose-700 disabled:cursor-not-allowed disabled:opacity-60"
                type="button"
                onClick={() => onDelete(destination)}
                disabled={isDeleting}
              >
                {isDeleting ? 'Briše se...' : 'Obriši'}
              </button>
            ) : null}
          </div>
        ) : null}
      </div>

      <div className="mt-5 flex flex-wrap gap-3 text-sm">
        <span className="rounded-full bg-amber-100 px-3 py-1 font-semibold text-amber-900">
          Dolazak: {destination.arrivalDate.slice(0, 10)}
        </span>
        <span className="rounded-full bg-sky-100 px-3 py-1 font-semibold text-sky-900">
          Odlazak: {destination.departureDate.slice(0, 10)}
        </span>
      </div>

      <p className="mt-4 rounded-[1.5rem] border border-white/50 bg-white/70 px-4 py-3 text-sm leading-6 text-slate-600">
        {destination.description || 'Nema dodatnog opisa za ovu destinaciju.'}
      </p>
    </article>
  )
}

export default DestinationCard
