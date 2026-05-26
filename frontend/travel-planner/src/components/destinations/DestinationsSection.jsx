import { useEffect, useState } from 'react'
import {
  createDestinationFormModel,
  createDestinationPayload,
  createEmptyDestinationForm,
} from '../../models'
import {
  createDestination,
  deleteDestination,
  getDestinations,
  updateDestination,
} from '../../services/destinationService'
import DestinationForm from './DestinationForm'
import DestinationList from './DestinationList'

function DestinationsSection({ travelPlanId, plan }) {
  const [destinations, setDestinations] = useState([])
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [deletingDestinationId, setDeletingDestinationId] = useState(null)
  const [editingDestinationId, setEditingDestinationId] = useState(null)
  const [error, setError] = useState('')
  const [form, setForm] = useState(createEmptyDestinationForm)

  const loadDestinations = async () => {
    setLoading(true)
    setError('')

    try {
      const data = await getDestinations(travelPlanId)
      setDestinations(data)
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadDestinations()
  }, [travelPlanId])

  const handleChange = (event) => {
    const { name, value } = event.target
    setForm((current) => ({ ...current, [name]: value }))
  }

  const resetForm = () => {
    setForm(createEmptyDestinationForm())
    setEditingDestinationId(null)
  }

  const hasOverlap = () => {
    const newArrival = new Date(form.arrivalDate)
    const newDeparture = new Date(form.departureDate)

    return destinations.some((destination) => {
      if (destination.id === editingDestinationId) {
        return false
      }

      const existingArrival = new Date(destination.arrivalDate.slice(0, 10))
      const existingDeparture = new Date(destination.departureDate.slice(0, 10))

      return newArrival <= existingDeparture && newDeparture >= existingArrival
    })
  }

  const validateForm = () => {
    if (!form.name.trim()) {
      return 'Naziv destinacije je obavezan.'
    }

    if (!form.location.trim()) {
      return 'Lokacija je obavezna.'
    }

    if (!form.arrivalDate || !form.departureDate) {
      return 'Datum dolaska i odlaska su obavezni.'
    }

    const planStart = new Date(plan.startDate.slice(0, 10))
    const planEnd = new Date(plan.endDate.slice(0, 10))
    const arrival = new Date(form.arrivalDate)
    const departure = new Date(form.departureDate)

    if (departure < arrival) {
      return 'Datum odlaska ne može biti prije datuma dolaska.'
    }

    if (arrival < planStart) {
      return 'Datum dolaska ne može biti prije početka plana putovanja.'
    }

    if (departure > planEnd) {
      return 'Datum odlaska ne može biti poslije kraja plana putovanja.'
    }

    if (hasOverlap()) {
      return 'Period destinacije se preklapa sa već postojećom destinacijom.'
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
      const payload = createDestinationPayload(form)

      if (editingDestinationId) {
        await updateDestination(travelPlanId, editingDestinationId, payload)
      } else {
        await createDestination(travelPlanId, payload)
      }

      resetForm()
      await loadDestinations()
    } catch (err) {
      setError(err.message)
    } finally {
      setSaving(false)
    }
  }

  const handleEdit = (destination) => {
    setError('')
    setEditingDestinationId(destination.id)
    setForm(createDestinationFormModel(destination))
  }

  const handleDelete = async (destination) => {
    const confirmed = window.confirm(`Da li sigurno želiš obrisati destinaciju "${destination.name}"?`)

    if (!confirmed) {
      return
    }

    setError('')
    setDeletingDestinationId(destination.id)

    try {
      await deleteDestination(travelPlanId, destination.id)

      if (editingDestinationId === destination.id) {
        resetForm()
      }

      await loadDestinations()
    } catch (err) {
      setError(err.message)
    } finally {
      setDeletingDestinationId(null)
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
        <DestinationForm
          form={form}
          plan={plan}
          error=""
          saving={saving}
          submitLabel={editingDestinationId ? 'Sačuvaj izmjene' : 'Dodaj destinaciju'}
          title={editingDestinationId ? 'Izmjena destinacije' : 'Nova destinacija'}
          onChange={handleChange}
          onSubmit={handleSubmit}
          onCancel={resetForm}
          showCancel={Boolean(editingDestinationId)}
        />

        <DestinationList
          destinations={destinations}
          loading={loading}
          onRefresh={loadDestinations}
          onEdit={handleEdit}
          onDelete={handleDelete}
          deletingDestinationId={deletingDestinationId}
        />
      </div>
    </div>
  )
}

export default DestinationsSection
