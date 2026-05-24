import { httpClient } from '../api/httpClient'

export async function getDestinations(travelPlanId) {
  return httpClient(`/travel-plans/${travelPlanId}/destinations`)
}

export async function createDestination(travelPlanId, payload) {
  return httpClient(`/travel-plans/${travelPlanId}/destinations`, {
    method: 'POST',
    body: payload,
  })
}