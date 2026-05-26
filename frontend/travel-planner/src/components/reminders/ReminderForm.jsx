function ReminderForm({
  form,
  error,
  saving,
  title,
  submitLabel,
  onChange,
  onSubmit,
  onCancel,
  showCancel = false,
}) {
  return (
    <section className="glass-panel rounded-[2.2rem] p-6">
      <div className="mb-6 flex items-start justify-between gap-3">
        <div>
          <p className="text-sm uppercase tracking-[0.28em] text-sky-700">Podsjetnici</p>
          <h2 className="mt-2 text-3xl font-black tracking-tight text-slate-950">{title}</h2>
        </div>

        <div className="rounded-[1.35rem] bg-[linear-gradient(135deg,#e0f2fe_0%,#fef3c7_100%)] px-4 py-3 text-sm font-semibold text-slate-900 shadow-sm">
          Plan pripreme
        </div>
      </div>

      <form className="space-y-4" onSubmit={onSubmit}>
        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">Naslov podsjetnika</span>
          <input
            className="w-full rounded-[1.3rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-sky-500 focus:shadow-[0_0_0_4px_rgba(186,230,253,0.35)]"
            type="text"
            name="title"
            value={form.title}
            onChange={onChange}
            placeholder="Provjeriti rezervaciju smještaja"
            required
          />
        </label>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">Vrijeme podsjetnika</span>
          <input
            className="w-full rounded-[1.3rem] border border-slate-300 bg-white/90 px-4 py-3 outline-none transition focus:border-sky-500 focus:shadow-[0_0_0_4px_rgba(186,230,253,0.35)]"
            type="datetime-local"
            name="remindAt"
            value={form.remindAt}
            onChange={onChange}
            required
          />
        </label>

        {error ? (
          <div className="rounded-[1.3rem] border border-rose-300 bg-rose-50 px-4 py-3 text-sm text-rose-700">
            {error}
          </div>
        ) : null}

        <div className="flex gap-3">
          <button
            className="flex-1 rounded-[1.35rem] bg-[linear-gradient(90deg,#38bdf8_0%,#facc15_100%)] px-4 py-3.5 font-bold text-slate-950 shadow-[0_18px_36px_rgba(15,23,42,0.14)] transition hover:brightness-105 disabled:cursor-not-allowed disabled:opacity-60"
            type="submit"
            disabled={saving}
          >
            {saving ? 'Čuva se...' : submitLabel}
          </button>

          {showCancel ? (
            <button
              className="rounded-[1.25rem] border border-slate-300 bg-white/75 px-4 py-3 text-sm font-semibold text-slate-700 transition hover:bg-white"
              type="button"
              onClick={onCancel}
              disabled={saving}
            >
              Otkaži
            </button>
          ) : null}
        </div>
      </form>
    </section>
  )
}

export default ReminderForm
