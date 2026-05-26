import { httpClient } from '../api/httpClient'
import { normalizeExpense, normalizeExpenses } from '../models'

export async function getExpenses(travelPlanId) {
  const data = await httpClient(`/travel-plans/${travelPlanId}/expenses`)
  return normalizeExpenses(data)
}

export async function createExpense(travelPlanId, payload) {
  const data = await httpClient(`/travel-plans/${travelPlanId}/expenses`, {
    method: 'POST',
    body: payload,
  })
  return normalizeExpense(data)
}

export async function updateExpense(travelPlanId, expenseId, payload) {
  const data = await httpClient(`/travel-plans/${travelPlanId}/expenses/${expenseId}`, {
    method: 'PUT',
    body: payload,
  })
  return normalizeExpense(data)
}

export async function deleteExpense(travelPlanId, expenseId) {
  return httpClient(`/travel-plans/${travelPlanId}/expenses/${expenseId}`, {
    method: 'DELETE',
  })
}
