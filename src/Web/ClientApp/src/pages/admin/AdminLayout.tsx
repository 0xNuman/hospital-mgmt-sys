import { NavLink, Outlet } from 'react-router-dom';
import { Users, UserCog, Calendar } from 'lucide-react';
import './AdminLayout.css';

export default function AdminLayout() {
    return (
        <div className="admin-layout">
            <aside className="admin-sidebar">
                <div className="admin-header">
                    <h2 className="gradient-text">Admin Panel</h2>
                </div>

                <nav className="admin-nav">
                    <NavLink to="/admin/doctors" className="nav-link">
                        <UserCog size={20} />
                        <span>Doctors</span>
                    </NavLink>
                    <NavLink to="/admin/patients" className="nav-link">
                        <Users size={20} />
                        <span>Patients</span>
                    </NavLink>
                    <NavLink to="/" className="nav-link nav-link-secondary">
                        <Calendar size={20} />
                        <span>Back to Patient View</span>
                    </NavLink>
                </nav>
            </aside>

            <main className="admin-content">
                <Outlet />
            </main>
        </div>
    );
}
