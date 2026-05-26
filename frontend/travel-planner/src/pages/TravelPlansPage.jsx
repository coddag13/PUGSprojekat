import { useEffect, useState } from 'react'
import { useAuth } from '../context/AuthContext'
import { createTravelPlan, getTravelPlans } from '../services/travelPlanService'
import TravelPlansHeader from '../components/travel-plans/TravelPlansHeader'
import TravelPlanForm from '../components/travel-plans/TravelPlanForm'
import TravelPlanList from '../components/travel-plans/TravelPlanList'

function TravelPlansPage() {
  const { user, logout } = useAuth()
  const [plans, setPlans] = useState([])
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [form, setForm] = useState({
    title: '',
    description: '',
    startDate: '',
    endDate: '',
    plannedBudget: '',
    notes: '',
  })

  const loadPlans = async () => {
    setLoading(true)
    setError('')

    try {
      const data = await getTravelPlans()
      setPlans(data)
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadPlans()
  }, [])

  const handleChange = (event) => {
    const { name, value } = event.target
    setForm((current) => ({ ...current, [name]: value }))
  }

  const validateForm = () => {
    const trimmedTitle = form.title.trim()
    const trimmedDescription = form.description.trim()
    const budget = Number(form.plannedBudget)

    if (!trimmedTitle) {
      return 'Naziv plana je obavezan.'
    }

    if (!trimmedDescription) {
      return 'Opis putovanja je obavezan.'
    }

    if (!form.startDate || !form.endDate) {
      return 'Početni i krajnji datum su obavezni.'
    }

    if (new Date(form.endDate) < new Date(form.startDate)) {
      return 'Krajnji datum ne može biti prije početnog.'
    }

    if (Number.isNaN(budget) || budget < 0) {
      return 'Planirani budžet mora biti pozitivan broj ili nula.'
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
      await createTravelPlan({
        title: form.title.trim(),
        description: form.description.trim(),
        startDate: `${form.startDate}T00:00:00`,
        endDate: `${form.endDate}T00:00:00`,
        plannedBudget: Number(form.plannedBudget),
        notes: form.notes.trim(),
      })

      setForm({
        title: '',
        description: '',
        startDate: '',
        endDate: '',
        plannedBudget: '',
        notes: '',
      })

      await loadPlans()
    } catch (err) {
      setError(err.message)
    } finally {
      setSaving(false)
    }
  }

  return (
    <main className="travel-shell min-h-screen px-4 py-8">
      <div className="relative mx-auto max-w-7xl">
        <TravelPlansHeader
          firstName={user?.firstName}
          isAdmin={user?.role === 'Admin'}
          onLogout={logout}
        />

        <div className="grid gap-6 lg:grid-cols-[430px_1fr]">
          <TravelPlanForm
            form={form}
            error={error}
            saving={saving}
            onChange={handleChange}
            onSubmit={handleSubmit}
          />

          <TravelPlanList
            plans={plans}
            loading={loading}
            onRefresh={loadPlans}
          />
        </div>
      </div>
    </main>
  )
}

export default TravelPlansPage
