import { httpClient } from '../api/httpClient'

export async function registerUser(payload) {
  return httpClient('/users', {
    method: 'POST',
    body: payload,
  })
}

export async function loginUser(payload) {
  return httpClient('/sessions', {
    method: 'POST',
    body: payload,
  })
}
