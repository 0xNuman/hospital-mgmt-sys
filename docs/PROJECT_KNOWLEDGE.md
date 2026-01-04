# Hospital Management System - Project Knowledge Base

> **Last Updated**: 2026-01-04  
> **Project Location**: `/Users/nauman/wrk/tmp/hms`  
> **Workspace**: `0xNuman/health-mgmt-sys`

## 📋 Project Overview

### What is This Application?

The **Hospital Management System (HMS)** is a full-stack appointment booking and scheduling system for hospitals/clinics. It enables:
- **Patients** to browse doctors, view available time slots, and book appointments
- **Administrators** to manage doctors, configure availability schedules, and oversee the system

### Technology Stack

**Backend (.NET 10)**
- ASP.NET Core Minimal APIs
- Entity Framework Core 10
- SQLite databases (separate DBs for bounded contexts)
- Clean Architecture with Domain-Driven Design
- Scalar for API documentation

**Frontend (React + TypeScript)**
- React 19 with TypeScript
- Vite for build tooling
- React Router for routing
- date-fns for date manipulation
- lucide-react for icons

---

## 🏗️ Architecture & Design Patterns

### Backend Architecture

The backend follows **Clean Architecture** and **Domain-Driven Design** with two bounded contexts:

#### 1. ReferenceData Context
**Purpose**: Manages master data (doctors, patients, availability configurations)

**Layers**:
- `Domain/` - Core entities: `Doctor`, `Patient`, `DoctorAvailability`
- `Application/` - Use cases and ports (interfaces)
- `Infrastructure/` - EF Core implementation, database: `reference-data.dev.db`

**Key Entities**:
```csharp
Doctor {
  Guid Id
  string FullName
  string Specialty
  bool IsActive
}

DoctorAvailability {
  Guid DoctorId
  IReadOnlySet<DayOfWeek> WorkingDays
  TimeOnly DailyStartTime
  TimeOnly DailyEndTime
  int SlotDurationMinutes
  int RollingWindowDays
  bool IsActive
}
```

#### 2. Scheduling Context
**Purpose**: Handles transactional data (slots, bookings)

**Layers**:
- `Domain/` - Core entities: `Slot`, `Booking`
- `Application/` - Use cases for booking, cancellation, slot generation
- `Infrastructure/` - EF Core implementation, database: `scheduling.dev.db`

**Key Entities**:
```csharp
Slot {
  Guid Id
  Guid DoctorId
  DateOnly Date
  TimeOnly StartTime
  TimeOnly EndTime
  SlotStatus Status // Available, Booked, Blocked
}

Booking {
  Guid Id
  Guid SlotId
  Guid PatientId
  BookingStatus Status // Active, Cancelled, Invalidated
}
```

#### Web Layer
**Location**: `src/Web/`

**Key Files**:
- `Program.cs` - Application startup, DI configuration
- `Endpoints/` - Minimal API endpoint definitions
- `Dtos/` - Data transfer objects
- `Workers/SlotGenerationWorker.cs` - Background service that auto-generates slots every 12 hours

**API Base URL**: `http://localhost:5128/api`

---

## 🔌 API Endpoints Reference

### Public Endpoints (Patient Flow)

```
GET    /api/doctors
       → Get all active doctors

GET    /api/doctors/{doctorId}/slots?date={yyyy-MM-dd}
       → Get available slots for a doctor (optional date filter)

POST   /api/doctors/{doctorId}/slots/{slotId}/book
       Body: { "patientId": "guid" }
       → Book an appointment

GET    /api/patients/{patientId}/bookings
       → Get patient's active bookings

POST   /api/doctors/{doctorId}/slots/{slotId}/cancel
       → Cancel a booking

POST   /api/doctors/{doctorId}/slots/{slotId}/block
       → Block a slot (admin)
```

### Admin Endpoints

```
GET    /api/admin/doctors
       → Get all doctors (including inactive)

GET    /api/admin/patients
       → Get all patients

PUT    /api/admin/doctors/{doctorId}/availability
       Body: {
         "workingDays": "Mon,Tue,Wed,Thu,Fri",
         "dailyStartTime": "09:00",
         "dailyEndTime": "17:00",
         "slotDurationMinutes": 30,
         "rollingWindowDays": 14,
         "isActive": true
       }
       → Configure doctor availability

GET    /api/admin/doctors/{doctorId}/availability
       → Get doctor availability configuration

POST   /api/admin/doctors/{doctorId}/availability-exceptions
       Body: {
         "date": "2026-02-10",
         "type": "FullDayBlock" | "PartialDayBlock",
         "startTime": "10:00", // optional for partial
         "endTime": "13:00",   // optional for partial
         "reason": "Emergency leave"
       }
       → Create availability exception

GET    /api/admin/doctors/{doctorId}/availability-exceptions?from={date}&to={date}
       → Get availability exceptions in date range
```

---

## 🎨 Frontend Architecture

### Directory Structure

```
src/Web/ClientApp/src/
├── lib/
│   ├── api.ts          # API client with typed methods
│   └── types.ts        # TypeScript interfaces matching backend DTOs
├── components/
│   ├── Button.tsx      # Reusable button (primary, secondary, danger, ghost)
│   ├── Card.tsx        # Glassmorphic card component
│   ├── Modal.tsx       # Modal dialog
│   └── LoadingSpinner.tsx
├── pages/
│   ├── DoctorList.tsx      # Browse doctors with search/filter
│   ├── DoctorDetail.tsx    # View slots & book appointments
│   ├── MyBookings.tsx      # Manage patient bookings
│   └── admin/
│       ├── AdminLayout.tsx    # Admin sidebar navigation
│       ├── AdminDoctors.tsx   # Doctor management & availability config
│       └── AdminPatients.tsx  # Patient listing
├── App.tsx             # Main app with React Router
├── App.css             # Navigation styles
└── index.css           # Design system & global styles
```

### Design System

**Location**: `src/Web/ClientApp/src/index.css`

**Color Palette**:
```css
--color-bg-primary: #0a0e1a       /* Dark background */
--color-bg-secondary: #131827     /* Card backgrounds */
--color-bg-glass: rgba(26,32,53,0.7)  /* Glassmorphism */

--color-accent-primary: #6366f1   /* Indigo */
--color-accent-secondary: #8b5cf6 /* Purple */
--color-accent-gradient: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%)

--color-success: #10b981
--color-danger: #ef4444
--color-text-primary: #f8fafc
--color-text-secondary: #cbd5e1
```

**Key Features**:
- Dark theme with vibrant gradients
- Glassmorphism effects (backdrop-filter: blur)
- Smooth animations (fadeIn, slideIn, pulse, spin)
- Responsive breakpoints (768px, 1024px)
- Inter font family

### Routing Structure

```typescript
/ (DoctorList)
  → Browse all active doctors
  → Search by name, filter by specialty

/doctors/:doctorId (DoctorDetail)
  → View doctor profile
  → Select date (next 7 days)
  → View available slots
  → Book appointment

/my-bookings (MyBookings)
  → Enter patient ID
  → View active bookings
  → Cancel bookings

/admin (AdminLayout)
  /admin/doctors (AdminDoctors)
    → View all doctors
    → Configure availability
  
  /admin/patients (AdminPatients)
    → View all patients
    → Search by name/email
```

### API Client Pattern

**Location**: `src/Web/ClientApp/src/lib/api.ts`

All API calls are centralized with typed methods:

```typescript
// Public API
api.getDoctors(): Promise<Doctor[]>
api.getAvailableSlots(doctorId, date?): Promise<Slot[]>
api.bookSlot(doctorId, slotId, patientId): Promise<void>
api.getPatientBookings(patientId): Promise<Booking[]>
api.cancelBooking(doctorId, slotId): Promise<void>

// Admin API
adminApi.getDoctors(): Promise<Doctor[]>
adminApi.getPatients(): Promise<Patient[]>
adminApi.getDoctorAvailability(doctorId): Promise<DoctorAvailability>
adminApi.setDoctorAvailability(doctorId, config): Promise<void>
```

**Error Handling**: Centralized in `handleResponse<T>()` helper
- Throws errors with meaningful messages
- Handles empty responses (returns `{} as T`)

---

## 🔄 Key Business Logic

### Slot Generation (Background Worker)

**File**: `src/Web/Workers/SlotGenerationWorker.cs`

**How It Works**:
1. Runs every 12 hours automatically
2. Reads doctor availability configurations
3. Generates slots for the rolling window period
4. Respects working days, daily hours, and slot duration
5. Skips availability exceptions (full-day or partial-day blocks)

**Example**:
- Doctor works Mon-Fri, 9:00-17:00
- Slot duration: 30 minutes
- Rolling window: 14 days
- Result: ~80 slots generated per doctor (8 hours × 2 slots/hour × 5 days × 2 weeks)

### Booking Flow

1. **Patient selects slot** → Frontend calls `POST /api/doctors/{doctorId}/slots/{slotId}/book`
2. **Backend validates**:
   - Slot exists and is Available
   - Slot is not already booked
3. **Backend creates Booking** with status Active
4. **Slot status changes** to Booked
5. **Frontend refreshes** slot list

### Cancellation Flow

1. **Patient cancels** → Frontend calls `POST /api/doctors/{doctorId}/slots/{slotId}/cancel`
2. **Backend finds booking** by slotId
3. **Booking status changes** to Cancelled
4. **Slot status changes** back to Available

---

## 🛠️ Development Workflow

### Running the Application

**Backend**:
```bash
cd src/Web
dotnet run
# Runs on http://localhost:5128
# Swagger/Scalar docs at /scalar/v1
```

**Frontend**:
```bash
cd src/Web/ClientApp
npm install  # First time only
npm run dev
# Runs on http://localhost:5173 or 5174
# Proxies /api to http://localhost:5128
```

**Build Frontend**:
```bash
cd src/Web/ClientApp
npm run build
# Output: dist/ folder
# Served by backend in production
```

### Database Seeding

**Location**: `src/ReferenceData/Infrastructure/ReferenceDataSeeder.cs`

**What Gets Seeded** (in Development only):
- Sample doctors with various specialties
- Sample patients
- Default availability configurations

**Databases**:
- `src/Web/reference-data.dev.db` - Doctors, patients, availability
- `src/Web/scheduling.dev.db` - Slots, bookings

---

## 📝 Implementation Session Summary

### What Was Built (2026-01-04)

**Phase 1: Planning & Infrastructure**
- Analyzed backend API structure and endpoints
- Created implementation plan with patient and admin flows
- Set up design system with dark theme and glassmorphism
- Created API client utilities with TypeScript types
- Built shared components (Button, Card, Modal, LoadingSpinner)

**Phase 2: Patient Booking Flow**
- `DoctorList.tsx` - Doctor browsing with search and specialty filter
- `DoctorDetail.tsx` - Date picker (next 7 days) and slot selection
- Booking modal with patient ID input
- `MyBookings.tsx` - View and cancel bookings

**Phase 3: Admin Panel**
- `AdminLayout.tsx` - Sidebar navigation
- `AdminDoctors.tsx` - Doctor management with availability configuration modal
  - Working days selector (Mon-Sun checkboxes)
  - Time pickers for daily hours
  - Slot duration and rolling window inputs
- `AdminPatients.tsx` - Patient listing with search

**Phase 4: Integration & Testing**
- React Router setup with all routes
- Navigation bar with patient/admin switching
- Build verification (TypeScript compilation successful)
- Live demo recording showing all workflows

### Design Decisions Made

1. **No Authentication**: Backend doesn't have auth, so patient ID is manually entered
2. **Two Separate Databases**: Maintains bounded context separation
3. **Glassmorphism Design**: Modern, premium aesthetic with dark theme
4. **Type-Safe API Client**: All endpoints typed with TypeScript interfaces
5. **Responsive-First**: Mobile breakpoints at 768px and 1024px

### Known Limitations

**Not Implemented** (APIs exist but no UI):
- Availability exceptions management UI
- Slot blocking/unblocking interface
- Patient profile editing
- Booking history (past/cancelled)
- Authentication/authorization

**Technical Debt**:
- Cancellation endpoint needs doctorId but booking response doesn't include it (workaround: using placeholder GUID)
- No error boundary for React component errors
- No loading states for route transitions

---

## 🎯 Quick Reference

### Common Tasks

**Add a new doctor** (via seeder):
```csharp
// src/ReferenceData/Infrastructure/ReferenceDataSeeder.cs
new Doctor { 
  Id = Guid.NewGuid(), 
  FullName = "Dr. Name", 
  Specialty = "Specialty", 
  IsActive = true 
}
```

**Configure doctor availability** (via API or UI):
```json
{
  "workingDays": "Mon,Wed,Fri",
  "dailyStartTime": "10:00",
  "dailyEndTime": "18:00",
  "slotDurationMinutes": 45,
  "rollingWindowDays": 30,
  "isActive": true
}
```

**Test booking flow**:
1. Start backend: `cd src/Web && dotnet run`
2. Start frontend: `cd src/Web/ClientApp && npm run dev`
3. Open `http://localhost:5173`
4. Click doctor → Select date → Click slot → Enter patient ID → Book

### File Locations Cheat Sheet

| What | Where |
|------|-------|
| Backend startup | `src/Web/Program.cs` |
| API endpoints | `src/Web/Endpoints/` |
| Domain models | `src/{Context}/Domain/` |
| Use cases | `src/{Context}/Application/UseCases/` |
| Frontend entry | `src/Web/ClientApp/src/main.tsx` |
| Routing | `src/Web/ClientApp/src/App.tsx` |
| API client | `src/Web/ClientApp/src/lib/api.ts` |
| Design system | `src/Web/ClientApp/src/index.css` |
| Components | `src/Web/ClientApp/src/components/` |
| Pages | `src/Web/ClientApp/src/pages/` |

### Port Configuration

- **Backend**: `5128` (configured in `launchSettings.json`)
- **Frontend Dev**: `5173` or `5174` (Vite auto-selects)
- **API Proxy**: Frontend proxies `/api` to `http://localhost:5128`

---

## 💡 Future Enhancement Ideas

### High Priority
1. **Authentication & Authorization**
   - JWT-based auth for patients and admins
   - Role-based access control
   - Secure patient data access

2. **Availability Exceptions UI**
   - Calendar view for blocking dates
   - Full-day and partial-day block creation
   - Visual indicators on date picker

3. **Enhanced Booking Management**
   - Booking history (past appointments)
   - Rescheduling capability
   - Email/SMS notifications

### Medium Priority
4. **Admin Enhancements**
   - Slot blocking/unblocking interface
   - Analytics dashboard (bookings per doctor, cancellation rates)
   - Patient profile management

5. **Patient Experience**
   - Doctor profiles with photos and bios
   - Reviews and ratings
   - Favorite doctors
   - Appointment reminders

### Low Priority
6. **Technical Improvements**
   - Real-time updates with SignalR
   - Optimistic UI updates
   - Offline support with service workers
   - Unit and integration tests

---

## 🐛 Troubleshooting

### Frontend won't build
- **Error**: Type import issues
- **Fix**: Ensure all React type imports use `import type { ... }`

### Slots not showing
- **Cause**: Doctor availability not configured
- **Fix**: Go to Admin → Doctors → Configure Availability

### Booking fails
- **Cause**: Slot already booked or blocked
- **Fix**: Refresh page to see current slot status

### Backend database errors
- **Cause**: Database not created
- **Fix**: Delete `.db` files and restart backend (auto-creates in dev)

---

## 📚 Learning Resources

### Clean Architecture
- Bounded contexts separate concerns (ReferenceData vs Scheduling)
- Domain layer has no dependencies
- Application layer defines use cases
- Infrastructure implements ports

### React Best Practices Used
- Functional components with hooks
- Custom hooks for reusable logic
- Centralized API client
- Type-safe props with TypeScript
- CSS modules for component styles

### Design Patterns Applied
- **Repository Pattern**: Data access abstraction
- **Use Case Pattern**: Business logic encapsulation
- **Factory Pattern**: Entity creation
- **Observer Pattern**: Background worker for slot generation

---

## ✅ Verification Checklist

When making changes, verify:

- [ ] Backend compiles: `dotnet build`
- [ ] Frontend compiles: `npm run build`
- [ ] No TypeScript errors: `tsc -b`
- [ ] API endpoints return expected data
- [ ] UI components render correctly
- [ ] Responsive design works on mobile
- [ ] Animations are smooth
- [ ] Error handling shows user-friendly messages

---

**End of Knowledge Base**

*This document should be updated whenever significant changes are made to the project architecture, API endpoints, or frontend structure.*
