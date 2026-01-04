import type {
    Doctor,
    Patient,
    Slot,
    Booking,
    DoctorAvailability,
    AvailabilityException,
    BookSlotRequest,
    SetAvailabilityRequest,
    CreateExceptionRequest,
} from './types';

const API_BASE = '/api';

// Error handling helper
async function handleResponse<T>(response: Response): Promise<T> {
    if (!response.ok) {
        const error = await response.json().catch(() => ({
            code: 'UnknownError',
            message: 'An unexpected error occurred',
        }));
        throw new Error(error.message || `HTTP ${response.status}: ${response.statusText}`);
    }

    // Handle empty responses
    const text = await response.text();
    return text ? JSON.parse(text) : ({} as T);
}

// Public API - Patient Flow
export const api = {
    // Get all active doctors
    getDoctors: async (): Promise<Doctor[]> => {
        const response = await fetch(`${API_BASE}/doctors`);
        return handleResponse<Doctor[]>(response);
    },

    // Get available slots for a doctor
    getAvailableSlots: async (doctorId: string, date?: string): Promise<Slot[]> => {
        const url = date
            ? `${API_BASE}/doctors/${doctorId}/slots?date=${date}`
            : `${API_BASE}/doctors/${doctorId}/slots`;
        const response = await fetch(url);
        return handleResponse<Slot[]>(response);
    },

    // Book a slot
    bookSlot: async (doctorId: string, slotId: string, patientId: string): Promise<void> => {
        const response = await fetch(`${API_BASE}/doctors/${doctorId}/slots/${slotId}/book`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ patientId } as BookSlotRequest),
        });
        return handleResponse<void>(response);
    },

    // Get patient bookings
    getPatientBookings: async (patientId: string): Promise<Booking[]> => {
        const response = await fetch(`${API_BASE}/patients/${patientId}/bookings`);
        return handleResponse<Booking[]>(response);
    },

    // Cancel booking
    cancelBooking: async (doctorId: string, slotId: string): Promise<void> => {
        const response = await fetch(`${API_BASE}/doctors/${doctorId}/slots/${slotId}/cancel`, {
            method: 'POST',
        });
        return handleResponse<void>(response);
    },

    // Block slot
    blockSlot: async (doctorId: string, slotId: string): Promise<void> => {
        const response = await fetch(`${API_BASE}/doctors/${doctorId}/slots/${slotId}/block`, {
            method: 'POST',
        });
        return handleResponse<void>(response);
    },
};

// Admin API
export const adminApi = {
    // Get all doctors (including inactive)
    getDoctors: async (): Promise<Doctor[]> => {
        const response = await fetch(`${API_BASE}/admin/doctors`);
        return handleResponse<Doctor[]>(response);
    },

    // Get all patients
    getPatients: async (): Promise<Patient[]> => {
        const response = await fetch(`${API_BASE}/admin/patients`);
        return handleResponse<Patient[]>(response);
    },

    // Get doctor availability
    getDoctorAvailability: async (doctorId: string): Promise<DoctorAvailability> => {
        const response = await fetch(`${API_BASE}/admin/doctors/${doctorId}/availability`);
        return handleResponse<DoctorAvailability>(response);
    },

    // Set doctor availability
    setDoctorAvailability: async (
        doctorId: string,
        availability: SetAvailabilityRequest
    ): Promise<void> => {
        const response = await fetch(`${API_BASE}/admin/doctors/${doctorId}/availability`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(availability),
        });
        return handleResponse<void>(response);
    },

    // Get availability exceptions
    getAvailabilityExceptions: async (
        doctorId: string,
        from: string,
        to: string
    ): Promise<AvailabilityException[]> => {
        const response = await fetch(
            `${API_BASE}/admin/doctors/${doctorId}/availability-exceptions?from=${from}&to=${to}`
        );
        return handleResponse<AvailabilityException[]>(response);
    },

    // Create availability exception
    createAvailabilityException: async (
        doctorId: string,
        exception: CreateExceptionRequest
    ): Promise<void> => {
        const response = await fetch(
            `${API_BASE}/admin/doctors/${doctorId}/availability-exceptions`,
            {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(exception),
            }
        );
        return handleResponse<void>(response);
    },
};
