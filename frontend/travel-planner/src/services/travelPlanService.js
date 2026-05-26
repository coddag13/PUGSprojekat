import { httpClient } from '../api/httpClient'
import { normalizeTravelPlan, normalizeTravelPlans } from '../models'

export async function getTravelPlans() {
  const data = await httpClient('/travel-plans')
  return normalizeTravelPlans(data)
}

export async function getTravelPlanById(id) {
  const data = await httpClient(`/travel-plans/${id}`)
  return normalizeTravelPlan(data)
}

export async function createTravelPlan(payload) {
  return httpClient('/travel-plans', {
    method: 'POST',
    body: payload,
  })
}

export async function updateTravelPlan(id, payload) {
  return httpClient(`/travel-plans/${id}`, {
    method: 'PUT',
    body: payload,
  })
}

export async function deleteTravelPlan(id) {
  return httpClient(`/travel-plans/${id}`, {
    method: 'DELETE',
  })
}
