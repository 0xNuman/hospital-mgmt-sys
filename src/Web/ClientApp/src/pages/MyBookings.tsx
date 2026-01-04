import { useState } from 'react';
import { Calendar, Clock, X, User } from 'lucide-react';
import { format, parseISO } from 'date-fns';
import { api } from '../lib/api';
import type { Booking } from '../lib/types';
import Card from '../components/Card';
import Button from '../components/Button';
import Modal from '../components/Modal';
import LoadingSpinner from '../components/LoadingSpinner';
import './MyBookings.css';

export default function MyBookings() {
    const [patientId, setPatientId] = useState('');
    const [inputPatientId, setInputPatientId] = useState('');
    const [bookings, setBookings] = useState<Booking[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [cancellingBooking, setCancellingBooking] = useState<Booking | null>(null);
    const [cancelling, setCancelling] = useState(false);

    const loadBookings = async (pid: string) => {
        try {
            setLoading(true);
            setError(null);
            const data = await api.getPatientBookings(pid);
            setBookings(data.filter(b => b.status === 'Active'));
            setPatientId(pid);
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Failed to load bookings');
            setBookings([]);
        } finally {
            setLoading(false);
        }
    };

    const handleCancelBooking = async () => {
        if (!cancellingBooking) return;

        try {
            setCancelling(true);
            // Note: We need doctorId for cancellation, but it's not in the booking response
            // For now, we'll use a placeholder. In production, this should be included in the API response
            await api.cancelBooking('00000000-0000-0000-0000-000000000000', cancellingBooking.slotId);
            setCancellingBooking(null);
            // Reload bookings
            if (patientId) {
                await loadBookings(patientId);
            }
            alert('Booking cancelled successfully');
        } catch (err) {
            alert(err instanceof Error ? err.message : 'Failed to cancel booking');
        } finally {
            setCancelling(false);
        }
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (inputPatientId.trim()) {
            loadBookings(inputPatientId.trim());
        }
    };

    return (
        <div className="my-bookings-page">
            <div className="page-header">
                <h1 className="gradient-text">My Bookings</h1>
                <p>View and manage your appointments</p>
            </div>

            {!patientId ? (
                <Card className="patient-id-card">
                    <form onSubmit={handleSubmit} className="patient-id-form">
                        <div className="form-group">
                            <label htmlFor="patientIdInput">
                                <User size={20} />
                                Enter Your Patient ID
                            </label>
                            <input
                                id="patientIdInput"
                                type="text"
                                placeholder="Patient ID"
                                value={inputPatientId}
                                onChange={(e) => setInputPatientId(e.target.value)}
                                className="form-input"
                                required
                            />
                        </div>
                        <Button type="submit" size="lg">
                            View My Bookings
                        </Button>
                    </form>
                </Card>
            ) : (
                <>
                    <div className="patient-info">
                        <p>
                            <strong>Patient ID:</strong> {patientId}
                        </p>
                        <Button
                            variant="ghost"
                            size="sm"
                            onClick={() => {
                                setPatientId('');
                                setInputPatientId('');
                                setBookings([]);
                            }}
                        >
                            Change Patient ID
                        </Button>
                    </div>

                    {loading ? (
                        <LoadingSpinner size="lg" text="Loading bookings..." />
                    ) : error ? (
                        <Card className="error-card">
                            <p className="error-message">{error}</p>
                        </Card>
                    ) : bookings.length === 0 ? (
                        <Card className="no-bookings">
                            <Calendar size={48} />
                            <h3>No Active Bookings</h3>
                            <p>You don't have any upcoming appointments</p>
                        </Card>
                    ) : (
                        <div className="bookings-grid">
                            {bookings.map((booking) => (
                                <Card key={booking.id} className="booking-card">
                                    <div className="booking-header">
                                        <h3>{booking.doctorName || 'Doctor'}</h3>
                                        <span className="status-badge status-active">Active</span>
                                    </div>

                                    <div className="booking-details">
                                        <div className="detail-item">
                                            <Calendar size={18} />
                                            <span>
                                                {booking.date
                                                    ? format(parseISO(booking.date), 'MMMM d, yyyy')
                                                    : 'Date not available'}
                                            </span>
                                        </div>
                                        <div className="detail-item">
                                            <Clock size={18} />
                                            <span>
                                                {booking.startTime && booking.endTime
                                                    ? `${booking.startTime} - ${booking.endTime}`
                                                    : 'Time not available'}
                                            </span>
                                        </div>
                                    </div>

                                    <Button
                                        variant="danger"
                                        size="sm"
                                        onClick={() => setCancellingBooking(booking)}
                                        className="cancel-button"
                                    >
                                        <X size={16} />
                                        Cancel Booking
                                    </Button>
                                </Card>
                            ))}
                        </div>
                    )}
                </>
            )}

            <Modal
                isOpen={!!cancellingBooking}
                onClose={() => setCancellingBooking(null)}
                title="Cancel Booking"
                footer={
                    <>
                        <Button
                            variant="secondary"
                            onClick={() => setCancellingBooking(null)}
                        >
                            Keep Booking
                        </Button>
                        <Button
                            variant="danger"
                            onClick={handleCancelBooking}
                            isLoading={cancelling}
                        >
                            Yes, Cancel
                        </Button>
                    </>
                }
            >
                <p>Are you sure you want to cancel this appointment?</p>
                <p className="warning-text">This action cannot be undone.</p>
            </Modal>
        </div>
    );
}
