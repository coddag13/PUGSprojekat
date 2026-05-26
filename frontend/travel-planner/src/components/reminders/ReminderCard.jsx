function ReminderCard({ reminder, onEdit, onDelete, onToggle, isUpdating, isDeleting, allowActions = true }) {
  const statusClasses = reminder.isCompleted
    ? 'bg-emerald-100 text-emerald-900'
    : 'bg-amber-100 text-amber-900'

  return (
    <article className="route-card rounded-[2rem] p-5">
      <div className="flex flex-col gap-4 md:flex-row md:items-start md:justify-between">
        <div className="space-y-3">
          <div className="flex flex-wrap items-center gap-3">
            <h3 className="text-xl font-black text-slate-950">{reminder.title}</h3>
            <span className={`rounded-full px-3 py-1 text-sm font-semibold ${statusClasses}`}>
              {reminder.isCompleted ? 'Završeno' : 'Aktivno'}
            </span>
          </div>

          <p className="text-sm font-medium text-slate-600">
            {reminder.remindAt.slice(0, 10)} u {reminder.remindAt.slice(11, 16)}
          </p>
        </div>

        {allowActions ? (
          <div className="flex flex-wrap gap-3">
            <button
              className="rounded-[1.25rem] border border-slate-300 bg-white/75 px-4 py-2.5 text-sm font-semibold text-slate-700 transition hover:bg-white disabled:cursor-not-allowed disabled:opacity-60"
              type="button"
              onClick={() => onToggle(reminder)}
              disabled={isUpdating}
            >
              {isUpdating ? 'Čuva se...' : reminder.isCompleted ? 'Vrati u aktivno' : 'Označi završeno'}
            </button>

            <button
              className="rounded-[1.25rem] border border-slate-300 bg-white/75 px-4 py-2.5 text-sm font-semibold text-slate-700 transition hover:bg-white"
              type="button"
              onClick={() => onEdit(reminder)}
            >
              Izmijeni
            </button>

            <button
              className="rounded-[1.25rem] bg-rose-600 px-4 py-2.5 text-sm font-bold text-white transition hover:bg-rose-700 disabled:cursor-not-allowed disabled:opacity-60"
              type="button"
              onClick={() => onDelete(reminder)}
              disabled={isDeleting}
            >
              {isDeleting ? 'Briše se...' : 'Obriši'}
            </button>
          </div>
        ) : null}
      </div>
    </article>
  )
}

export default ReminderCard
