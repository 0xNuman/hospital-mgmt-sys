import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Search, Stethoscope } from 'lucide-react';
import { api } from '../lib/api';
import type { Doctor } from '../lib/types';
import Card from '../components/Card';
import LoadingSpinner from '../components/LoadingSpinner';
import './DoctorList.css';

export default function DoctorList() {
    const navigate = useNavigate();
    const [doctors, setDoctors] = useState<Doctor[]>([]);
    const [filteredDoctors, setFilteredDoctors] = useState<Doctor[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [searchQuery, setSearchQuery] = useState('');
    const [selectedSpecialty, setSelectedSpecialty] = useState<string>('all');

    useEffect(() => {
        loadDoctors();
    }, []);

    useEffect(() => {
        filterDoctors();
    }, [searchQuery, selectedSpecialty, doctors]);

    const loadDoctors = async () => {
        try {
            setLoading(true);
            const data = await api.getDoctors();
            setDoctors(data);
            setFilteredDoctors(data);
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Failed to load doctors');
        } finally {
            setLoading(false);
        }
    };

    const filterDoctors = () => {
        let filtered = doctors;

        if (searchQuery) {
            filtered = filtered.filter((doctor) =>
                doctor.fullName.toLowerCase().includes(searchQuery.toLowerCase())
            );
        }

        if (selectedSpecialty !== 'all') {
            filtered = filtered.filter((doctor) => doctor.specialty === selectedSpecialty);
        }

        setFilteredDoctors(filtered);
    };

    const specialties = Array.from(new Set(doctors.map((d) => d.specialty))).sort();

    if (loading) {
        return <LoadingSpinner size="lg" text="Loading doctors..." />;
    }

    if (error) {
        return (
            <div className="error-container">
                <p className="error-message">{error}</p>
            </div>
        );
    }

    return (
        <div className="doctor-list-page">
            <div className="page-header">
                <h1 className="gradient-text">Find Your Doctor</h1>
                <p>Browse our expert medical professionals and book an appointment</p>
            </div>

            <div className="filters-section">
                <div className="search-box">
                    <Search size={20} />
                    <input
                        type="text"
                        placeholder="Search by doctor name..."
                        value={searchQuery}
                        onChange={(e) => setSearchQuery(e.target.value)}
                    />
                </div>

                <div className="specialty-filter">
                    <label htmlFor="specialty">Specialty:</label>
                    <select
                        id="specialty"
                        value={selectedSpecialty}
                        onChange={(e) => setSelectedSpecialty(e.target.value)}
                    >
                        <option value="all">All Specialties</option>
                        {specialties.map((specialty) => (
                            <option key={specialty} value={specialty}>
                                {specialty}
                            </option>
                        ))}
                    </select>
                </div>
            </div>

            {filteredDoctors.length === 0 ? (
                <div className="no-results">
                    <Stethoscope size={48} />
                    <p>No doctors found matching your criteria</p>
                </div>
            ) : (
                <div className="doctors-grid">
                    {filteredDoctors.map((doctor) => (
                        <Card
                            key={doctor.id}
                            hover
                            onClick={() => navigate(`/doctors/${doctor.id}`)}
                            className="doctor-card"
                        >
                            <div className="doctor-avatar">
                                <Stethoscope size={32} />
                            </div>
                            <h3>{doctor.fullName}</h3>
                            <p className="specialty-badge">{doctor.specialty}</p>
                            <div className="card-footer">
                                <span className="view-slots-link">View Available Slots →</span>
                            </div>
                        </Card>
                    ))}
                </div>
            )}
        </div>
    );
}
