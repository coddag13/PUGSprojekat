import { httpClient } from '../api/httpClient'

export async function getShareTokens(travelPlanId) {
  return httpClient(`/travel-plans/${travelPlanId}/share-tokens`)
}

export async function createShareToken(travelPlanId, payload) {
  return httpClient(`/travel-plans/${travelPlanId}/share-tokens`, {
    method: 'POST',
    body: payload,
  })
}

export async function deleteShareToken(travelPlanId, tokenId) {
  return httpClient(`/travel-plans/${travelPlanId}/share-tokens/${tokenId}`, {
    method: 'DELETE',
  })
}