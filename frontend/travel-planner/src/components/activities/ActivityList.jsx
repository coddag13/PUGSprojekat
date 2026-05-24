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
        <div className="grid gap-4">
          {activities.map((activity) => (
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
      )}
    </section>
  )
}

export default ActivityList
