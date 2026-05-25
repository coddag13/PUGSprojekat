function ShareTokenForm({ form, error, saving, onChange, onSubmit }) {
  return (
    <section className="rounded-[2rem] border border-white/70 bg-white/85 p-6 shadow-[0_18px_50px_rgba(15,23,42,0.08)] backdrop-blur-sm">
      <h2 className="text-2xl font-black text-slate-900">Podijeli plan</h2>

      <form className="mt-6 space-y-4" onSubmit={onSubmit}>
        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">Tip pristupa</span>
          <select
            className="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none transition focus:border-amber-500"
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
            className="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none transition focus:border-amber-500"
            type="datetime-local"
            name="expiresAt"
            value={form.expiresAt}
            onChange={onChange}
            required
          />
        </label>

        {error ? (
          <div className="rounded-2xl border border-rose-300 bg-rose-50 px-4 py-3 text-sm text-rose-700">
            {error}
          </div>
        ) : null}

        <button
          className="w-full rounded-2xl bg-amber-300 px-4 py-3 font-bold text-slate-900 transition hover:bg-amber-200 disabled:cursor-not-allowed disabled:opacity-60"
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
