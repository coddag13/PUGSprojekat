function ActivityCalendar({ activities }) {
  const now = new Date()
  const [year, month] = (() => {
    if (activities.length > 0) {
      const first = [...activities].sort((a, b) => a.date.localeCompare(b.date))[0]
      const activityDate = new Date(first.date)
      return [activityDate.getFullYear(), activityDate.getMonth()]
    }

    return [now.getFullYear(), now.getMonth()]
  })()

  const firstDay = new Date(year, month, 1)
  const lastDay = new Date(year, month + 1, 0)
  const daysInMonth = lastDay.getDate()
  const startWeekday = (firstDay.getDay() + 6) % 7

  const sortedActivities = [...activities].sort((a, b) => {
    const dateCompare = a.date.localeCompare(b.date)
    if (dateCompare !== 0) {
      return dateCompare
    }

    return a.time.localeCompare(b.time)
  })

  const groupedByDate = sortedActivities.reduce((groups, activity) => {
    const dateKey = activity.date.slice(0, 10)

    if (!groups[dateKey]) {
      groups[dateKey] = []
    }

    groups[dateKey].push(activity)
    return groups
  }, {})

  const cells = []

  for (let i = 0; i < startWeekday; i += 1) {
    cells.push(null)
  }

  for (let day = 1; day <= daysInMonth; day += 1) {
    const date = new Date(year, month, day)
    const dateKey = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`
    cells.push({
      day,
      dateKey,
      activities: groupedByDate[dateKey] ?? [],
    })
  }

  return (
    <section className="glass-panel rounded-[2.2rem] p-6">
      <div className="mb-5 flex items-start justify-between gap-4">
        <div>
          <p className="text-sm uppercase tracking-[0.28em] text-slate-500">Kalendar</p>
          <h2 className="mt-2 text-3xl font-black tracking-tight text-slate-950">
            Aktivnosti po mjesecu
          </h2>
        </div>
        <div className="rounded-[1.25rem] bg-slate-950 px-4 py-2.5 text-sm font-semibold capitalize text-white">
          {firstDay.toLocaleString('sr-Latn-RS', { month: 'long', year: 'numeric' })}
        </div>
      </div>

      <div className="grid grid-cols-7 gap-3 text-center text-sm font-semibold text-slate-600">
        <div>Pon</div>
        <div>Uto</div>
        <div>Sri</div>
        <div>Čet</div>
        <div>Pet</div>
        <div>Sub</div>
        <div>Ned</div>
      </div>

      <div className="mt-4 grid grid-cols-7 gap-3">
        {cells.map((cell, index) =>
          cell ? (
            <div
              key={cell.dateKey}
              className="min-h-36 rounded-[1.5rem] border border-slate-200 bg-[linear-gradient(180deg,rgba(255,255,255,0.9)_0%,rgba(248,251,255,0.95)_100%)] p-3 shadow-sm"
            >
              <div className="mb-3 flex items-center justify-between">
                <span className="text-sm font-black text-slate-950">{cell.day}</span>
                {cell.activities.length > 0 ? (
                  <span className="rounded-full bg-amber-100 px-2 py-1 text-xs font-semibold text-amber-900">
                    {cell.activities.length}
                  </span>
                ) : null}
              </div>

              <div className="space-y-2">
                {cell.activities.map((activity) => (
                  <div
                    key={activity.id}
                    className="rounded-[1rem] bg-slate-950 px-2 py-2 text-xs text-white shadow-sm"
                  >
                    <p className="font-semibold text-amber-300">{activity.time.slice(0, 5)}</p>
                    <p className="mt-1 truncate">{activity.name}</p>
                  </div>
                ))}
              </div>
            </div>
          ) : (
            <div key={`empty-${index}`} className="min-h-36 rounded-[1.5rem] bg-transparent" />
          ),
        )}
      </div>
    </section>
  )
}

export default ActivityCalendar
