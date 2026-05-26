import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import ActivitiesSection from '../components/activities/ActivitiesSection'
import ChecklistSection from '../components/checklist/ChecklistSection'
import DestinationsSection from '../components/destinations/DestinationsSection'
import ExpensesSection from '../components/expenses/ExpensesSection'
import RemindersSection from '../components/reminders/RemindersSection'
import SharingSection from '../components/sharing/SharingSection'
import PlanDetailsHeader from '../components/travel-plan-details/PlanDetailsHeader'
import PlanOverviewSection from '../components/travel-plan-details/PlanOverviewSection'
import PlanTabs from '../components/travel-plan-details/PlanTabs'
import {
  deleteTravelPlan,
  downloadTravelPlanReport,
  getTravelPlanById,
  updateTravelPlan,
} from '../services/travelPlanService'

function TravelPlanDetailsPage() {
  const { id } = useParams()
  const navigate = useNavigate()

  const [plan, setPlan] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [activeTab, setActiveTab] = useState('overview')
  const [savingPlan, setSavingPlan] = useState(false)
  const [deletingPlan, setDeletingPlan] = useState(false)
  const [downloadingReport, setDownloadingReport] = useState(false)
  const [planActionError, setPlanActionError] = useState('')

  const loadPlan = async () => {
    setLoading(true)
    setError('')

    try {
      const data = await getTravelPlanById(id)
      setPlan(data)
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadPlan()
  }, [id])

  const handleSavePlan = async (payload, validationError = '') => {
    setPlanActionError('')

    if (validationError) {
      setPlanActionError(validationError)
      return false
    }

    setSavingPlan(true)

    try {
      await updateTravelPlan(id, payload)
      await loadPlan()
      return true
    } catch (err) {
      setPlanActionError(err.message)
      return false
    } finally {
      setSavingPlan(false)
    }
  }

  const handleDeletePlan = async () => {
    const confirmed = window.confirm('Da li sigurno želiš obrisati ovaj plan putovanja?')

    if (!confirmed) {
      return
    }

    setPlanActionError('')
    setDeletingPlan(true)

    try {
      await deleteTravelPlan(id)
      navigate('/travel-plans')
    } catch (err) {
      setPlanActionError(err.message)
    } finally {
      setDeletingPlan(false)
    }
  }

  const handleDownloadReport = async () => {
    if (!plan) {
      return
    }

    setPlanActionError('')
    setDownloadingReport(true)

    try {
      const blob = await downloadTravelPlanReport(plan.id)
      const url = window.URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.download = `${plan.title}-izvjestaj.pdf`
      document.body.appendChild(link)
      link.click()
      link.remove()
      window.URL.revokeObjectURL(url)
    } catch (err) {
      setPlanActionError(err.message)
    } finally {
      setDownloadingReport(false)
    }
  }

  if (loading) {
    return (
      <main className="travel-shell min-h-screen px-4 py-8">
        <div className="glass-panel mx-auto max-w-7xl rounded-[2rem] p-10 text-center text-slate-500">
          Učitavanje detalja plana...
        </div>
      </main>
    )
  }

  if (error) {
    return (
      <main className="travel-shell min-h-screen px-4 py-8">
        <div className="mx-auto max-w-7xl rounded-[2rem] border border-rose-300 bg-rose-50 p-10 text-center text-rose-700 shadow-lg">
          {error}
        </div>
      </main>
    )
  }

  if (!plan) {
    return (
      <main className="travel-shell min-h-screen px-4 py-8">
        <div className="glass-panel mx-auto max-w-7xl rounded-[2rem] p-10 text-center text-slate-500">
          Plan nije pronađen.
        </div>
      </main>
    )
  }

  const renderActiveSection = () => {
    switch (activeTab) {
      case 'overview':
        return (
          <PlanOverviewSection
            plan={plan}
            onSave={handleSavePlan}
            onDelete={handleDeletePlan}
            saving={savingPlan}
            deleting={deletingPlan}
            error={planActionError}
          />
        )
      case 'destinations':
        return <DestinationsSection travelPlanId={plan.id} plan={plan} />
      case 'activities':
        return <ActivitiesSection travelPlanId={plan.id} plan={plan} />
      case 'expenses':
        return <ExpensesSection travelPlanId={plan.id} plan={plan} />
      case 'checklist':
        return <ChecklistSection travelPlanId={plan.id} />
      case 'reminders':
        return <RemindersSection travelPlanId={plan.id} />
      case 'sharing':
        return <SharingSection travelPlanId={plan.id} />
      default:
        return (
          <PlanOverviewSection
            plan={plan}
            onSave={handleSavePlan}
            onDelete={handleDeletePlan}
            saving={savingPlan}
            deleting={deletingPlan}
            error={planActionError}
          />
        )
    }
  }

  return (
    <main className="travel-shell min-h-screen px-4 py-8">
      <div className="relative mx-auto max-w-7xl space-y-6">
        <PlanDetailsHeader
          plan={plan}
          onDownloadReport={handleDownloadReport}
          downloadingReport={downloadingReport}
        />
        <PlanTabs activeTab={activeTab} onChange={setActiveTab} />
        {renderActiveSection()}
      </div>
    </main>
  )
}

export default TravelPlanDetailsPage
