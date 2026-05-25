import { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import ActivityList from '../components/activities/ActivityList'
import ChecklistForm from '../components/checklist/ChecklistForm'
import ChecklistList from '../components/checklist/ChecklistList'
import PlanOverviewSection from '../components/travel-plan-details/PlanOverviewSection'
import SharedDestinationsSection from '../components/shared-plan/SharedDestinationsSection'
import SharedExpensesSection from '../components/shared-plan/SharedExpensesSection'
import SharedPlanHeader from '../components/shared-plan/SharedPlanHeader'
import SharedPlanTabs from '../components/shared-plan/SharedPlanTabs'
import {
  createSharedChecklistItem,
  getSharedPlan,
  updateSharedActivityStatus,
  updateSharedChecklistItem,
} from '../services/sharedPlanService'

function SharedPlanPage() {
  const { token } = useParams()
  const [shared, setShared] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')
  const [activeTab, setActiveTab] = useState('overview')
  const [updatingActivityId, setUpdatingActivityId] = useState(null)
  const [updatingChecklistId, setUpdatingChecklistId] = useState(null)
  const [savingChecklist, setSavingChecklist] = useState(false)
  const [checklistForm, setChecklistForm] = useState({ text: '' })

  const loadSharedPlan = async () => {
    setLoading(true)
    setError('')

    try {
      const data = await getSharedPlan(token)
      setShared(data)
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadSharedPlan()
  }, [token])

  const isEditMode = shared?.accessType === 1

  const handleActivityStatusChange = async (activity, newStatus) => {
    setError('')
    setSuccess('')
    setUpdatingActivityId(activity.id)

    try {
      await updateSharedActivityStatus(token, activity.id, { status: newStatus })
      setSuccess('Status aktivnosti je azuriran.')
      await loadSharedPlan()
    } catch (err) {
      setError(err.message)
    } finally {
      setUpdatingActivityId(null)
    }
  }

  const handleChecklistInputChange = (event) => {
    const { name, value } = event.target
    setChecklistForm((current) => ({ ...current, [name]: value }))
  }

  const handleChecklistSubmit = async (event) => {
    event.preventDefault()
    setError('')
    setSuccess('')

    if (!checklistForm.text.trim()) {
      setError('Tekst checklist stavke je obavezan.')
      return
    }

    setSavingChecklist(true)

    try {
      await createSharedChecklistItem(token, { text: checklistForm.text.trim() })
      setChecklistForm({ text: '' })
      setSuccess('Checklist stavka je dodana.')
      await loadSharedPlan()
    } catch (err) {
      setError(err.message)
    } finally {
      setSavingChecklist(false)
    }
  }

  const handleChecklistToggle = async (item) => {
    setError('')
    setSuccess('')
    setUpdatingChecklistId(item.id)

    try {
      await updateSharedChecklistItem(token, item.id, {
        text: item.text,
        isCompleted: !item.isCompleted,
      })
      setSuccess('Stavka liste je ažurirana.')
      await loadSharedPlan()
    } catch (err) {
      setError(err.message)
    } finally {
      setUpdatingChecklistId(null)
    }
  }

  const renderActiveSection = () => {
    switch (activeTab) {
      case 'overview':
        return <PlanOverviewSection plan={shared.plan} />
      case 'destinations':
        return <SharedDestinationsSection destinations={shared.destinations} />
      case 'activities':
        return (
          <ActivityList
            activities={shared.activities}
            destinations={shared.destinations}
            loading={false}
            onRefresh={loadSharedPlan}
            onStatusChange={handleActivityStatusChange}
            updatingActivityId={updatingActivityId}
            allowStatusEdit={isEditMode}
            allowItemActions={false}
          />
        )
      case 'expenses':
        return <SharedExpensesSection expenses={shared.expenses} />
      case 'checklist':
        return (
          <div className="space-y-6">
            {isEditMode ? (
              <ChecklistForm
                form={checklistForm}
                error=""
                saving={savingChecklist}
                onChange={handleChecklistInputChange}
                onSubmit={handleChecklistSubmit}
              />
            ) : null}

            <ChecklistList
              items={shared.checklistItems}
              loading={false}
              onRefresh={loadSharedPlan}
              onToggle={handleChecklistToggle}
              updatingItemId={updatingChecklistId}
              allowToggle={isEditMode}
              title="Lista stvari"
            />
          </div>
        )
      default:
        return <PlanOverviewSection plan={shared.plan} />
    }
  }

  if (loading) {
    return (
      <main className="min-h-screen bg-[linear-gradient(180deg,#fff8ee_0%,#eef6ff_100%)] px-4 py-8">
        <div className="mx-auto max-w-7xl rounded-[2rem] bg-white p-10 text-center text-slate-500 shadow-lg">
          Učitavanje dijeljenog plana...
        </div>
      </main>
    )
  }

  if (error && !shared) {
    return (
      <main className="min-h-screen bg-[linear-gradient(180deg,#fff8ee_0%,#eef6ff_100%)] px-4 py-8">
        <div className="mx-auto max-w-7xl rounded-[2rem] border border-rose-300 bg-rose-50 p-10 text-center text-rose-700 shadow-lg">
          {error}
        </div>
      </main>
    )
  }

  return (
    <main className="min-h-screen bg-[linear-gradient(180deg,#fff8ee_0%,#eef6ff_100%)] px-4 py-8">
      <div className="mx-auto max-w-7xl space-y-6">
        <SharedPlanHeader plan={shared.plan} accessType={shared.accessType} />
        <SharedPlanTabs activeTab={activeTab} onChange={setActiveTab} />

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

        {renderActiveSection()}
      </div>
    </main>
  )
}

export default SharedPlanPage
