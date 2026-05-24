import { httpClient } from '../api/httpClient'

export async function getChecklistItems(travelPlanId) {
  return httpClient(`/travel-plans/${travelPlanId}/checklist-items`)
}

export async function createChecklistItem(travelPlanId, payload) {
  return httpClient(`/travel-plans/${travelPlanId}/checklist-items`, {
    method: 'POST',
    body: payload,
  })
}

export async function updateChecklistItem(travelPlanId, itemId, payload) {
  return httpClient(`/travel-plans/${travelPlanId}/checklist-items/${itemId}`, {
    method: 'PUT',
    body: payload,
  })
}