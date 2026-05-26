import { normalizeReminder, normalizeReminders } from '../models'
import { httpClient } from '../api/httpClient'

export async function getReminders(travelPlanId) {
  const data = await httpClient(`/travel-plans/${travelPlanId}/reminders`)
  return normalizeReminders(data)
}

export async function createReminder(travelPlanId, payload) {
  const data = await httpClient(`/travel-plans/${travelPlanId}/reminders`, {
    method: 'POST',
    body: payload,
  })
  return normalizeReminder(data)
}

export async function updateReminder(travelPlanId, reminderId, payload) {
  const data = await httpClient(`/travel-plans/${travelPlanId}/reminders/${reminderId}`, {
    method: 'PUT',
    body: payload,
  })
  return data ? normalizeReminder(data) : null
}

export async function deleteReminder(travelPlanId, reminderId) {
  return httpClient(`/travel-plans/${travelPlanId}/reminders/${reminderId}`, {
    method: 'DELETE',
  })
}
