import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ArrowLeft, Calendar, Clock, Stethoscope } from 'lucide-react';
import { format, addDays, parseISO } from 'date-fns';
import { api } from '../lib/api';
import type { Doctor, Slot } from '../lib/types';
import { useAuth } from '../context/AuthContext';
import Card from '../components/Card';
import Button from '../components/Button';
import Modal from '../components/Modal';
import LoadingSpinner from '../components/LoadingSpinner';
import './DoctorDetail.css';

export default function DoctorDetail() {
    const { doctorId } = useParams<{ doctorId: string }>();
    const navigate = useNavigate();
    const { patient } = useAuth(); // <--- Get logged in patient

    const [doctor, setDoctor] = useState<Doctor | null>(null);
    const [slots, setSlots] = useState<Slot[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [selectedDate, setSelectedDate] = useState<string>(format(new Date(), 'yyyy-MM-dd'));
    const [selectedSlot, setSelectedSlot] = useState<Slot | null>(null);
    const [showBookingModal, setShowBookingModal] = useState(false);
    const [patientId, setPatientId] = useState('');
    const [booking, setBooking] = useState(false);

    // Auto-fill patient ID if logged in
    useEffect(() => {
        if (patient) {
            setPatientId(patient.id);
        }
    }, [patient]);

    useEffect(() => {
        if (doctorId) {
            loadDoctorAndSlots();
        }
    }, [doctorId, selectedDate]);

    const loadDoctorAndSlots = async () => {
        if (!doctorId) return;

        try {
            setLoading(true);
            const [doctorData, slotsData] = await Promise.all([
                api.getDoctors().then(doctors => doctors.find(d => d.id === doctorId)),
                api.getAvailableSlots(doctorId, selectedDate),
            ]);

            setDoctor(doctorData || null);
            setSlots(slotsData);
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Failed to load data');
        } finally {
            setLoading(false);
        }
    };

    const handleBookSlot = async () => {
        // Use either logged-in ID or manually entered one
        const finalPatientId = patient ? patient.id : patientId;

        if (!selectedSlot || !doctorId || !finalPatientId.trim()) return;

        try {
            setBooking(true);
            await api.bookSlot(doctorId, selectedSlot.id, finalPatientId.trim());
            setShowBookingModal(false);
            setSelectedSlot(null);
            // Don't clear patientId if we are using the logged-in user
            if (!patient) {
                setPatientId('');
            }
            await loadDoctorAndSlots();
            alert('Booking successful! 🎉');
        } catch (err) {
            alert(err instanceof Error ? err.message : 'Failed to book slot');
        } finally {
            setBooking(false);
        }
    };

    const getDateButtons = () => {
        const dates = [];
        for (let i = 0; i < 7; i++) {
            dates.push(format(addDays(new Date(), i), 'yyyy-MM-dd'));
        }
        return dates;
    };

    if (loading) {
        return <LoadingSpinner size="lg" text="Loading doctor details..." />;
    }

    if (error || !doctor) {
        return (
            <div className="error-container">
                <p className="error-message">{error || 'Doctor not found'}</p>
                <Button onClick={() => navigate('/')}>Back to Doctors</Button>
            </div>
        );
    }

    return (
        <div className="doctor-detail-page">
            <Button
                variant="ghost"
                onClick={() => navigate('/')}
                className="back-button"
            >
                <ArrowLeft size={20} />
                Back to Doctors
            </Button>

            <Card className="doctor-info-card">
                <div className="doctor-info-header">
                    <div className="doctor-avatar-large">
                        <Stethoscope size={48} />
                    </div>
                    <div className="doctor-info-text">
                        <h1>{doctor.fullName}</h1>
                        <p className="specialty-badge-large">{doctor.specialty}</p>
                    </div>
                </div>
            </Card>

            <div className="booking-section">
                <h2>
                    <Calendar size={24} />
                    Select Date
                </h2>
                <div className="date-selector">
                    {getDateButtons().map((date) => (
                        <button
                            key={date}
                            className={`date-button ${selectedDate === date ? 'active' : ''}`}
                            onClick={() => setSelectedDate(date)}
                        >
                            <span className="date-day">
                                {format(parseISO(date), 'EEE')}
                            </span>
                            <span className="date-number">
                                {format(parseISO(date), 'd')}
                            </span>
                            <span className="date-month">
                                {format(parseISO(date), 'MMM')}
                            </span>
                        </button>
                    ))}
                </div>

                <h2>
                    <Clock size={24} />
                    Available Time Slots
                </h2>

                {slots.length === 0 ? (
                    <Card className="no-slots">
                        <p>No available slots for this date</p>
                        <p className="hint">Try selecting a different date</p>
                    </Card>
                ) : (
                    <div className="slots-grid">
                        {slots.map((slot) => (
                            <button
                                key={slot.id}
                                className="slot-button"
                                onClick={() => {
                                    setSelectedSlot(slot);
                                    setShowBookingModal(true);
                                }}
                            >
                                <Clock size={16} />
                                {slot.startTime} - {slot.endTime}
                            </button>
                        ))}
                    </div>
                )}
            </div>

            <Modal
                isOpen={showBookingModal}
                onClose={() => {
                    setShowBookingModal(false);
                    setSelectedSlot(null);
                    setPatientId('');
                }}
                title="Book Appointment"
                footer={
                    <>
                        <Button
                            variant="secondary"
                            onClick={() => {
                                setShowBookingModal(false);
                                setSelectedSlot(null);
                                setPatientId('');
                            }}
                        >
                            Cancel
                        </Button>
                        <Button
                            onClick={handleBookSlot}
                            isLoading={booking}
                            disabled={!patientId.trim()}
                        >
                            Confirm Booking
                        </Button>
                    </>
                }
            >
                <div className="booking-modal-content">
                    <div className="booking-details">
                        <p><strong>Doctor:</strong> {doctor.fullName}</p>
                        <p><strong>Date:</strong> {format(parseISO(selectedDate), 'MMMM d, yyyy')}</p>
                        <p>
                            <strong>Time:</strong> {selectedSlot?.startTime} - {selectedSlot?.endTime}
                        </p>
                    </div>

                    <div className="form-group">
                        <label htmlFor="patientId">Patient ID</label>
                        {patient ? (
                            <div className="logged-in-message">
                                <p>Booking as <strong>{patient.fullName}</strong></p>
                                <p className="text-sm text-secondary">ID: {patient.id}</p>
                            </div>
                        ) : (
                            <>
                                <input
                                    id="patientId"
                                    type="text"
                                    placeholder="Enter your patient ID"
                                    value={patientId}
                                    onChange={(e) => setPatientId(e.target.value)}
                                    className="form-input"
                                />
                                <p className="form-hint">
                                    Enter your patient ID to confirm the booking
                                </p>
                            </>
                        )}
                    </div>
                </div>
            </Modal>
        </div>
    );
}
