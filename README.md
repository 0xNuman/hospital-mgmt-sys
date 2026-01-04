# Hospital Management System (HMS)

> A modern, full-stack appointment booking and scheduling system for hospitals and clinics

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-19-61DAFB?logo=react)](https://react.dev/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.9-3178C6?logo=typescript)](https://www.typescriptlang.org/)
[![Vite](https://img.shields.io/badge/Vite-7.2-646CFF?logo=vite)](https://vitejs.dev/)

![HMS Screenshot](docs/assets/screenshot.png)

## ✨ Features

### 👥 Patient Portal
- **Browse Doctors** - Search and filter by specialty
- **View Availability** - See available time slots for the next 7 days
- **Book Appointments** - Simple booking flow with instant confirmation
- **Manage Bookings** - View and cancel upcoming appointments

### 🔧 Admin Panel
- **Doctor Management** - View and manage doctor profiles
- **Availability Configuration** - Set working days, hours, and slot durations
- **Patient Management** - View patient information and contact details
- **Automated Slot Generation** - Background worker creates slots automatically

### 🎨 Design Highlights
- **Modern Dark Theme** with vibrant gradient accents
- **Glassmorphism Effects** for depth and visual appeal
- **Smooth Animations** throughout the interface
- **Fully Responsive** - Works on mobile, tablet, and desktop
- **Accessible** with keyboard navigation and ARIA labels

## 🏗️ Architecture

### Backend (.NET 10)
Built with **Clean Architecture** and **Domain-Driven Design**:

```
src/
├── ReferenceData/          # Bounded Context: Master Data
│   ├── Domain/            # Entities: Doctor, Patient, DoctorAvailability
│   ├── Application/       # Use Cases & Ports
│   └── Infrastructure/    # EF Core, Database
├── Scheduling/            # Bounded Context: Transactional Data
│   ├── Domain/           # Entities: Slot, Booking
│   ├── Application/      # Use Cases & Ports
│   └── Infrastructure/   # EF Core, Database
└── Web/                  # API & Frontend Host
    ├── Endpoints/        # Minimal API Endpoints
    ├── Workers/          # Background Services
    └── ClientApp/        # React Frontend
```

**Key Technologies**:
- ASP.NET Core Minimal APIs
- Entity Framework Core 10
- SQLite (separate databases per bounded context)
- Scalar for API documentation

### Frontend (React + TypeScript)
Modern React application with:

```
src/Web/ClientApp/src/
├── lib/
│   ├── api.ts           # Typed API client
│   └── types.ts         # TypeScript interfaces
├── components/          # Reusable components
│   ├── Button.tsx
│   ├── Card.tsx
│   ├── Modal.tsx
│   └── LoadingSpinner.tsx
├── pages/
│   ├── DoctorList.tsx      # Browse doctors
│   ├── DoctorDetail.tsx    # View slots & book
│   ├── MyBookings.tsx      # Manage bookings
│   └── admin/              # Admin panel
└── App.tsx              # Routing & navigation
```

**Key Technologies**:
- React 19 with TypeScript
- React Router for client-side routing
- Vite for fast development
- date-fns for date manipulation
- lucide-react for icons

## 🚀 Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 18+](https://nodejs.org/)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd hms
   ```

2. **Install frontend dependencies**
   ```bash
   cd src/Web/ClientApp
   npm install
   ```

### Running the Application

#### Option 1: Development Mode (Recommended)

**Terminal 1 - Backend**:
```bash
cd src/Web
dotnet run
```
Backend runs on `http://localhost:5128`

**Terminal 2 - Frontend**:
```bash
cd src/Web/ClientApp
npm run dev
```
Frontend runs on `http://localhost:5173` (or 5174)

#### Option 2: Production Build

```bash
# Build frontend
cd src/Web/ClientApp
npm run build

# Run backend (serves built frontend)
cd ../
dotnet run
```

Visit `http://localhost:5128` for the full application.

### API Documentation

When running in development mode, access the interactive API documentation:
- **Scalar UI**: `http://localhost:5128/scalar/v1`

## 📖 Usage

### Patient Flow

1. **Browse Doctors**
   - Navigate to the home page
   - Use search to find doctors by name
   - Filter by medical specialty

2. **Book an Appointment**
   - Click on a doctor card
   - Select a date from the date picker
   - Choose an available time slot
   - Enter your patient ID
   - Confirm booking

3. **Manage Bookings**
   - Click "My Bookings" in navigation
   - Enter your patient ID
   - View active appointments
   - Cancel if needed

### Admin Flow

1. **Access Admin Panel**
   - Click "Admin" in navigation
   - Navigate to Doctors or Patients

2. **Configure Doctor Availability**
   - Go to Admin → Doctors
   - Click "Configure Availability" on a doctor
   - Set working days (Mon-Sun)
   - Set daily hours and slot duration
   - Set rolling window (days ahead for bookings)
   - Save configuration

3. **View Patients**
   - Go to Admin → Patients
   - Search by name or email
   - View contact information

## 🔌 API Endpoints

### Public Endpoints

```http
GET    /api/doctors
GET    /api/doctors/{doctorId}/slots?date={yyyy-MM-dd}
POST   /api/doctors/{doctorId}/slots/{slotId}/book
GET    /api/patients/{patientId}/bookings
POST   /api/doctors/{doctorId}/slots/{slotId}/cancel
```

### Admin Endpoints

```http
GET    /api/admin/doctors
GET    /api/admin/patients
PUT    /api/admin/doctors/{doctorId}/availability
GET    /api/admin/doctors/{doctorId}/availability
POST   /api/admin/doctors/{doctorId}/availability-exceptions
GET    /api/admin/doctors/{doctorId}/availability-exceptions
```

For detailed API documentation with request/response examples, see [docs/WALKTHROUGH.md](docs/WALKTHROUGH.md).

## 🎯 Key Features Explained

### Automated Slot Generation

A background worker (`SlotGenerationWorker`) runs every 12 hours to:
- Read doctor availability configurations
- Generate time slots for the rolling window period
- Respect working days, daily hours, and slot duration
- Skip availability exceptions (holidays, time off)

**Example**: A doctor working Mon-Fri, 9:00-17:00 with 30-minute slots and a 14-day rolling window will have ~80 slots auto-generated.

### Booking Management

- **Book**: Slot status changes from `Available` to `Booked`
- **Cancel**: Booking status changes to `Cancelled`, slot becomes `Available` again
- **Block**: Admins can block slots to prevent bookings

### Availability Configuration

Admins can configure for each doctor:
- **Working Days**: Which days of the week they work
- **Daily Hours**: Start and end time each day
- **Slot Duration**: Length of each appointment (e.g., 30 minutes)
- **Rolling Window**: How far ahead patients can book (e.g., 14 days)

## 📁 Project Structure

```
hms/
├── src/
│   ├── ReferenceData/          # Master data bounded context
│   ├── Scheduling/             # Scheduling bounded context
│   └── Web/                    # API & Frontend
│       ├── ClientApp/          # React application
│       ├── Endpoints/          # API endpoints
│       ├── Workers/            # Background services
│       └── Program.cs          # Application startup
├── test/
│   └── Scheduling.Tests/       # Unit tests
├── docs/                       # Documentation
│   ├── WALKTHROUGH.md         # Detailed walkthrough
│   └── assets/                # Screenshots & videos
└── README.md                   # This file
```

## 🧪 Testing

### Build Verification

```bash
# Backend
cd src/Web
dotnet build

# Frontend
cd src/Web/ClientApp
npm run build
```

### Manual Testing

See [docs/WALKTHROUGH.md](docs/WALKTHROUGH.md) for detailed testing workflows.

## 🛠️ Development

### Adding a New Feature

1. **Backend**:
   - Add domain entity in appropriate bounded context
   - Create use case in Application layer
   - Implement in Infrastructure layer
   - Add endpoint in Web/Endpoints

2. **Frontend**:
   - Add TypeScript interface in `lib/types.ts`
   - Add API method in `lib/api.ts`
   - Create/update page component
   - Update routing if needed

### Code Style

- **Backend**: Follow C# conventions, use nullable reference types
- **Frontend**: ESLint configuration included, use TypeScript strictly

## 🐛 Troubleshooting

### Frontend won't build
**Issue**: TypeScript errors about type imports  
**Fix**: Ensure type-only imports use `import type { ... }`

### No slots showing
**Issue**: Doctor availability not configured  
**Fix**: Go to Admin → Doctors → Configure Availability

### Backend database errors
**Issue**: Database files corrupted  
**Fix**: Delete `*.db` files in `src/Web/` and restart backend

## 📚 Documentation

- **[Walkthrough](docs/WALKTHROUGH.md)** - Detailed feature documentation with screenshots
- **[Project Knowledge](docs/PROJECT_KNOWLEDGE.md)** - Comprehensive architecture and implementation guide
- **[API Reference](http://localhost:5128/scalar/v1)** - Interactive API documentation (when running)

## 🔮 Future Enhancements

### Planned Features
- [ ] Authentication & Authorization (JWT-based)
- [ ] Availability Exceptions UI (holidays, time off)
- [ ] Slot Blocking Interface for admins
- [ ] Email/SMS notifications for bookings
- [ ] Doctor profiles with photos and bios
- [ ] Patient reviews and ratings
- [ ] Analytics dashboard for admins

### Technical Improvements
- [ ] Unit and integration tests
- [ ] Real-time updates with SignalR
- [ ] Optimistic UI updates
- [ ] Offline support with service workers

## 🤝 Contributing

This is a demonstration project. For production use, consider:
- Adding proper authentication and authorization
- Implementing comprehensive error handling
- Adding logging and monitoring
- Setting up CI/CD pipelines
- Adding automated tests

## 📄 License

This project is provided as-is for educational and demonstration purposes.

## 🙏 Acknowledgments

Built with:
- [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
- [React](https://react.dev/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [Vite](https://vitejs.dev/)
- [Lucide Icons](https://lucide.dev/)
- [date-fns](https://date-fns.org/)

---

**Made with ❤️ using Clean Architecture and Domain-Driven Design**
