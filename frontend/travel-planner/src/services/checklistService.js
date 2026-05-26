import { httpClient } from '../api/httpClient'
import { normalizeChecklistItem, normalizeChecklistItems } from '../models'

export async function getChecklistItems(travelPlanId) {
  const data = await httpClient(`/travel-plans/${travelPlanId}/checklist-items`)
  return normalizeChecklistItems(data)
}

export async function createChecklistItem(travelPlanId, payload) {
  const data = await httpClient(`/travel-plans/${travelPlanId}/checklist-items`, {
    method: 'POST',
    body: payload,
  })
  return normalizeChecklistItem(data)
}

export async function updateChecklistItem(travelPlanId, itemId, payload) {
  const data = await httpClient(`/travel-plans/${travelPlanId}/checklist-items/${itemId}`, {
    method: 'PUT',
    body: payload,
  })
  return normalizeChecklistItem(data)
}
