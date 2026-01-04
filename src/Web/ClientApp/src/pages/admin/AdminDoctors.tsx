import { useState, useEffect } from 'react';
import { UserCog, Settings } from 'lucide-react';
import { adminApi } from '../../lib/api';
import type { Doctor } from '../../lib/types';
import Card from '../../components/Card';
import Button from '../../components/Button';
import Modal from '../../components/Modal';
import LoadingSpinner from '../../components/LoadingSpinner';
import './AdminDoctors.css';

export default function AdminDoctors() {
    const [doctors, setDoctors] = useState<Doctor[]>([]);
    const [loading, setLoading] = useState(true);
    const [selectedDoctor, setSelectedDoctor] = useState<Doctor | null>(null);

    const [showAvailabilityModal, setShowAvailabilityModal] = useState(false);
    const [loadingAvailability, setLoadingAvailability] = useState(false);
    const [savingAvailability, setSavingAvailability] = useState(false);

    const [formData, setFormData] = useState({
        workingDays: '',
        dailyStartTime: '09:00',
        dailyEndTime: '17:00',
        slotDurationMinutes: 30,
        rollingWindowDays: 14,
        isActive: true,
    });

    useEffect(() => {
        loadDoctors();
    }, []);

    const loadDoctors = async () => {
        try {
            setLoading(true);
            const data = await adminApi.getDoctors();
            setDoctors(data);
        } catch (err) {
            alert('Failed to load doctors');
        } finally {
            setLoading(false);
        }
    };

    const openAvailabilityModal = async (doctor: Doctor) => {
        setSelectedDoctor(doctor);
        setShowAvailabilityModal(true);
        setLoadingAvailability(true);

        try {
            const avail = await adminApi.getDoctorAvailability(doctor.id);
            // Load existing availability
            setFormData({
                workingDays: avail.workingDays,
                dailyStartTime: avail.dailyStartTime,
                dailyEndTime: avail.dailyEndTime,
                slotDurationMinutes: avail.slotDurationMinutes,
                rollingWindowDays: avail.rollingWindowDays,
                isActive: avail.isActive,
            });
        } catch (err) {
            // No availability set yet, use defaults
            setFormData({
                workingDays: 'Mon,Tue,Wed,Thu,Fri',
                dailyStartTime: '09:00',
                dailyEndTime: '17:00',
                slotDurationMinutes: 30,
                rollingWindowDays: 14,
                isActive: true,
            });
        } finally {
            setLoadingAvailability(false);
        }
    };

    const handleSaveAvailability = async () => {
        if (!selectedDoctor) return;

        try {
            setSavingAvailability(true);
            await adminApi.setDoctorAvailability(selectedDoctor.id, formData);
            setShowAvailabilityModal(false);
            alert('Availability saved successfully!');
        } catch (err) {
            alert(err instanceof Error ? err.message : 'Failed to save availability');
        } finally {
            setSavingAvailability(false);
        }
    };

    const toggleDay = (day: string) => {
        const days = formData.workingDays.split(',').filter(d => d);
        const index = days.indexOf(day);

        if (index > -1) {
            days.splice(index, 1);
        } else {
            days.push(day);
        }

        setFormData({ ...formData, workingDays: days.join(',') });
    };

    const isDaySelected = (day: string) => {
        return formData.workingDays.split(',').includes(day);
    };

    if (loading) {
        return <LoadingSpinner size="lg" text="Loading doctors..." />;
    }

    return (
        <div className="admin-doctors-page">
            <div className="page-header">
                <h1>Manage Doctors</h1>
                <p>Configure doctor availability and schedules</p>
            </div>

            <div className="doctors-grid">
                {doctors.map((doctor) => (
                    <Card key={doctor.id} className="doctor-admin-card">
                        <div className="doctor-card-header">
                            <div className="doctor-avatar-small">
                                <UserCog size={24} />
                            </div>
                            <div>
                                <h3>{doctor.fullName}</h3>
                                <p className="specialty">{doctor.specialty}</p>
                            </div>
                        </div>

                        <div className="doctor-status">
                            <span className={`status-badge ${doctor.isActive ? 'status-active' : 'status-inactive'}`}>
                                {doctor.isActive ? 'Active' : 'Inactive'}
                            </span>
                        </div>

                        <Button
                            variant="secondary"
                            size="sm"
                            onClick={() => openAvailabilityModal(doctor)}
                        >
                            <Settings size={16} />
                            Configure Availability
                        </Button>
                    </Card>
                ))}
            </div>

            <Modal
                isOpen={showAvailabilityModal}
                onClose={() => setShowAvailabilityModal(false)}
                title={`Configure Availability - ${selectedDoctor?.fullName}`}
                footer={
                    <>
                        <Button
                            variant="secondary"
                            onClick={() => setShowAvailabilityModal(false)}
                        >
                            Cancel
                        </Button>
                        <Button
                            onClick={handleSaveAvailability}
                            isLoading={savingAvailability}
                        >
                            Save Availability
                        </Button>
                    </>
                }
            >
                {loadingAvailability ? (
                    <LoadingSpinner text="Loading availability..." />
                ) : (
                    <div className="availability-form">
                        <div className="form-section">
                            <label>Working Days</label>
                            <div className="days-selector">
                                {['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'].map((day) => (
                                    <button
                                        key={day}
                                        type="button"
                                        className={`day-button ${isDaySelected(day) ? 'selected' : ''}`}
                                        onClick={() => toggleDay(day)}
                                    >
                                        {day}
                                    </button>
                                ))}
                            </div>
                        </div>

                        <div className="form-row">
                            <div className="form-group">
                                <label htmlFor="startTime">Start Time</label>
                                <input
                                    id="startTime"
                                    type="time"
                                    value={formData.dailyStartTime}
                                    onChange={(e) => setFormData({ ...formData, dailyStartTime: e.target.value })}
                                    className="form-input"
                                />
                            </div>

                            <div className="form-group">
                                <label htmlFor="endTime">End Time</label>
                                <input
                                    id="endTime"
                                    type="time"
                                    value={formData.dailyEndTime}
                                    onChange={(e) => setFormData({ ...formData, dailyEndTime: e.target.value })}
                                    className="form-input"
                                />
                            </div>
                        </div>

                        <div className="form-row">
                            <div className="form-group">
                                <label htmlFor="slotDuration">Slot Duration (minutes)</label>
                                <input
                                    id="slotDuration"
                                    type="number"
                                    min="5"
                                    max="120"
                                    step="5"
                                    value={formData.slotDurationMinutes}
                                    onChange={(e) => setFormData({ ...formData, slotDurationMinutes: parseInt(e.target.value) })}
                                    className="form-input"
                                />
                            </div>

                            <div className="form-group">
                                <label htmlFor="rollingWindow">Rolling Window (days)</label>
                                <input
                                    id="rollingWindow"
                                    type="number"
                                    min="1"
                                    max="90"
                                    value={formData.rollingWindowDays}
                                    onChange={(e) => setFormData({ ...formData, rollingWindowDays: parseInt(e.target.value) })}
                                    className="form-input"
                                />
                            </div>
                        </div>

                        <div className="form-group">
                            <label className="checkbox-label">
                                <input
                                    type="checkbox"
                                    checked={formData.isActive}
                                    onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
                                />
                                <span>Active</span>
                            </label>
                        </div>
                    </div>
                )}
            </Modal>
        </div>
    );
}
