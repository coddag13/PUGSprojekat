import { httpClient } from '../api/httpClient'
import { normalizeShareToken, normalizeShareTokens } from '../models'

export async function getShareTokens(travelPlanId) {
  const data = await httpClient(`/travel-plans/${travelPlanId}/share-tokens`)
  return normalizeShareTokens(data)
}

export async function createShareToken(travelPlanId, payload) {
  const data = await httpClient(`/travel-plans/${travelPlanId}/share-tokens`, {
    method: 'POST',
    body: payload,
  })
  return normalizeShareToken(data)
}

export async function deleteShareToken(travelPlanId, tokenId) {
  return httpClient(`/travel-plans/${travelPlanId}/share-tokens/${tokenId}`, {
    method: 'DELETE',
  })
}
