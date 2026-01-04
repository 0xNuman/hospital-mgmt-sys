// Type definitions matching backend DTOs

export interface Doctor {
  id: string;
  fullName: string;
  specialty: string;
  isActive: boolean;
}

export interface Patient {
  id: string;
  fullName: string;
  email: string | null;
  phone: string;
}

export interface Slot {
  id: string; // Mapped from slotId
  doctorId: string; // Not in response, but useful context
  date: string; // Not in response, but useful context
  startTime: string; // Mapped from start
  endTime: string; // Mapped from end
  status: 'Available' | 'Booked' | 'Blocked'; // Inferred
}

export interface BackendSlotDto {
  slotId: string;
  start: string;
  end: string;
}

export interface Booking {
  id: string;
  slotId: string;
  patientId: string;
  status: 'Active' | 'Cancelled' | 'Invalidated';
  doctorName?: string;
  date?: string;
  startTime?: string;
  endTime?: string;
}

export interface DoctorAvailability {
  workingDays: string; // e.g., "Mon,Tue,Wed,Thu,Fri"
  dailyStartTime: string; // HH:mm
  dailyEndTime: string; // HH:mm
  slotDurationMinutes: number;
  rollingWindowDays: number;
  isActive: boolean;
}

export interface AvailabilityException {
  id: string;
  doctorId: string;
  date: string;
  type: 'FullDayBlock' | 'PartialDayBlock';
  startTime?: string;
  endTime?: string;
  reason: string;
}

export interface ApiResult<T = void> {
  success: boolean;
  data?: T;
  error?: {
    code: string;
    message: string;
  };
}

export interface BookSlotRequest {
  patientId: string;
}

export interface SetAvailabilityRequest {
  workingDays: string;
  dailyStartTime: string;
  dailyEndTime: string;
  slotDurationMinutes: number;
  rollingWindowDays: number;
  isActive: boolean;
}

export interface CreateExceptionRequest {
  date: string;
  type: 'FullDayBlock' | 'PartialDayBlock';
  startTime?: string;
  endTime?: string;
  reason: string;
}
