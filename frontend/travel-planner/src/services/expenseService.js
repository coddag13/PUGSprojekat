import { httpClient } from '../api/httpClient'

export async function getExpenses(travelPlanId) {
  return httpClient(`/travel-plans/${travelPlanId}/expenses`)
}

export async function createExpense(travelPlanId, payload) {
  return httpClient(`/travel-plans/${travelPlanId}/expenses`, {
    method: 'POST',
    body: payload,
  })
}

export async function updateExpense(travelPlanId, expenseId, payload) {
  return httpClient(`/travel-plans/${travelPlanId}/expenses/${expenseId}`, {
    method: 'PUT',
    body: payload,
  })
}

export async function deleteExpense(travelPlanId, expenseId) {
  return httpClient(`/travel-plans/${travelPlanId}/expenses/${expenseId}`, {
    method: 'DELETE',
  })
}