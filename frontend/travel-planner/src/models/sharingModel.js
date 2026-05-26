export function createEmptyShareTokenForm() {
  return {
    accessType: '0',
    expiresAt: '',
  }
}

export function createShareTokenPayload(form) {
  return {
    accessType: Number(form.accessType),
    expiresAt: new Date(form.expiresAt).toISOString(),
  }
}

export function normalizeShareToken(token) {
  return {
    id: token.id ?? '',
    travelPlanId: token.travelPlanId ?? '',
    token: token.token ?? '',
    accessType: Number(token.accessType ?? 0),
    expiresAt: token.expiresAt ?? '',
    createdAt: token.createdAt ?? '',
  }
}

export function normalizeShareTokens(tokens) {
  return tokens.map(normalizeShareToken)
}
