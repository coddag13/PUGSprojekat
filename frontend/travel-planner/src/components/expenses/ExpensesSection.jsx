import { useEffect, useState } from 'react'
import {
  createExpense,
  deleteExpense,
  getExpenses,
  updateExpense,
} from '../../services/expenseService'
import { getActivities } from '../../services/activityService'
import ExpenseForm from './ExpenseForm'
import ExpenseList from './ExpenseList'

function ExpensesSection({ travelPlanId, plan }) {
  const emptyForm = {
    name: '',
    category: '0',
    amount: '',
    date: '',
    description: '',
  }

  const [expenses, setExpenses] = useState([])
  const [activities, setActivities] = useState([])
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [deletingExpenseId, setDeletingExpenseId] = useState(null)
  const [editingExpenseId, setEditingExpenseId] = useState(null)
  const [error, setError] = useState('')
  const [form, setForm] = useState(emptyForm)

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

  const resetForm = () => {
    setForm(emptyForm)
    setEditingExpenseId(null)
  }

  const getExpensesTotal = () => {
    return expenses.reduce((sum, expense) => sum + Number(expense.amount), 0)
  }

  const getActivitiesEstimatedTotal = () => {
    return activities.reduce((sum, activity) => sum + Number(activity.estimatedCost), 0)
  }

  const getCommittedAmount = (excludeExpenseId = null) => {
    const filteredExpenses = excludeExpenseId
      ? expenses.filter((expense) => expense.id !== excludeExpenseId)
      : expenses

    const expensesTotal = filteredExpenses.reduce((sum, expense) => sum + Number(expense.amount), 0)
    const activitiesTotal = getActivitiesEstimatedTotal()
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

    const committedAmount = getCommittedAmount(editingExpenseId)
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
      const payload = {
        name: form.name.trim(),
        category: Number(form.category),
        amount: Number(form.amount),
        date: `${form.date}T00:00:00`,
        description: form.description.trim(),
      }

      if (editingExpenseId) {
        await updateExpense(travelPlanId, editingExpenseId, payload)
      } else {
        await createExpense(travelPlanId, payload)
      }

      resetForm()
      await loadData()
    } catch (err) {
      setError(err.message)
    } finally {
      setSaving(false)
    }
  }

  const handleEdit = (expense) => {
    setError('')
    setEditingExpenseId(expense.id)
    setForm({
      name: expense.name,
      category: String(expense.category),
      amount: String(expense.amount),
      date: expense.date.slice(0, 10),
      description: expense.description ?? '',
    })
  }

  const handleDelete = async (expense) => {
    const confirmed = window.confirm(`Da li sigurno želiš obrisati trošak "${expense.name}"?`)

    if (!confirmed) {
      return
    }

    setError('')
    setDeletingExpenseId(expense.id)

    try {
      await deleteExpense(travelPlanId, expense.id)

      if (editingExpenseId === expense.id) {
        resetForm()
      }

      await loadData()
    } catch (err) {
      setError(err.message)
    } finally {
      setDeletingExpenseId(null)
    }
  }

  const expensesTotal = getExpensesTotal()
  const activitiesEstimatedTotal = getActivitiesEstimatedTotal()
  const committedAmount = expensesTotal + activitiesEstimatedTotal
  const remainingBudget = Number(plan.plannedBudget) - committedAmount

  return (
    <div className="space-y-6">
      <div className="rounded-[2rem] bg-slate-950 p-5 text-white shadow-lg">
        <p className="text-sm uppercase tracking-[0.2em] text-amber-300">Budžet</p>
        <div className="mt-4 grid gap-4 md:grid-cols-4">
          <div className="rounded-2xl bg-white/10 p-4">
            <p className="text-sm text-slate-300">Planirani budžet</p>
            <p className="mt-2 text-2xl font-bold">{plan.plannedBudget} EUR</p>
          </div>
          <div className="rounded-2xl bg-white/10 p-4">
            <p className="text-sm text-slate-300">Ukupni troškovi</p>
            <p className="mt-2 text-2xl font-bold">{expensesTotal.toFixed(2)} EUR</p>
          </div>
          <div className="rounded-2xl bg-white/10 p-4">
            <p className="text-sm text-slate-300">Procijenjene aktivnosti</p>
            <p className="mt-2 text-2xl font-bold">{activitiesEstimatedTotal.toFixed(2)} EUR</p>
          </div>
          <div className="rounded-2xl bg-white/10 p-4">
            <p className="text-sm text-slate-300">Preostali budžet</p>
            <p className="mt-2 text-2xl font-bold">{remainingBudget.toFixed(2)} EUR</p>
          </div>
        </div>

        <div className="mt-4 rounded-2xl bg-white/10 p-4">
          <p className="text-sm text-slate-300">Ukupno zauzeto</p>
          <p className="mt-2 text-2xl font-bold">{committedAmount.toFixed(2)} EUR</p>
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
          title={editingExpenseId ? 'Izmjena troška' : 'Novi trošak'}
          submitLabel={editingExpenseId ? 'Sačuvaj izmjene' : 'Dodaj trošak'}
          onChange={handleChange}
          onSubmit={handleSubmit}
          onCancel={resetForm}
          showCancel={Boolean(editingExpenseId)}
        />

        <ExpenseList
          expenses={expenses}
          loading={loading}
          onRefresh={loadData}
          onEdit={handleEdit}
          onDelete={handleDelete}
          deletingExpenseId={deletingExpenseId}
        />
      </div>
    </div>
  )
}

export default ExpensesSection