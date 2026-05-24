import { useEffect, useState } from 'react'
import {
  createShareToken,
  deleteShareToken,
  getShareTokens,
} from '../../services/shareTokenService'
import ShareTokenForm from './ShareTokenForm'
import ShareTokenList from './ShareTokenList'

function SharingSection({ travelPlanId }) {
  const [tokens, setTokens] = useState([])
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [deletingTokenId, setDeletingTokenId] = useState(null)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')
  const [form, setForm] = useState({
    accessType: '0',
    expiresAt: '',
  })

  const loadTokens = async () => {
    setLoading(true)
    setError('')

    try {
      const data = await getShareTokens(travelPlanId)
      setTokens(data)
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadTokens()
  }, [travelPlanId])

  const handleChange = (event) => {
    const { name, value } = event.target
    setForm((current) => ({ ...current, [name]: value }))
  }

  const validateForm = () => {
    if (!form.expiresAt) {
      return 'Datum isteka je obavezan.'
    }

    const selectedDate = new Date(form.expiresAt)
    const now = new Date()

    if (selectedDate <= now) {
      return 'Datum isteka mora biti u budućnosti.'
    }

    return ''
  }

  const handleSubmit = async (event) => {
    event.preventDefault()
    setError('')
    setSuccess('')

    const validationError = validateForm()
    if (validationError) {
      setError(validationError)
      return
    }

    setSaving(true)

    try {
      await createShareToken(travelPlanId, {
        accessType: Number(form.accessType),
        expiresAt: new Date(form.expiresAt).toISOString(),
      })

      setForm({
        accessType: '0',
        expiresAt: '',
      })

      setSuccess('Share token je uspješno kreiran.')
      await loadTokens()
    } catch (err) {
      setError(err.message)
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async (token) => {
    setError('')
    setSuccess('')
    setDeletingTokenId(token.id)

    try {
      await deleteShareToken(travelPlanId, token.id)
      setSuccess('Share token je obrisan.')
      await loadTokens()
    } catch (err) {
      setError(err.message)
    } finally {
      setDeletingTokenId(null)
    }
  }

  const handleCopy = async (value) => {
    setError('')
    setSuccess('')

    try {
      await navigator.clipboard.writeText(value)
      setSuccess('Link je kopiran.')
    } catch {
      setError('Kopiranje nije uspjelo.')
    }
  }

  return (
    <div className="space-y-6">
      {error ? (
        <div className="rounded-2xl border border-rose-300 bg-rose-50 px-4 py-3 text-sm text-rose-700">
          {error}
        </div>
      ) : null}

      {success ? (
        <div className="rounded-2xl border border-emerald-300 bg-emerald-50 px-4 py-3 text-sm text-emerald-700">
          {success}
        </div>
      ) : null}

      <div className="grid gap-6 lg:grid-cols-[430px_1fr]">
        <ShareTokenForm
          form={form}
          error=""
          saving={saving}
          onChange={handleChange}
          onSubmit={handleSubmit}
        />

        <ShareTokenList
          tokens={tokens}
          loading={loading}
          onRefresh={loadTokens}
          onDelete={handleDelete}
          deletingTokenId={deletingTokenId}
          onCopy={handleCopy}
        />
      </div>
    </div>
  )
}

export default SharingSection
