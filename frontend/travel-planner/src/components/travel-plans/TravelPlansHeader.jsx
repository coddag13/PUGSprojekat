import { Link } from 'react-router-dom'

function TravelPlansHeader({ firstName, isAdmin, onLogout }) {
  return (
    <header className="relative mb-8 overflow-hidden rounded-[2.25rem] bg-slate-950 px-6 py-7 text-white shadow-[0_30px_80px_rgba(15,23,42,0.28)] md:px-8 md:py-8">
      <div className="pointer-events-none absolute right-[-3rem] top-[-3rem] h-40 w-40 rounded-full bg-amber-300/20 blur-3xl" />
      <div className="pointer-events-none absolute bottom-[-4rem] left-[-2rem] h-44 w-44 rounded-full bg-sky-300/15 blur-3xl" />

      <div className="relative flex flex-col gap-5 md:flex-row md:items-end md:justify-between">
        <div className="max-w-2xl">
          <p className="text-sm uppercase tracking-[0.3em] text-amber-300">Tvoja putovanja</p>
          <h1 className="mt-3 text-4xl font-black tracking-tight md:text-5xl">
            Zdravo, {firstName}
          </h1>
          <p className="mt-3 text-slate-300">
            Organizuj planove, prati troškove i drži sve važne detalje puta na jednom mjestu.
          </p>
        </div>

        <div className="flex flex-wrap gap-3">
          {isAdmin ? (
            <Link
              className="rounded-2xl border border-amber-300/30 bg-amber-300/10 px-5 py-3 font-semibold text-amber-200 transition hover:bg-amber-300/20"
              to="/admin"
            >
              Administracija
            </Link>
          ) : null}

          <button
            className="rounded-2xl border border-white/15 px-5 py-3 font-semibold text-white transition hover:bg-white/10"
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
