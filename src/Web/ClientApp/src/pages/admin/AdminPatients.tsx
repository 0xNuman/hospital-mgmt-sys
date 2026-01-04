import { useState, useEffect } from 'react';
import { Users, Mail, Phone } from 'lucide-react';
import { adminApi } from '../../lib/api';
import type { Patient } from '../../lib/types';
import Card from '../../components/Card';
import LoadingSpinner from '../../components/LoadingSpinner';
import './AdminPatients.css';

export default function AdminPatients() {
    const [patients, setPatients] = useState<Patient[]>([]);
    const [loading, setLoading] = useState(true);
    const [searchQuery, setSearchQuery] = useState('');
    const [filteredPatients, setFilteredPatients] = useState<Patient[]>([]);

    useEffect(() => {
        loadPatients();
    }, []);

    useEffect(() => {
        filterPatients();
    }, [searchQuery, patients]);

    const loadPatients = async () => {
        try {
            setLoading(true);
            const data = await adminApi.getPatients();
            setPatients(data);
            setFilteredPatients(data);
        } catch (err) {
            alert('Failed to load patients');
        } finally {
            setLoading(false);
        }
    };

    const filterPatients = () => {
        if (!searchQuery) {
            setFilteredPatients(patients);
            return;
        }

        const filtered = patients.filter((patient) =>
            patient.fullName.toLowerCase().includes(searchQuery.toLowerCase()) ||
            (patient.email && patient.email.toLowerCase().includes(searchQuery.toLowerCase()))
        );
        setFilteredPatients(filtered);
    };

    if (loading) {
        return <LoadingSpinner size="lg" text="Loading patients..." />;
    }

    return (
        <div className="admin-patients-page">
            <div className="page-header">
                <h1>Manage Patients</h1>
                <p>View patient information and booking history</p>
            </div>

            <div className="search-section">
                <input
                    type="text"
                    placeholder="Search by name or email..."
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                    className="search-input"
                />
            </div>

            {filteredPatients.length === 0 ? (
                <Card className="no-results">
                    <Users size={48} />
                    <p>No patients found</p>
                </Card>
            ) : (
                <div className="patients-grid">
                    {filteredPatients.map((patient) => (
                        <Card key={patient.id} className="patient-card">
                            <div className="patient-header">
                                <div className="patient-avatar">
                                    <Users size={24} />
                                </div>
                                <h3>{patient.fullName}</h3>
                            </div>

                            <div className="patient-details">
                                <div className="detail-row">
                                    <Mail size={16} />
                                    <span>{patient.email}</span>
                                </div>
                                <div className="detail-row">
                                    <Phone size={16} />
                                    <span>{patient.phone}</span>
                                </div>
                            </div>
                        </Card>
                    ))}
                </div>
            )}
        </div>
    );
}
