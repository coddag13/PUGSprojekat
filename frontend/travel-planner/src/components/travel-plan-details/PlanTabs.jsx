const tabs = [
  { key: 'overview', label: 'Pregled' },
  { key: 'destinations', label: 'Destinacije' },
  { key: 'activities', label: 'Aktivnosti' },
  { key: 'expenses', label: 'Troškovi' },
  { key: 'checklist', label: 'Lista stvari' },
  { key: 'sharing', label: 'Dijeljenje' },
]

function PlanTabs({ activeTab, onChange }) {
  return (
    <section className="rounded-[2rem] border border-white/70 bg-white/85 p-4 shadow-[0_18px_50px_rgba(15,23,42,0.08)] backdrop-blur-sm">
      <div className="flex flex-wrap gap-3">
        {tabs.map((tab) => {
          const isActive = activeTab === tab.key

          return (
            <button
              key={tab.key}
              type="button"
              onClick={() => onChange(tab.key)}
              className={[
                'rounded-2xl px-4 py-3 text-sm font-bold transition',
                isActive
                  ? 'bg-slate-950 text-white shadow-lg'
                  : 'border border-slate-300 bg-white text-slate-700 hover:bg-slate-50',
              ].join(' ')}
            >
              {tab.label}
            </button>
          )
        })}
      </div>
    </section>
  )
}

export default PlanTabs
