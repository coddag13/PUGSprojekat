import { Link } from 'react-router-dom'

function TravelPlansHeader({ firstName, isAdmin, onLogout }) {
  return (
    <header className="dark-signal-panel relative mb-8 overflow-hidden rounded-[2.5rem] px-6 py-8 text-white md:px-8 md:py-10">
      <div className="pointer-events-none absolute left-[12%] top-[22%] h-px w-44 bg-gradient-to-r from-transparent via-amber-300/50 to-transparent" />
      <div className="pointer-events-none absolute right-[14%] top-[30%] h-px w-32 bg-gradient-to-r from-transparent via-sky-300/50 to-transparent" />

      <div className="relative flex flex-col gap-6 lg:flex-row lg:items-end lg:justify-between">
        <div className="max-w-3xl">
          <div className="flex flex-wrap items-center gap-3">
            <span className="signal-chip inline-flex rounded-full px-4 py-1.5 text-xs font-semibold uppercase tracking-[0.32em] text-amber-200">
              Travel Command
            </span>
            <span className="rounded-full border border-white/10 bg-white/5 px-4 py-1.5 text-xs uppercase tracking-[0.32em] text-slate-300">
              Aktivna ruta
            </span>
          </div>

          <h1 className="travel-heading mt-6 text-4xl font-black md:text-6xl">
            Zdravo, {firstName}
          </h1>
          <p className="mt-5 max-w-2xl text-lg leading-8 text-slate-300">
            Ovdje upravljaš svim planovima, pratiš budžet i usklađuješ svaku etapu putovanja bez
            gubljenja pregleda.
          </p>

          <div className="mt-8 grid max-w-3xl gap-4 md:grid-cols-3">
            <div className="route-kpi rounded-[1.7rem] bg-white/6 p-4 backdrop-blur-sm">
              <p className="text-xs uppercase tracking-[0.34em] text-slate-300">Mreža</p>
              <p className="mt-3 text-xl font-black">Planovi</p>
              <p className="mt-2 text-sm text-slate-300">Sve rute na jednom panelu.</p>
            </div>

            <div className="route-kpi rounded-[1.7rem] bg-amber-300/90 p-4 text-slate-950">
              <p className="text-xs uppercase tracking-[0.34em] text-amber-950/70">Kontrola</p>
              <p className="mt-3 text-xl font-black">Budžet</p>
              <p className="mt-2 text-sm text-amber-950/80">Troškovi uvijek pod nadzorom.</p>
            </div>

            <div className="route-kpi rounded-[1.7rem] bg-sky-300/85 p-4 text-slate-950">
              <p className="text-xs uppercase tracking-[0.34em] text-sky-950/70">Ritam</p>
              <p className="mt-3 text-xl font-black">Dani</p>
              <p className="mt-2 text-sm text-sky-950/80">Jasan pregled aktivnosti.</p>
            </div>
          </div>
        </div>

        <div className="flex flex-wrap gap-3">
          {isAdmin ? (
            <Link
              className="rounded-[1.35rem] border border-amber-300/30 bg-amber-300/10 px-5 py-3 font-semibold text-amber-200 transition hover:bg-amber-300/20"
              to="/admin"
            >
              Administracija
            </Link>
          ) : null}

          <button
            className="rounded-[1.35rem] border border-white/15 bg-white/5 px-5 py-3 font-semibold text-white transition hover:bg-white/10"
            type="button"
            onClick={onLogout}
          >
            Odjavi se
          </button>
        </div>
      </div>
    </header>
  )
}

export default TravelPlansHeader
