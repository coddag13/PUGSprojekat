import { httpClient } from '../api/httpClient'
import { normalizeActivities, normalizeActivity } from '../models'

export async function getActivities(travelPlanId) {
  const data = await httpClient(`/travel-plans/${travelPlanId}/activities`)
  return normalizeActivities(data)
}

export async function createActivity(travelPlanId, payload) {
  const data = await httpClient(`/travel-plans/${travelPlanId}/activities`, {
    method: 'POST',
    body: payload,
  })
  return normalizeActivity(data)
}

export async function updateActivity(travelPlanId, activityId, payload) {
  const data = await httpClient(`/travel-plans/${travelPlanId}/activities/${activityId}`, {
    method: 'PUT',
    body: payload,
  })
  return data ? normalizeActivity(data) : null
}

export async function deleteActivity(travelPlanId, activityId) {
  return httpClient(`/travel-plans/${travelPlanId}/activities/${activityId}`, {
    method: 'DELETE',
  })
}
