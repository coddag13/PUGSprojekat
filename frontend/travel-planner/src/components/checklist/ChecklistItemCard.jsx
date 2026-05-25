function ChecklistItemCard({ item, onToggle, isUpdating, allowToggle = true }) {
  return (
    <article className="rounded-3xl border border-slate-200 bg-[linear-gradient(135deg,#fffaf0_0%,#f8fbff_100%)] p-5 shadow-sm">
      <div className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
        <div className="flex items-start gap-4">
          {allowToggle ? (
            <button
              className={[
                'mt-1 flex h-6 w-6 items-center justify-center rounded-full border transition',
                item.isCompleted
                  ? 'border-emerald-500 bg-emerald-500 text-white'
                  : 'border-slate-300 bg-white text-transparent',
              ].join(' ')}
              type="button"
              onClick={() => onToggle(item)}
              disabled={isUpdating}
            >
              ✓
            </button>
          ) : (
            <span
              className={[
                'mt-1 flex h-6 w-6 items-center justify-center rounded-full border',
                item.isCompleted
                  ? 'border-emerald-500 bg-emerald-500 text-white'
                  : 'border-slate-300 bg-white text-transparent',
              ].join(' ')}
            >
              ✓
            </span>
          )}

          <div>
            <p
              className={[
                'text-base font-medium',
                item.isCompleted ? 'text-slate-400 line-through' : 'text-slate-900',
              ].join(' ')}
            >
              {item.text}
            </p>

            <p
              className={[
                'mt-2 inline-flex rounded-full px-3 py-1 text-sm font-semibold',
                item.isCompleted
                  ? 'bg-emerald-100 text-emerald-900'
                  : 'bg-amber-100 text-amber-900',
              ].join(' ')}
            >
              {item.isCompleted ? 'Završeno' : 'U toku'}
            </p>
          </div>
        </div>

        {allowToggle ? (
          <button
            className="rounded-2xl border border-slate-300 px-4 py-2 text-sm font-semibold text-slate-700 transition hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-60"
            type="button"
            onClick={() => onToggle(item)}
            disabled={isUpdating}
          >
            {isUpdating
              ? 'Cuva se...'
              : item.isCompleted
              ? 'Vrati na nezavrseno'
              : 'Oznaci kao zavrseno'}
          </button>
        ) : null}
      </div>
    </article>
  )
}

export default ChecklistItemCard
