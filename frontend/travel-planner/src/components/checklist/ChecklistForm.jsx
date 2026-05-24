function ChecklistForm({
  form,
  error,
  saving,
  onChange,
  onSubmit,
}) {
  return (
    <section className="rounded-[2rem] bg-white p-6 shadow-lg">
      <h2 className="text-2xl font-bold text-slate-900">Nova checklist stavka</h2>

      <form className="mt-6 space-y-4" onSubmit={onSubmit}>
        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">
            Tekst stavke
          </span>
          <textarea
            className="min-h-28 w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none transition focus:border-amber-500"
            name="text"
            value={form.text}
            onChange={onChange}
            placeholder="Spakovati pasoš"
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
          {saving ? 'Čuva se...' : 'Dodaj stavku'}
        </button>
      </form>
    </section>
  )
}

export default ChecklistForm