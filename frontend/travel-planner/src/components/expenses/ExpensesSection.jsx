import { useEffect, useState } from 'react'
import { createExpense, getExpenses } from '../../services/expenseService'
import { getActivities } from '../../services/activityService'
import ExpenseForm from './ExpenseForm'
import ExpenseList from './ExpenseList'

function ExpensesSection({ travelPlanId, plan }) {
  const [expenses, setExpenses] = useState([])
  const [activities, setActivities] = useState([])
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState('')
  const [form, setForm] = useState({
    name: '',
    category: '0',
    amount: '',
    date: '',
    description: '',
  })

  const loadData = async () => {
    setLoading(true)
    setError('')

    try {
      const [expensesData, activitiesData] = await Promise.all([
        getExpenses(travelPlanId),
        getActivities(travelPlanId),
      ])

      setExpenses(expensesData)
      setActivities(activitiesData)
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

  const getCommittedAmount = () => {
    const expensesTotal = expenses.reduce((sum, expense) => sum + Number(expense.amount), 0)
    const activitiesTotal = activities.reduce((sum, activity) => sum + Number(activity.estimatedCost), 0)
    return expensesTotal + activitiesTotal
  }

  const validateForm = () => {
    if (!form.name.trim()) {
      return 'Naziv troška je obavezan.'
    }

    if (!form.date) {
      return 'Datum troška je obavezan.'
    }

    const amount = Number(form.amount)
    if (Number.isNaN(amount) || amount < 0) {
      return 'Iznos mora biti pozitivan broj ili nula.'
    }

    const expenseDate = new Date(form.date)
    const planStart = new Date(plan.startDate.slice(0, 10))
    const planEnd = new Date(plan.endDate.slice(0, 10))

    if (expenseDate < planStart || expenseDate > planEnd) {
      return 'Datum troška mora biti unutar perioda plana putovanja.'
    }

    const committedAmount = getCommittedAmount()
    if (committedAmount + amount > Number(plan.plannedBudget)) {
      return 'Ovaj trošak bi prešao planirani budžet.'
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
      await createExpense(travelPlanId, {
        name: form.name.trim(),
        category: Number(form.category),
        amount: Number(form.amount),
        date: `${form.date}T00:00:00`,
        description: form.description.trim(),
      })

      setForm({
        name: '',
        category: '0',
        amount: '',
        date: '',
        description: '',
      })

      await loadData()
    } catch (err) {
      setError(err.message)
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="space-y-6">
      <div className="rounded-[2rem] bg-slate-950 p-5 text-white shadow-lg">
        <p className="text-sm uppercase tracking-[0.2em] text-amber-300">Budžet</p>
        <div className="mt-4 grid gap-4 md:grid-cols-3">
          <div className="rounded-2xl bg-white/10 p-4">
            <p className="text-sm text-slate-300">Planirani budžet</p>
            <p className="mt-2 text-2xl font-bold">{plan.plannedBudget} EUR</p>
          </div>
          <div className="rounded-2xl bg-white/10 p-4">
            <p className="text-sm text-slate-300">Zauzeto ukupno</p>
            <p className="mt-2 text-2xl font-bold">{getCommittedAmount()} EUR</p>
          </div>
          <div className="rounded-2xl bg-white/10 p-4">
            <p className="text-sm text-slate-300">Preostalo</p>
            <p className="mt-2 text-2xl font-bold">
              {(Number(plan.plannedBudget) - getCommittedAmount()).toFixed(2)} EUR
            </p>
          </div>
        </div>
      </div>

      {error ? (
        <div className="rounded-2xl border border-rose-300 bg-rose-50 px-4 py-3 text-sm text-rose-700">
          {error}
        </div>
      ) : null}

      <div className="grid gap-6 lg:grid-cols-[430px_1fr]">
        <ExpenseForm
          form={form}
          plan={plan}
          error=""
          saving={saving}
          onChange={handleChange}
          onSubmit={handleSubmit}
        />

        <ExpenseList
          expenses={expenses}
          loading={loading}
          onRefresh={loadData}
        />
      </div>
    </div>
  )
}

export default ExpensesSection