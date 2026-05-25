import { httpClient } from '../api/httpClient'

export async function getAdminUsers() {
  return httpClient('/admin/users')
}

export async function updateAdminUserRole(userId, payload) {
  return httpClient(`/admin/users/${userId}/role`, {
    method: 'PUT',
    body: payload,
  })
}

export async function deleteAdminUser(userId) {
  return httpClient(`/admin/users/${userId}`, {
    method: 'DELETE',
  })
}

export async function getAdminTravelPlans() {
  return httpClient('/admin/travel-plans')
}

export async function deleteAdminTravelPlan(planId) {
  return httpClient(`/admin/travel-plans/${planId}`, {
    method: 'DELETE',
  })
}
