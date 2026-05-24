import { Navigate, Route, Routes } from 'react-router-dom'
import ProtectedRoute from '../components/ProtectedRoute'
import LoginPage from '../pages/LoginPage'
import RegisterPage from '../pages/RegisterPage'
import TravelPlansPage from '../pages/TravelPlansPage'

function AppRouter() {
  return (
    <Routes>
      <Route path="/" element={<Navigate to="/travel-plans" replace />} />
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />

      <Route element={<ProtectedRoute />}>
        <Route path="/travel-plans" element={<TravelPlansPage />} />
      </Route>
    </Routes>
  )
}

export default AppRouter