import { httpClient } from '../api/httpClient'
import { normalizeTravelPlan, normalizeTravelPlans } from '../models'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL

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

export async function downloadTravelPlanReport(id) {
  const token = localStorage.getItem('travel_planner_token')
  const response = await fetch(`${API_BASE_URL}/travel-plans/${id}/reports/summary`, {
    headers: {
      Accept: 'application/pdf',
      Authorization: token ? `Bearer ${token}` : '',
    },
  })

  if (!response.ok) {
    const message = await response.text()
    throw new Error(message || 'Preuzimanje PDF izvještaja nije uspjelo.')
  }

  return response.blob()
}
