import ChecklistItemCard from './ChecklistItemCard'

function ChecklistList({
  items,
  loading,
  onRefresh,
  onToggle,
  updatingItemId,
  allowToggle = true,
  title = 'Lista stvari',
}) {
  return (
    <section className="rounded-[2rem] bg-white p-6 shadow-lg">
      <div className="mb-5 flex items-center justify-between">
        <h2 className="text-2xl font-bold text-slate-900">{title}</h2>

        {onRefresh ? (
          <button
            className="rounded-2xl border border-slate-300 px-4 py-2 text-sm font-semibold text-slate-700 hover:bg-slate-50"
            type="button"
            onClick={onRefresh}
          >
            Osvjezi
          </button>
        ) : null}
      </div>

      {loading ? (
        <div className="rounded-3xl border border-dashed border-slate-300 p-10 text-center text-slate-500">
          Ucitavanje checklist stavki...
        </div>
      ) : items.length === 0 ? (
        <div className="rounded-3xl border border-dashed border-slate-300 p-10 text-center text-slate-500">
          Jos nema unesenih checklist stavki.
        </div>
      ) : (
        <div className="grid gap-4">
          {items.map((item) => (
            <ChecklistItemCard
              key={item.id}
              item={item}
              onToggle={onToggle}
              isUpdating={updatingItemId === item.id}
              allowToggle={allowToggle}
            />
          ))}
        </div>
      )}
    </section>
  )
}

export default ChecklistList
