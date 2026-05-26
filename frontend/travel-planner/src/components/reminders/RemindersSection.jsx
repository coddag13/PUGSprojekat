import { useEffect, useState } from 'react'
import {
  createEmptyReminderForm,
  createReminderFormModel,
  createReminderPayload,
} from '../../models'
import {
  createReminder,
  deleteReminder,
  getReminders,
  updateReminder,
} from '../../services/reminderService'
import ReminderForm from './ReminderForm'
import ReminderList from './ReminderList'

function RemindersSection({ travelPlanId }) {
  const [reminders, setReminders] = useState([])
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [updatingReminderId, setUpdatingReminderId] = useState(null)
  const [deletingReminderId, setDeletingReminderId] = useState(null)
  const [editingReminderId, setEditingReminderId] = useState(null)
  const [error, setError] = useState('')
  const [form, setForm] = useState(createEmptyReminderForm)

  const loadReminders = async () => {
    setLoading(true)
    setError('')

    try {
      const data = await getReminders(travelPlanId)
      setReminders(data)
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadReminders()
  }, [travelPlanId])

  const handleChange = (event) => {
    const { name, value } = event.target
    setForm((current) => ({ ...current, [name]: value }))
  }

  const resetForm = () => {
    setForm(createEmptyReminderForm())
    setEditingReminderId(null)
  }

  const validateForm = () => {
    if (!form.title.trim()) {
      return 'Naslov podsjetnika je obavezan.'
    }

    if (!form.remindAt) {
      return 'Vrijeme podsjetnika je obavezno.'
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
      const payload = createReminderPayload(form, false)

      if (editingReminderId) {
        const existing = reminders.find((reminder) => reminder.id === editingReminderId)
        await updateReminder(travelPlanId, editingReminderId, {
          ...payload,
          isCompleted: existing?.isCompleted ?? false,
        })
      } else {
        await createReminder(travelPlanId, payload)
      }

      resetForm()
      await loadReminders()
    } catch (err) {
      setError(err.message)
    } finally {
      setSaving(false)
    }
  }

  const handleEdit = (reminder) => {
    setError('')
    setEditingReminderId(reminder.id)
    setForm(createReminderFormModel(reminder))
  }

  const handleToggle = async (reminder) => {
    if (!reminder) {
      return
    }

    setError('')
    setUpdatingReminderId(reminder.id)

    try {
      await updateReminder(travelPlanId, reminder.id, {
        title: reminder.title,
        remindAt: reminder.remindAt,
        isCompleted: !reminder.isCompleted,
      })
      await loadReminders()
    } catch (err) {
      setError(err.message)
    } finally {
      setUpdatingReminderId(null)
    }
  }

  const handleDelete = async (reminder) => {
    const confirmed = window.confirm(`Da li sigurno želiš obrisati podsjetnik "${reminder.title}"?`)
    if (!confirmed) {
      return
    }

    setError('')
    setDeletingReminderId(reminder.id)

    try {
      await deleteReminder(travelPlanId, reminder.id)
      if (editingReminderId === reminder.id) {
        resetForm()
      }
      await loadReminders()
    } catch (err) {
      setError(err.message)
    } finally {
      setDeletingReminderId(null)
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
        <ReminderForm
          form={form}
          error=""
          saving={saving}
          title={editingReminderId ? 'Izmjena podsjetnika' : 'Novi podsjetnik'}
          submitLabel={editingReminderId ? 'Sačuvaj izmjene' : 'Dodaj podsjetnik'}
          onChange={handleChange}
          onSubmit={handleSubmit}
          onCancel={resetForm}
          showCancel={Boolean(editingReminderId)}
        />

        <ReminderList
          reminders={reminders}
          loading={loading}
          onRefresh={loadReminders}
          onEdit={handleEdit}
          onDelete={handleDelete}
          onToggle={handleToggle}
          updatingReminderId={updatingReminderId}
          deletingReminderId={deletingReminderId}
        />
      </div>
    </div>
  )
}

export default RemindersSection
