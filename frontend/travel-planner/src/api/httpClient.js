const API_BASE_URL = import.meta.env.VITE_API_BASE_URL

function buildHeaders(token, hasBody) {
  const headers = {
    Accept: 'application/json',
  }

  if (hasBody) {
    headers['Content-Type'] = 'application/json'
  }

  if (token) {
    headers.Authorization = `Bearer ${token}`
  }

  return headers
}

export async function httpClient(path, options = {}) {
  const token = localStorage.getItem('travel_planner_token')
  const hasBody = options.body !== undefined

  const response = await fetch(`${API_BASE_URL}${path}`, {
    method: options.method ?? 'GET',
    headers: buildHeaders(token, hasBody),
    body: hasBody ? JSON.stringify(options.body) : undefined,
  })

  const text = await response.text()
  let data = null

  try {
    data = text ? JSON.parse(text) : null
  } catch {
    data = text
  }

  if (!response.ok) {
    const message =
      typeof data === 'string'
        ? data
        : data?.error || data?.title || 'Request failed.'

    throw new Error(message)
  }

  return data
}