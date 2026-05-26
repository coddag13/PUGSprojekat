import { useEffect, useState } from 'react'
import { createChecklistItemPayload, createEmptyChecklistForm } from '../../models'
import {
  createChecklistItem,
  getChecklistItems,
  updateChecklistItem,
} from '../../services/checklistService'
import ChecklistForm from './ChecklistForm'
import ChecklistList from './ChecklistList'

function ChecklistSection({ travelPlanId }) {
  const [items, setItems] = useState([])
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [updatingItemId, setUpdatingItemId] = useState(null)
  const [error, setError] = useState('')
  const [form, setForm] = useState(createEmptyChecklistForm)

  const loadItems = async () => {
    setLoading(true)
    setError('')

    try {
      const data = await getChecklistItems(travelPlanId)
      setItems(data)
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadItems()
  }, [travelPlanId])

  const handleChange = (event) => {
    const { name, value } = event.target
    setForm((current) => ({ ...current, [name]: value }))
  }

  const hasDuplicate = () => {
    const normalized = form.text.trim().toLowerCase()
    return items.some((item) => item.text.trim().toLowerCase() === normalized)
  }

  const validateForm = () => {
    if (!form.text.trim()) {
      return 'Tekst checklist stavke je obavezan.'
    }

    if (hasDuplicate()) {
      return 'Checklist stavka već postoji.'
    }

    return ''
  }

  const handleSubmit = async (event) => {
    event.preventDefault()
    setError('')

    const validationError = validateForm()
    if (validationError) {
      setError(validationError)
      return
    }

    setSaving(true)

    try {
      await createChecklistItem(travelPlanId, createChecklistItemPayload(form))

      setForm(createEmptyChecklistForm())

      await loadItems()
    } catch (err) {
      setError(err.message)
    } finally {
      setSaving(false)
    }
  }

  const handleToggle = async (item) => {
    setError('')
    setUpdatingItemId(item.id)

    try {
      await updateChecklistItem(travelPlanId, item.id, {
        text: item.text,
        isCompleted: !item.isCompleted,
      })

      await loadItems()
    } catch (err) {
      setError(err.message)
    } finally {
      setUpdatingItemId(null)
    }
  }

  return (
    <div className="space-y-6">
      {error ? (
        <div className="rounded-[1.3rem] border border-rose-300 bg-rose-50 px-4 py-3 text-sm text-rose-700">
          {error}
        </div>
      ) : null}

      <div className="grid gap-6 lg:grid-cols-[430px_1fr]">
        <ChecklistForm
          form={form}
          error=""
          saving={saving}
          onChange={handleChange}
          onSubmit={handleSubmit}
        />

        <ChecklistList
          items={items}
          loading={loading}
          onRefresh={loadItems}
          onToggle={handleToggle}
          updatingItemId={updatingItemId}
        />
      </div>
    </div>
  )
}

export default ChecklistSection
