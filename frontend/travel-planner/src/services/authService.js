import { httpClient } from '../api/httpClient'

export async function registerUser(payload) {
  return httpClient('/auth/register', {
    method: 'POST',
    body: payload,
  })
}

export async function loginUser(payload) {
  return httpClient('/auth/login', {
    method: 'POST',
    body: payload,
  })
}