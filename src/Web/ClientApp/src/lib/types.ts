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
  email: string;
  phoneNumber: string;
}

export interface Slot {
  id: string;
  doctorId: string;
  date: string; // ISO date string
  startTime: string; // HH:mm format
  endTime: string; // HH:mm format
  status: 'Available' | 'Booked' | 'Blocked';
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
