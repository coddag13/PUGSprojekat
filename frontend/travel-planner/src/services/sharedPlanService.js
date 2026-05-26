import { httpClient } from '../api/httpClient'
import { normalizeChecklistItem, normalizeSharedPlan } from '../models'

export async function getSharedPlan(token) {
  const data = await httpClient(`/shared-plans/${token}`)
  return normalizeSharedPlan(data)
}

export async function updateSharedActivityStatus(token, activityId, payload) {
  return httpClient(`/shared-plans/${token}/activities/${activityId}`, {
    method: 'PUT',
    body: payload,
  })
}

export async function createSharedChecklistItem(token, payload) {
  const data = await httpClient(`/shared-plans/${token}/checklist-items`, {
    method: 'POST',
    body: payload,
  })
  return normalizeChecklistItem(data)
}

export async function updateSharedChecklistItem(token, itemId, payload) {
  const data = await httpClient(`/shared-plans/${token}/checklist-items/${itemId}`, {
    method: 'PUT',
    body: payload,
  })
  return normalizeChecklistItem(data)
}
