import ActivityCard from './ActivityCard'
import ActivityCalendar from './ActivityCalendar'

function ActivityList({
  activities,
  destinations,
  loading,
  onRefresh,
  onStatusChange,
  onEdit,
  onDelete,
  updatingActivityId,
  deletingActivityId,
  allowStatusEdit = true,
  allowItemActions = true,
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
    <div className="space-y-6">
      <section className="glass-panel rounded-[2.2rem] p-6">
        <div className="mb-5 flex items-start justify-between gap-4">
          <div>
            <p className="text-sm uppercase tracking-[0.28em] text-slate-500">Raspored</p>
            <h2 className="mt-2 text-3xl font-black tracking-tight text-slate-950">
              Aktivnosti po danima
            </h2>
          </div>

          <button
            className="rounded-[1.25rem] border border-slate-300 bg-white/75 px-4 py-2.5 text-sm font-semibold text-slate-700 transition hover:bg-white"
            type="button"
            onClick={onRefresh}
          >
            Osvježi
          </button>
        </div>

        {loading ? (
          <div className="rounded-[1.8rem] border border-dashed border-slate-300 bg-white/50 p-10 text-center text-slate-500">
            Učitavanje aktivnosti...
          </div>
        ) : activities.length === 0 ? (
          <div className="rounded-[1.8rem] border border-dashed border-slate-300 bg-white/50 p-10 text-center text-slate-500">
            Još nema unesenih aktivnosti.
          </div>
        ) : (
          <div className="space-y-6">
            {groupEntries.map(([date, dayActivities]) => (
              <div key={date} className="space-y-4">
                <div className="rounded-[1.5rem] bg-slate-950 px-4 py-3 text-white shadow-sm">
                  <p className="text-sm uppercase tracking-[0.24em] text-amber-300">Dan</p>
                  <p className="mt-1 text-lg font-black">{date}</p>
                </div>

                <div className="grid gap-4">
                  {dayActivities.map((activity) => (
                    <ActivityCard
                      key={activity.id}
                      activity={activity}
                      destinationName={getDestinationName(activity.destinationId)}
                      onStatusChange={onStatusChange}
                      onEdit={onEdit}
                      onDelete={onDelete}
                      isUpdating={updatingActivityId === activity.id}
                      isDeleting={deletingActivityId === activity.id}
                      allowStatusEdit={allowStatusEdit}
                      allowItemActions={allowItemActions}
                    />
                  ))}
                </div>
              </div>
            ))}
          </div>
        )}
      </section>

      {!loading && activities.length > 0 ? <ActivityCalendar activities={activities} /> : null}
    </div>
  )
}

export default ActivityList
