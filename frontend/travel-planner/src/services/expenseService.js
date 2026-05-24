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