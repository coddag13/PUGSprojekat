function ChecklistItemCard({ item, onToggle, isUpdating, allowToggle = true }) {
  return (
    <article className="route-card rounded-[2rem] p-5">
      <div className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
        <div className="flex items-start gap-4">
          {allowToggle ? (
            <button
              className={[
                'mt-1 flex h-7 w-7 items-center justify-center rounded-full border text-sm font-bold transition',
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
                'mt-1 flex h-7 w-7 items-center justify-center rounded-full border text-sm font-bold',
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
                'text-base font-semibold',
                item.isCompleted ? 'text-slate-400 line-through' : 'text-slate-950',
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
            className="rounded-[1.25rem] border border-slate-300 bg-white/75 px-4 py-2.5 text-sm font-semibold text-slate-700 transition hover:bg-white disabled:cursor-not-allowed disabled:opacity-60"
            type="button"
            onClick={() => onToggle(item)}
            disabled={isUpdating}
          >
            {isUpdating
              ? 'Čuva se...'
              : item.isCompleted
              ? 'Vrati na nezavršeno'
              : 'Označi kao završeno'}
          </button>
        ) : null}
      </div>
    </article>
  )
}

export default ChecklistItemCard
