import { useEffect, useState } from 'react'

const STATUS_LABELS = {
  0: 'Planned',
  1: 'Reserved',
  2: 'Completed',
  3: 'Cancelled',
}

const STATUS_CLASSES = {
  0: 'bg-sky-100 text-sky-900 border-sky-200',
  1: 'bg-amber-100 text-amber-900 border-amber-200',
  2: 'bg-emerald-100 text-emerald-900 border-emerald-200',
  3: 'bg-rose-100 text-rose-900 border-rose-200',
}

function ActivityCard({ activity, destinationName, onStatusChange, isUpdating }) {
  const [selectedStatus, setSelectedStatus] = useState(String(activity.status))
  const [isEditingStatus, setIsEditingStatus] = useState(false)

  useEffect(() => {
    setSelectedStatus(String(activity.status))
  }, [activity.status])

  const handleStartEditing = () => {
    setSelectedStatus(String(activity.status))
    setIsEditingStatus(true)
  }

  const handleCancelEditing = () => {
    setSelectedStatus(String(activity.status))
    setIsEditingStatus(false)
  }

  const handleStatusSubmit = async () => {
    await onStatusChange(activity, Number(selectedStatus))
    setIsEditingStatus(false)
  }

  const statusClass =
    STATUS_CLASSES[Number(selectedStatus)] ?? 'bg-slate-100 text-slate-800 border-slate-200'

  const currentStatusClass =
    STATUS_CLASSES[activity.status] ?? 'bg-slate-100 text-slate-800 border-slate-200'

  return (
    <article className="rounded-3xl border border-slate-200 bg-[linear-gradient(135deg,#fffaf0_0%,#f8fbff_100%)] p-5 shadow-sm">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <h3 className="text-xl font-bold text-slate-900">{activity.name}</h3>
          <p className="mt-2 text-slate-700">{activity.location}</p>
        </div>

        <div className="rounded-2xl bg-slate-900 px-4 py-2 text-sm font-semibold text-white">
          {activity.estimatedCost} EUR
        </div>
      </div>

      <div className="mt-4 flex flex-wrap gap-3 text-sm text-slate-600">
        <span className="rounded-full bg-amber-100 px-3 py-1">{activity.date.slice(0, 10)}</span>
        <span className="rounded-full bg-sky-100 px-3 py-1">{activity.time}</span>
        <span className={`rounded-full border px-3 py-1 font-semibold ${currentStatusClass}`}>
          {STATUS_LABELS[activity.status] ?? 'Unknown'}
        </span>
      </div>

      {destinationName ? (
        <p className="mt-4 text-sm font-medium text-slate-600">
          Destinacija: {destinationName}
        </p>
      ) : null}

      {activity.description ? (
        <p className="mt-4 rounded-2xl bg-white/80 px-4 py-3 text-sm text-slate-600">
          {activity.description}
        </p>
      ) : null}

      {!isEditingStatus ? (
        <div className="mt-5 flex justify-end">
          <button
            className="rounded-2xl border border-slate-300 px-4 py-2 text-sm font-semibold text-slate-700 transition hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-60"
            type="button"
            onClick={handleStartEditing}
            disabled={isUpdating}
          >
            Promijeni status
          </button>
        </div>
      ) : (
        <div className="mt-5 rounded-2xl border border-slate-200 bg-white/80 p-4">
          <div className="grid gap-3 md:grid-cols-[1fr_auto_auto] md:items-end">
            <label className="block">
              <span className="mb-2 block text-sm font-semibold text-slate-700">
                Novi status
              </span>
              <select
                className={`w-full rounded-2xl border px-4 py-3 outline-none transition ${statusClass}`}
                value={selectedStatus}
                onChange={(event) => setSelectedStatus(event.target.value)}
                disabled={isUpdating}
              >
                <option value="0">Planned</option>
                <option value="1">Reserved</option>
                <option value="2">Completed</option>
                <option value="3">Cancelled</option>
              </select>
            </label>

            <button
              className="rounded-2xl bg-slate-900 px-4 py-3 text-sm font-bold text-white transition hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-60"
              type="button"
              onClick={handleStatusSubmit}
              disabled={isUpdating || Number(selectedStatus) === activity.status}
            >
              {isUpdating ? 'Čuva se...' : 'Sačuvaj'}
            </button>

            <button
              className="rounded-2xl border border-slate-300 px-4 py-3 text-sm font-semibold text-slate-700 transition hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-60"
              type="button"
              onClick={handleCancelEditing}
              disabled={isUpdating}
            >
              Otkaži
            </button>
          </div>
        </div>
      )}
    </article>
  )
}

export default ActivityCard