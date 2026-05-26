import { useEffect, useState } from 'react'
import {
  createActivityFormModel,
  createActivityPayload,
  createEmptyActivityForm,
} from '../../models'
import {
  createActivity,
  deleteActivity,
  getActivities,
  updateActivity,
} from '../../services/activityService'
import { getDestinations } from '../../services/destinationService'
import ActivityForm from './ActivityForm'
import ActivityList from './ActivityList'

function ActivitiesSection({ travelPlanId, plan }) {
  const [activities, setActivities] = useState([])
  const [destinations, setDestinations] = useState([])
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState('')
  const [updatingActivityId, setUpdatingActivityId] = useState(null)
  const [deletingActivityId, setDeletingActivityId] = useState(null)
  const [editingActivityId, setEditingActivityId] = useState(null)
  const [form, setForm] = useState(createEmptyActivityForm)

  const selectedDestination = destinations.find(
    (destination) => destination.id === form.destinationId,
  )

  const loadData = async () => {
    setLoading(true)
    setError('')

    try {
      const [activitiesData, destinationsData] = await Promise.all([
        getActivities(travelPlanId),
        getDestinations(travelPlanId),
      ])

      setActivities(activitiesData)
      setDestinations(destinationsData)
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadData()
  }, [travelPlanId])

  const handleChange = (event) => {
    const { name, value } = event.target
    setForm((current) => ({ ...current, [name]: value }))
  }

  const resetForm = () => {
    setForm(createEmptyActivityForm())
    setEditingActivityId(null)
  }

  const hasSameSlotConflict = () => {
    return activities.some((activity) => {
      if (activity.id === editingActivityId) {
        return false
      }

      return activity.date.slice(0, 10) === form.date && activity.time.slice(0, 5) === form.time
    })
  }

  const validateForm = () => {
    if (!form.name.trim()) {
      return 'Naziv aktivnosti je obavezan.'
    }

    if (!form.date || !form.time) {
      return 'Datum i vrijeme su obavezni.'
    }

    if (!form.location.trim()) {
      return 'Lokacija je obavezna.'
    }

    const estimatedCost = Number(form.estimatedCost)
    if (Number.isNaN(estimatedCost) || estimatedCost < 0) {
      return 'Procijenjeni trošak mora biti pozitivan broj ili nula.'
    }

    const activityDate = new Date(form.date)
    const planStart = new Date(plan.startDate.slice(0, 10))
    const planEnd = new Date(plan.endDate.slice(0, 10))

    if (activityDate < planStart || activityDate > planEnd) {
      return 'Datum aktivnosti mora biti unutar perioda plana putovanja.'
    }

    if (selectedDestination) {
      const destinationStart = new Date(selectedDestination.arrivalDate.slice(0, 10))
      const destinationEnd = new Date(selectedDestination.departureDate.slice(0, 10))

      if (activityDate < destinationStart || activityDate > destinationEnd) {
        return 'Datum aktivnosti mora biti unutar perioda izabrane destinacije.'
      }
    }

    if (hasSameSlotConflict()) {
      return 'Već postoji aktivnost u istom terminu.'
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
      const payload = createActivityPayload(form)

      if (editingActivityId) {
        await updateActivity(travelPlanId, editingActivityId, payload)
      } else {
        await createActivity(travelPlanId, payload)
      }

      resetForm()
      await loadData()
    } catch (err) {
      setError(err.message)
    } finally {
      setSaving(false)
    }
  }

  const handleEdit = (activity) => {
    setError('')
    setEditingActivityId(activity.id)
    setForm(createActivityFormModel(activity))
  }

  const handleDelete = async (activity) => {
    const confirmed = window.confirm(`Da li sigurno želiš obrisati aktivnost "${activity.name}"?`)

    if (!confirmed) {
      return
    }

    setError('')
    setDeletingActivityId(activity.id)

    try {
      await deleteActivity(travelPlanId, activity.id)

      if (editingActivityId === activity.id) {
        resetForm()
      }

      await loadData()
    } catch (err) {
      setError(err.message)
    } finally {
      setDeletingActivityId(null)
    }
  }

  const handleStatusChange = async (activity, newStatus) => {
    setError('')
    setUpdatingActivityId(activity.id)

    try {
      await updateActivity(travelPlanId, activity.id, {
        destinationId: activity.destinationId,
        name: activity.name,
        date: activity.date,
        time: activity.time,
        location: activity.location,
        description: activity.description ?? '',
        estimatedCost: activity.estimatedCost,
        status: newStatus,
      })

      await loadData()
    } catch (err) {
      setError(err.message)
    } finally {
      setUpdatingActivityId(null)
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
        <ActivityForm
          form={form}
          plan={plan}
          destinations={destinations}
          selectedDestination={selectedDestination}
          error=""
          saving={saving}
          title={editingActivityId ? 'Izmjena aktivnosti' : 'Nova aktivnost'}
          submitLabel={editingActivityId ? 'Sačuvaj izmjene' : 'Dodaj aktivnost'}
          onChange={handleChange}
          onSubmit={handleSubmit}
          onCancel={resetForm}
          showCancel={Boolean(editingActivityId)}
        />

        <ActivityList
          activities={activities}
          destinations={destinations}
          loading={loading}
          onRefresh={loadData}
          onStatusChange={handleStatusChange}
          onEdit={handleEdit}
          onDelete={handleDelete}
          updatingActivityId={updatingActivityId}
          deletingActivityId={deletingActivityId}
        />
      </div>
    </div>
  )
}

export default ActivitiesSection
