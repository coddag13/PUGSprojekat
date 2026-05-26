import { httpClient } from '../api/httpClient'
import { normalizeDestination, normalizeDestinations } from '../models'

export async function getDestinations(travelPlanId) {
  const data = await httpClient(`/travel-plans/${travelPlanId}/destinations`)
  return normalizeDestinations(data)
}

export async function createDestination(travelPlanId, payload) {
  const data = await httpClient(`/travel-plans/${travelPlanId}/destinations`, {
    method: 'POST',
    body: payload,
  })
  return normalizeDestination(data)
}

export async function updateDestination(travelPlanId, destinationId, payload) {
  const data = await httpClient(`/travel-plans/${travelPlanId}/destinations/${destinationId}`, {
    method: 'PUT',
    body: payload,
  })
  return data ? normalizeDestination(data) : null
}

export async function deleteDestination(travelPlanId, destinationId) {
  return httpClient(`/travel-plans/${travelPlanId}/destinations/${destinationId}`, {
    method: 'DELETE',
  })
}
