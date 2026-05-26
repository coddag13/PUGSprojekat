import { createFormValidationHandlers } from '../../utils/formValidation'

function ShareTokenForm({ form, error, saving, onChange, onSubmit }) {
  const formValidation = createFormValidationHandlers()

  return (
    <section className="glass-panel rounded-[2.2rem] p-6">
      <div className="mb-6 flex items-start justify-between gap-3">
        <div>
          <p className="text-sm uppercase tracking-[0.28em] text-amber-700">Dijeljenje</p>
          <h2 className="mt-2 text-3xl font-black tracking-tight text-slate-950">Podijeli plan</h2>
        </div>

        <div className="rounded-[1.35rem] bg-[linear-gradient(135deg,#fef3c7_0%,#e0f2fe_100%)] px-4 py-3 text-sm font-semibold text-slate-900 shadow-sm">
          Link i QR kod
        </div>
      </div>

      <form className="mt-6 space-y-4" onSubmit={onSubmit} {...formValidation}>
        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">Tip pristupa</span>
          <select
            className="w-full rounded-[1.3rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
            name="accessType"
            value={form.accessType}
            onChange={onChange}
          >
            <option value="0">Pregled</option>
            <option value="1">Uređivanje</option>
          </select>
        </label>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">Važi do</span>
          <input
            className="w-full rounded-[1.3rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-amber-500 focus:shadow-[0_0_0_4px_rgba(253,230,138,0.35)]"
            type="datetime-local"
            name="expiresAt"
            value={form.expiresAt}
            onChange={onChange}
            required
          />
        </label>

        {error ? (
          <div className="rounded-[1.3rem] border border-rose-300 bg-rose-50 px-4 py-3 text-sm text-rose-700">
            {error}
          </div>
        ) : null}

        <button
          className="w-full rounded-[1.35rem] bg-[linear-gradient(90deg,#38bdf8_0%,#facc15_100%)] px-4 py-3.5 font-bold text-slate-950 shadow-[0_18px_36px_rgba(15,23,42,0.14)] transition hover:brightness-105 disabled:cursor-not-allowed disabled:opacity-60"
          type="submit"
          disabled={saving}
        >
          {saving ? 'Čuva se...' : 'Kreiraj pristup'}
        </button>
      </form>
    </section>
  )
}

export default ShareTokenForm
