const CATEGORY_LABELS = {
  0: 'Transport',
  1: 'Accommodation',
  2: 'Food',
  3: 'Activity',
  4: 'Shopping',
  5: 'Other',
}

const CATEGORY_CLASSES = {
  0: 'bg-sky-100 text-sky-900',
  1: 'bg-indigo-100 text-indigo-900',
  2: 'bg-amber-100 text-amber-900',
  3: 'bg-emerald-100 text-emerald-900',
  4: 'bg-pink-100 text-pink-900',
  5: 'bg-slate-100 text-slate-900',
}

function ExpenseCard({ expense }) {
  const categoryClass = CATEGORY_CLASSES[expense.category] ?? 'bg-slate-100 text-slate-900'

  return (
    <article className="rounded-3xl border border-slate-200 bg-[linear-gradient(135deg,#fffaf0_0%,#f8fbff_100%)] p-5 shadow-sm">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <h3 className="text-xl font-bold text-slate-900">{expense.name}</h3>
          <div className="mt-3 flex flex-wrap gap-3 text-sm">
            <span className={`rounded-full px-3 py-1 font-semibold ${categoryClass}`}>
              {CATEGORY_LABELS[expense.category] ?? 'Unknown'}
            </span>
            <span className="rounded-full bg-slate-100 px-3 py-1 text-slate-700">
              {expense.date.slice(0, 10)}
            </span>
          </div>
        </div>

        <div className="rounded-2xl bg-slate-900 px-4 py-2 text-sm font-semibold text-white">
          {expense.amount} EUR
        </div>
      </div>

      {expense.description ? (
        <p className="mt-4 rounded-2xl bg-white/80 px-4 py-3 text-sm text-slate-600">
          {expense.description}
        </p>
      ) : null}
    </article>
  )
}

export default ExpenseCard