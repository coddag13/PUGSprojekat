import ShareTokenCard from './ShareTokenCard'

function ShareTokenList({
  tokens,
  loading,
  onRefresh,
  onDelete,
  deletingTokenId,
  onCopy,
}) {
  const buildSharedUrl = (token) => {
    return `${window.location.origin}/shared/${token.token}`
  }

  return (
    <section className="rounded-[2rem] bg-white p-6 shadow-lg">
      <div className="mb-5 flex items-center justify-between">
        <h2 className="text-2xl font-bold text-slate-900">Share tokeni</h2>

        <button
          className="rounded-2xl border border-slate-300 px-4 py-2 text-sm font-semibold text-slate-700 hover:bg-slate-50"
          type="button"
          onClick={onRefresh}
        >
          Osvjezi
        </button>
      </div>

      {loading ? (
        <div className="rounded-3xl border border-dashed border-slate-300 p-10 text-center text-slate-500">
          Ucitavanje tokena...
        </div>
      ) : tokens.length === 0 ? (
        <div className="rounded-3xl border border-dashed border-slate-300 p-10 text-center text-slate-500">
          Jos nema kreiranih share tokena.
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
