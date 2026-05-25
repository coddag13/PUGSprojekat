import { QRCode } from 'react-qr-code'

const ACCESS_LABELS = {
  0: 'Pregled',
  1: 'Uređivanje',
}

const ACCESS_CLASSES = {
  0: 'bg-sky-100 text-sky-900',
  1: 'bg-amber-100 text-amber-900',
}

function ShareTokenCard({ token, sharedUrl, onDelete, isDeleting, onCopy }) {
  const accessClass = ACCESS_CLASSES[token.accessType] ?? 'bg-slate-100 text-slate-900'

  return (
    <article className="rounded-3xl border border-slate-200 bg-[linear-gradient(135deg,#fffaf0_0%,#f8fbff_100%)] p-5 shadow-sm">
      <div className="grid gap-5 lg:grid-cols-[1fr_144px] lg:items-start">
        <div>
          <div className="flex flex-wrap items-center gap-3">
            <span className={`rounded-full px-3 py-1 text-sm font-semibold ${accessClass}`}>
              {ACCESS_LABELS[token.accessType] ?? 'Nepoznato'}
            </span>
            <span className="rounded-full bg-slate-100 px-3 py-1 text-sm text-slate-700">
              Ističe: {token.expiresAt.replace('T', ' ').slice(0, 16)}
            </span>
          </div>

          <p className="mt-4 break-all rounded-2xl bg-white/80 px-4 py-3 text-sm text-slate-700">
            {sharedUrl}
          </p>

          <div className="mt-5 flex flex-wrap gap-3">
            <button
              className="rounded-2xl border border-slate-300 px-4 py-2 text-sm font-semibold text-slate-700 transition hover:bg-slate-50"
              type="button"
              onClick={() => onCopy(sharedUrl)}
            >
              Kopiraj link
            </button>

            <button
              className="rounded-2xl bg-rose-600 px-4 py-2 text-sm font-bold text-white transition hover:bg-rose-700 disabled:cursor-not-allowed disabled:opacity-60"
              type="button"
              onClick={() => onDelete(token)}
              disabled={isDeleting}
            >
              {isDeleting ? 'Briše se...' : 'Obriši pristup'}
            </button>
          </div>
        </div>

        <div className="flex justify-center">
          <div className="rounded-[1.6rem] bg-white p-4 shadow-md">
            <QRCode value={sharedUrl} size={112} />
          </div>
        </div>
      </div>
    </article>
  )
}

export default ShareTokenCard
