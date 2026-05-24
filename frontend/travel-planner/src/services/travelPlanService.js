import { httpClient } from '../api/httpClient'

export async function getTravelPlans() {
  return httpClient('/travel-plans')
}

export async function getTravelPlanById(id) {
  return httpClient(`/travel-plans/${id}`)
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