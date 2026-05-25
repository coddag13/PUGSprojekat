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

export async function updateDestination(travelPlanId, destinationId, payload) {
  return httpClient(`/travel-plans/${travelPlanId}/destinations/${destinationId}`, {
    method: 'PUT',
    body: payload,
  })
}

export async function deleteDestination(travelPlanId, destinationId) {
  return httpClient(`/travel-plans/${travelPlanId}/destinations/${destinationId}`, {
    method: 'DELETE',
  })
}