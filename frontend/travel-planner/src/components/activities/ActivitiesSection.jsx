import { useEffect, useState } from 'react'
import { createActivity, getActivities, updateActivity } from '../../services/activityService'
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
  const [form, setForm] = useState({
    destinationId: '',
    name: '',
    date: '',
    time: '',
    location: '',
    description: '',
    estimatedCost: '',
    status: '0',
  })

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

  const hasSameSlotConflict = () => {
    return activities.some((activity) => {
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
      await createActivity(travelPlanId, {
        destinationId: form.destinationId || null,
        name: form.name.trim(),
        date: `${form.date}T00:00:00`,
        time: `${form.time}:00`,
        location: form.location.trim(),
        description: form.description.trim(),
        estimatedCost: Number(form.estimatedCost),
        status: Number(form.status),
      })

      setForm({
        destinationId: '',
        name: '',
        date: '',
        time: '',
        location: '',
        description: '',
        estimatedCost: '',
        status: '0',
      })

      await loadData()
    } catch (err) {
      setError(err.message)
    } finally {
      setSaving(false)
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
        <div className="rounded-2xl border border-rose-300 bg-rose-50 px-4 py-3 text-sm text-rose-700">
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
          onChange={handleChange}
          onSubmit={handleSubmit}
        />

        <ActivityList
          activities={activities}
          destinations={destinations}
          loading={loading}
          onRefresh={loadData}
          onStatusChange={handleStatusChange}
          updatingActivityId={updatingActivityId}
        />
      </div>
    </div>
  )
}

export default ActivitiesSection