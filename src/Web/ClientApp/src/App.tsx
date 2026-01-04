import { BrowserRouter, Routes, Route, Link, useLocation } from 'react-router-dom';
import { Calendar, Users, Settings } from 'lucide-react';
import DoctorList from './pages/DoctorList';
import DoctorDetail from './pages/DoctorDetail';
import MyBookings from './pages/MyBookings';
import AdminLayout from './pages/admin/AdminLayout';
import AdminDoctors from './pages/admin/AdminDoctors';
import AdminPatients from './pages/admin/AdminPatients';
import { AuthProvider } from './context/AuthContext';
import './App.css';

function Navigation() {
  const location = useLocation();
  const isAdminRoute = location.pathname.startsWith('/admin');

  if (isAdminRoute) return null;

  return (
    <nav className="main-nav">
      <div className="nav-container">
        <Link to="/" className="nav-brand">
          <Calendar size={28} />
          <span className="gradient-text">HMS</span>
        </Link>

        <div className="nav-links">
          <Link to="/" className={location.pathname === '/' ? 'nav-link active' : 'nav-link'}>
            <Users size={18} />
            <span>Doctors</span>
          </Link>
          <Link to="/my-bookings" className={location.pathname === '/my-bookings' ? 'nav-link active' : 'nav-link'}>
            <Calendar size={18} />
            <span>My Bookings</span>
          </Link>
          <Link to="/admin/doctors" className="nav-link nav-link-admin">
            <Settings size={18} />
            <span>Admin</span>
          </Link>
        </div>
      </div>
    </nav>
  );
}

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <div className="app">
          <Navigation />
          <Routes>
            {/* Patient Routes */}
            <Route path="/" element={<DoctorList />} />
            <Route path="/doctors/:doctorId" element={<DoctorDetail />} />
            <Route path="/my-bookings" element={<MyBookings />} />

            {/* Admin Routes */}
            <Route path="/admin" element={<AdminLayout />}>
              <Route path="doctors" element={<AdminDoctors />} />
              <Route path="patients" element={<AdminPatients />} />
            </Route>
          </Routes>
        </div>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
