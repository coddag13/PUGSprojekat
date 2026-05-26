const CATEGORY_LABELS = {
  0: 'Prevoz',
  1: 'Smještaj',
  2: 'Hrana',
  3: 'Aktivnost',
  4: 'Kupovina',
  5: 'Ostalo',
}

const CATEGORY_CLASSES = {
  0: 'bg-sky-100 text-sky-900',
  1: 'bg-indigo-100 text-indigo-900',
  2: 'bg-amber-100 text-amber-900',
  3: 'bg-emerald-100 text-emerald-900',
  4: 'bg-pink-100 text-pink-900',
  5: 'bg-slate-100 text-slate-900',
}

function ExpenseCard({ expense, onEdit, onDelete, isDeleting }) {
  const categoryClass = CATEGORY_CLASSES[expense.category] ?? 'bg-slate-100 text-slate-900'

  return (
    <article className="route-card rounded-[2rem] p-5">
      <div className="flex flex-wrap items-start justify-between gap-4">
        <div className="max-w-2xl">
          <div className="flex flex-wrap items-center gap-2">
            <span className={`rounded-full px-3 py-1 text-xs font-semibold uppercase tracking-[0.22em] ${categoryClass}`}>
              {CATEGORY_LABELS[expense.category] ?? 'Nepoznato'}
            </span>
            <span className="rounded-full bg-white/75 px-3 py-1 text-xs font-semibold text-slate-600">
              {expense.date.slice(0, 10)}
            </span>
          </div>

          <h3 className="mt-4 text-2xl font-black tracking-tight text-slate-950">{expense.name}</h3>
        </div>

        <div className="flex flex-wrap gap-3">
          <div className="rounded-[1.5rem] bg-slate-950 px-5 py-4 text-right text-white shadow-lg">
            <p className="text-xs uppercase tracking-[0.3em] text-slate-300">Iznos</p>
            <p className="mt-2 text-xl font-black text-amber-300">{expense.amount} EUR</p>
          </div>

          {onEdit ? (
            <button
              className="rounded-[1.25rem] border border-slate-300 bg-white/75 px-4 py-2.5 text-sm font-semibold text-slate-700 transition hover:bg-white"
              type="button"
              onClick={() => onEdit(expense)}
            >
              Izmijeni
            </button>
          ) : null}

          {onDelete ? (
            <button
              className="rounded-[1.25rem] bg-rose-600 px-4 py-2.5 text-sm font-bold text-white transition hover:bg-rose-700 disabled:cursor-not-allowed disabled:opacity-60"
              type="button"
              onClick={() => onDelete(expense)}
              disabled={isDeleting}
            >
              {isDeleting ? 'Briše se...' : 'Obriši'}
            </button>
          ) : null}
        </div>
      </div>

      <p className="mt-4 rounded-[1.5rem] border border-white/50 bg-white/70 px-4 py-3 text-sm leading-6 text-slate-600">
        {expense.description || 'Nema dodatnog opisa za ovaj trošak.'}
      </p>
    </article>
  )
}

export default ExpenseCard
