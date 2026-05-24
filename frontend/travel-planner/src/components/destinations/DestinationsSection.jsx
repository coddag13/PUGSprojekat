import { useEffect, useState } from 'react'
import { createDestination, getDestinations } from '../../services/destinationService'
import DestinationForm from './DestinationForm'
import DestinationList from './DestinationList'

function DestinationsSection({ travelPlanId }) {
  const [destinations, setDestinations] = useState([])
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState('')
  const [form, setForm] = useState({
    name: '',
    location: '',
    arrivalDate: '',
    departureDate: '',
    description: '',
  })

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

    if (new Date(form.departureDate) < new Date(form.arrivalDate)) {
      return 'Datum odlaska ne može biti prije datuma dolaska.'
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
      await createDestination(travelPlanId, {
        name: form.name.trim(),
        location: form.location.trim(),
        arrivalDate: `${form.arrivalDate}T00:00:00`,
        departureDate: `${form.departureDate}T00:00:00`,
        description: form.description.trim(),
      })

      setForm({
        name: '',
        location: '',
        arrivalDate: '',
        departureDate: '',
        description: '',
      })

      await loadDestinations()
    } catch (err) {
      setError(err.message)
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="grid gap-6 lg:grid-cols-[430px_1fr]">
      <DestinationForm
        form={form}
        error={error}
        saving={saving}
        onChange={handleChange}
        onSubmit={handleSubmit}
      />

      <DestinationList
        destinations={destinations}
        loading={loading}
        onRefresh={loadDestinations}
      />
    </div>
  )
}

export default DestinationsSection