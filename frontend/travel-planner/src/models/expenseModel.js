export function createEmptyExpenseForm() {
  return {
    name: '',
    category: '0',
    amount: '',
    date: '',
    description: '',
  }
}

export function createExpensePayload(form) {
  return {
    name: form.name.trim(),
    category: Number(form.category),
    amount: Number(form.amount),
    date: `${form.date}T00:00:00`,
    description: form.description.trim(),
  }
}

export function createExpenseFormModel(expense) {
  return {
    name: expense.name ?? '',
    category: String(expense.category ?? 0),
    amount: String(expense.amount ?? ''),
    date: expense.date?.slice(0, 10) ?? '',
    description: expense.description ?? '',
  }
}

export function normalizeExpense(expense) {
  return {
    id: expense.id ?? '',
    travelPlanId: expense.travelPlanId ?? '',
    name: expense.name ?? '',
    category: Number(expense.category ?? 0),
    amount: Number(expense.amount ?? 0),
    date: expense.date ?? '',
    description: expense.description ?? '',
  }
}

export function normalizeExpenses(expenses) {
  return expenses.map(normalizeExpense)
}
