const tabs = [
  { key: 'overview', label: 'Pregled' },
  { key: 'destinations', label: 'Destinacije' },
  { key: 'activities', label: 'Aktivnosti' },
  { key: 'expenses', label: 'Troškovi' },
  { key: 'checklist', label: 'Lista stvari' },
  { key: 'reminders', label: 'Podsjetnici' },
]

function SharedPlanTabs({ activeTab, onChange }) {
  return (
    <section className="glass-panel rounded-[2.2rem] p-4">
      <div className="flex flex-wrap gap-3">
        {tabs.map((tab) => {
          const isActive = activeTab === tab.key

          return (
            <button
              key={tab.key}
              type="button"
              onClick={() => onChange(tab.key)}
              className={[
                'rounded-[1.25rem] px-4 py-3 text-sm font-bold transition',
                isActive
                  ? 'bg-[linear-gradient(90deg,#facc15_0%,#f59e0b_55%,#0f172a_100%)] text-white shadow-[0_16px_32px_rgba(15,23,42,0.14)]'
                  : 'border border-slate-300 bg-white/80 text-slate-700 hover:bg-white',
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

export default SharedPlanTabs
