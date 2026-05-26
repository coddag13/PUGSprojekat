import { createContext, useContext, useEffect, useState } from 'react'
import { createSessionModel, normalizeStoredSession } from '../models'
import { loginUser, registerUser } from '../services/authService'

const AuthContext = createContext(null)

const TOKEN_KEY = 'travel_planner_token'
const USER_KEY = 'travel_planner_user'

export function AuthProvider({ children }) {
  const [token, setToken] = useState(null)
  const [user, setUser] = useState(null)

  useEffect(() => {
    const savedToken = localStorage.getItem(TOKEN_KEY)
    const savedUser = localStorage.getItem(USER_KEY)

    if (savedToken) {
      setToken(savedToken)
    }

    if (savedUser) {
      const session = normalizeStoredSession({
        token: savedToken,
        user: JSON.parse(savedUser),
      })

      setUser(session?.user ?? null)
    }
  }, [])

  const persistSession = (session) => {
    localStorage.setItem(TOKEN_KEY, session.token)
    localStorage.setItem(USER_KEY, JSON.stringify(session.user))
    setToken(session.token)
    setUser(session.user)
  }

  const register = async (payload) => {
    const response = await registerUser(payload)
    const session = createSessionModel(response)
    persistSession(session)
    return session
  }

  const login = async (payload) => {
    const response = await loginUser(payload)
    const session = createSessionModel(response)
    persistSession(session)
    return session
  }

  const logout = () => {
    localStorage.removeItem(TOKEN_KEY)
    localStorage.removeItem(USER_KEY)
    setToken(null)
    setUser(null)
  }

  return (
    <AuthContext.Provider
      value={{
        token,
        user,
        isAuthenticated: Boolean(token),
        register,
        login,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const context = useContext(AuthContext)

  if (!context) {
    throw new Error('useAuth must be used within AuthProvider.')
  }

  return context
}
