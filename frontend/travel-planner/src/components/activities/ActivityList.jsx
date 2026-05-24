import ActivityCard from './ActivityCard'

function ActivityList({
  activities,
  destinations,
  loading,
  onRefresh,
  onStatusChange,
  updatingActivityId,
  allowStatusEdit = true,
}) {
  const getDestinationName = (destinationId) => {
    if (!destinationId) {
      return ''
    }

    const destination = destinations.find((item) => item.id === destinationId)
    return destination?.name ?? ''
  }

  const sortedActivities = [...activities].sort((a, b) => {
    const dateCompare = a.date.localeCompare(b.date)
    if (dateCompare !== 0) {
      return dateCompare
    }

    return a.time.localeCompare(b.time)
  })

  const groupedActivities = sortedActivities.reduce((groups, activity) => {
    const dateKey = activity.date.slice(0, 10)

    if (!groups[dateKey]) {
      groups[dateKey] = []
    }

    groups[dateKey].push(activity)
    return groups
  }, {})

  const groupEntries = Object.entries(groupedActivities)

  return (
    <section className="rounded-[2rem] bg-white p-6 shadow-lg">
      <div className="mb-5 flex items-center justify-between">
        <h2 className="text-2xl font-bold text-slate-900">Aktivnosti</h2>

        <button
          className="rounded-2xl border border-slate-300 px-4 py-2 text-sm font-semibold text-slate-700 hover:bg-slate-50"
          type="button"
          onClick={onRefresh}
        >
          Osvježi
        </button>
      </div>

      {loading ? (
        <div className="rounded-3xl border border-dashed border-slate-300 p-10 text-center text-slate-500">
          Učitavanje aktivnosti...
        </div>
      ) : activities.length === 0 ? (
        <div className="rounded-3xl border border-dashed border-slate-300 p-10 text-center text-slate-500">
          Još nema unesenih aktivnosti.
        </div>
      ) : (
        <div className="space-y-6">
          {groupEntries.map(([date, dayActivities]) => (
            <div key={date} className="space-y-4">
              <div className="sticky top-0 z-10 rounded-2xl bg-slate-950 px-4 py-3 text-white shadow-sm">
                <p className="text-sm uppercase tracking-[0.2em] text-amber-300">Dan</p>
                <p className="mt-1 text-lg font-bold">{date}</p>
              </div>

              <div className="grid gap-4">
                {dayActivities.map((activity) => (
                  <ActivityCard
                    key={activity.id}
                    activity={activity}
                    destinationName={getDestinationName(activity.destinationId)}
                    onStatusChange={onStatusChange}
                    isUpdating={updatingActivityId === activity.id}
                    allowStatusEdit={allowStatusEdit}
                  />
                ))}
              </div>
            </div>
          ))}
        </div>
      )}
    </section>
  )
}

export default ActivityList