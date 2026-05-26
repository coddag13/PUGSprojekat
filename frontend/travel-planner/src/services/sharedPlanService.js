import { httpClient } from '../api/httpClient'

export async function getSharedPlan(token) {
  return httpClient(`/shared-plans/${token}`)
}

export async function updateSharedActivityStatus(token, activityId, payload) {
  return httpClient(`/shared-plans/${token}/activities/${activityId}`, {
    method: 'PUT',
    body: payload,
  })
}

export async function createSharedChecklistItem(token, payload) {
  return httpClient(`/shared-plans/${token}/checklist-items`, {
    method: 'POST',
    body: payload,
  })
}

export async function updateSharedChecklistItem(token, itemId, payload) {
  return httpClient(`/shared-plans/${token}/checklist-items/${itemId}`, {
    method: 'PUT',
    body: payload,
  })
}
