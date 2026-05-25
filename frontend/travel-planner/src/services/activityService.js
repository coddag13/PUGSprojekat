import { httpClient } from '../api/httpClient'

export async function getActivities(travelPlanId) {
  return httpClient(`/travel-plans/${travelPlanId}/activities`)
}

export async function createActivity(travelPlanId, payload) {
  return httpClient(`/travel-plans/${travelPlanId}/activities`, {
    method: 'POST',
    body: payload,
  })
}

export async function updateActivity(travelPlanId, activityId, payload) {
  return httpClient(`/travel-plans/${travelPlanId}/activities/${activityId}`, {
    method: 'PUT',
    body: payload,
  })
}

export async function deleteActivity(travelPlanId, activityId) {
  return httpClient(`/travel-plans/${travelPlanId}/activities/${activityId}`, {
    method: 'DELETE',
  })
}