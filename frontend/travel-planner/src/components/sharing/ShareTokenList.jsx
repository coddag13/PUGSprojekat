import ShareTokenCard from './ShareTokenCard'

function ShareTokenList({ tokens, loading, onRefresh, onDelete, deletingTokenId, onCopy }) {
  const buildSharedUrl = (token) => {
    return `${window.location.origin}/shared/${token.token}`
  }

  return (
    <section className="glass-panel rounded-[2.2rem] p-6">
      <div className="mb-5 flex items-start justify-between gap-4">
        <div>
          <p className="text-sm uppercase tracking-[0.28em] text-slate-500">Aktivni pristupi</p>
          <h2 className="mt-2 text-3xl font-black tracking-tight text-slate-950">
            Linkovi za dijeljenje
          </h2>
        </div>

        <button
          className="rounded-[1.25rem] border border-slate-300 bg-white/75 px-4 py-2.5 text-sm font-semibold text-slate-700 transition hover:bg-white"
          type="button"
          onClick={onRefresh}
        >
          Osvježi
        </button>
      </div>

      {loading ? (
        <div className="rounded-[1.8rem] border border-dashed border-slate-300 bg-white/50 p-10 text-center text-slate-500">
          Učitavanje pristupa...
        </div>
      ) : tokens.length === 0 ? (
        <div className="rounded-[1.8rem] border border-dashed border-slate-300 bg-white/50 p-10 text-center text-slate-500">
          Još nema kreiranih pristupa za dijeljenje.
        </div>
      ) : (
        <div className="grid gap-4">
          {tokens.map((token) => (
            <ShareTokenCard
              key={token.id}
              token={token}
              sharedUrl={buildSharedUrl(token)}
              onDelete={onDelete}
              isDeleting={deletingTokenId === token.id}
              onCopy={onCopy}
            />
          ))}
        </div>
      )}
    </section>
  )
}

export default ShareTokenList
