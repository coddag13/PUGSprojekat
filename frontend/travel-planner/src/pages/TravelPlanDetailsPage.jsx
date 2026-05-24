import { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import PlanDetailsHeader from '../components/travel-plan-details/PlanDetailsHeader'
import PlanOverviewSection from '../components/travel-plan-details/PlanOverviewSection'
import { getTravelPlanById } from '../services/travelPlanService'

function TravelPlanDetailsPage() {
  const { id } = useParams()
  const [plan, setPlan] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
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

    loadPlan()
  }, [id])

  if (loading) {
    return (
      <main className="min-h-screen bg-[linear-gradient(180deg,#fff8ee_0%,#eef6ff_100%)] px-4 py-8">
        <div className="mx-auto max-w-7xl rounded-[2rem] bg-white p-10 text-center text-slate-500 shadow-lg">
          Učitavanje detalja plana...
        </div>
      </main>
    )
  }

  if (error) {
    return (
      <main className="min-h-screen bg-[linear-gradient(180deg,#fff8ee_0%,#eef6ff_100%)] px-4 py-8">
        <div className="mx-auto max-w-7xl rounded-[2rem] border border-rose-300 bg-rose-50 p-10 text-center text-rose-700 shadow-lg">
          {error}
        </div>
      </main>
    )
  }

  if (!plan) {
    return (
      <main className="min-h-screen bg-[linear-gradient(180deg,#fff8ee_0%,#eef6ff_100%)] px-4 py-8">
        <div className="mx-auto max-w-7xl rounded-[2rem] bg-white p-10 text-center text-slate-500 shadow-lg">
          Plan nije pronađen.
        </div>
      </main>
    )
  }

  return (
    <main className="min-h-screen bg-[linear-gradient(180deg,#fff8ee_0%,#eef6ff_100%)] px-4 py-8">
      <div className="mx-auto max-w-7xl space-y-6">
        <PlanDetailsHeader plan={plan} />
        <PlanOverviewSection plan={plan} />
      </div>
    </main>
  )
}

export default TravelPlanDetailsPage