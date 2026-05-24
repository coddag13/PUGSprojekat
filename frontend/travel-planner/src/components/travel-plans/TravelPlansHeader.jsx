function TravelPlansHeader({ firstName, onLogout }) {
  return (
    <header className="mb-8 flex flex-col gap-4 rounded-[2rem] bg-slate-950 px-6 py-6 text-white shadow-xl md:flex-row md:items-end md:justify-between">
      <div>
        <p className="text-sm uppercase tracking-[0.25em] text-amber-300">Dashboard</p>
        <h1 className="mt-2 text-4xl font-black tracking-tight">
          Zdravo, {firstName}
        </h1>
        <p className="mt-2 text-slate-300">
          Ovdje pocinje tvoj pregled putovanja, troskova i dnevnih planova.
        </p>
      </div>

      <button
        className="rounded-2xl border border-white/20 px-5 py-3 font-semibold text-white transition hover:bg-white/10"
        type="button"
        onClick={onLogout}
      >
        Logout
      </button>
    </header>
  )
}

export default TravelPlansHeader