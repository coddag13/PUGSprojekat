export function createEmptyLoginForm() {
  return {
    email: '',
    password: '',
  }
}

export function createEmptyRegisterForm() {
  return {
    firstName: '',
    lastName: '',
    email: '',
    password: '',
  }
}

export function createSessionModel(response) {
  return {
    token: response.token ?? '',
    user: {
      email: response.email ?? '',
      firstName: response.firstName ?? '',
      lastName: response.lastName ?? '',
      role: response.role ?? 'User',
    },
  }
}

export function normalizeStoredSession(session) {
  if (!session) {
    return null
  }

  return {
    token: session.token ?? '',
    user: {
      email: session.user?.email ?? '',
      firstName: session.user?.firstName ?? '',
      lastName: session.user?.lastName ?? '',
      role: session.user?.role ?? 'User',
    },
  }
}
