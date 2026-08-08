# BusTravel Management System

A comprehensive bus travel booking and management platform built with ASP.NET MVC 5, Entity Framework 6, MySQL 8, Bootstrap 5, and React 18.

## Architecture Overview

```
BusTravelManagement/
├── App_Start/           # Application startup configurations
│   ├── AuthConfig.cs
│   ├── BundleConfig.cs
│   ├── DatabaseConfig.cs
│   ├── DependencyInjectionConfig.cs
│   ├── FilterConfig.cs
│   ├── LoggingConfig.cs
│   ├── MapperConfig.cs
│   └── RouteConfig.cs
├── Controllers/         # MVC Controllers
│   ├── AccountController.cs
│   ├── AdminController.cs
│   ├── ApiController.cs
│   ├── BookingController.cs
│   ├── ErrorController.cs
│   ├── HomeController.cs
│   ├── OperatorController.cs
│   ├── ReportController.cs
│   ├── ReviewController.cs
│   └── SearchController.cs
├── Database/            # SQL Scripts
│   ├── schema.sql
│   └── seed-data.sql
├── Filters/             # Action Filters
│   ├── AuditLogFilter.cs
│   ├── GlobalExceptionFilter.cs
│   ├── RoleAuthorizationFilter.cs
│   └── ValidationFilter.cs
├── Models/
│   ├── BusTravelDbContext.cs
│   ├── Entities/        # Entity Framework POCOs
│   │   ├── Booking.cs
│   │   ├── Location.cs
│   │   ├── Operator.cs
│   │   ├── Schedule.cs
│   │   └── User.cs
│   └── ViewModels/      # MVC View Models
│       ├── AccountViewModels.cs
│       ├── AdminViewModels.cs
│       ├── BookingViewModels.cs
│       ├── OperatorViewModels.cs
│       ├── ReportViewModels.cs
│       ├── ReviewViewModels.cs
│       ├── SearchViewModels.cs
│       └── SharedViewModels.cs
├── Repositories/        # Data Access Layer
│   ├── Interfaces/
│   │   └── IRepository.cs
│   ├── BaseRepository.cs
│   ├── UnitOfWork.cs
│   └── UserRepository.cs
├── Security/            # Security & Authentication
│   ├── CustomIdentityManager.cs
│   ├── PasswordHelper.cs
│   └── TokenHelper.cs
├── Services/            # Business Logic Layer
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   └── ISearchService.cs
│   └── AuthService.cs
├── Utilities/           # Helpers & Extensions
│   ├── Constants/
│   ├── Extensions/
│   └── Helpers/
├── Views/               # Razor Views
├── Content/             # Static Assets
│   └── css/
├── Scripts/             # JavaScript
│   ├── js/
│   └── js/react/        # React Components
└── Global.asax.cs       # Application Entry Point
```

## Tech Stack

| Technology | Purpose |
|-----------|---------|
| ASP.NET MVC 5 | Web framework |
| Entity Framework 6 | ORM for data access |
| MySQL 8 | Relational database |
| Bootstrap 5 | Frontend UI framework |
| React 18 | Interactive UI components |
| OWIN | Authentication middleware |
| log4net | Logging |
| Newtonsoft.Json | JSON serialization |

## Key Features

### Customer Module
- Bus search with filters (source, destination, date, AC/Non-AC, Sleeper, price range)
- Interactive seat selection with real-time availability
- Multi-step booking process
- Multiple payment gateway integration
- E-ticket generation with QR code
- Booking history and cancellation
- Reviews and ratings

### Operator Module
- Bus and fleet management
- Schedule management
- Seat layout configuration
- Pricing and cancellation policy management
- Revenue and occupancy reports
- Real-time booking dashboard

### Admin Module
- User management with role-based access
- Operator approval and management
- Coupon and promotion management
- Review moderation
- Comprehensive reports (daily, monthly, yearly)
- Audit logging

### Technical Features
- Repository pattern with Unit of Work
- Dependency injection (custom DI container)
- Role-based authorization
- Global exception handling
- Audit logging
- Caching strategy
- Bundling and minification
- Responsive design (mobile-first)

## Database

The system uses 31+ normalized MySQL tables covering:

- **Users & Auth**: Users, Roles, Permissions, UserRoles
- **Operators**: Operators, Buses, BusTypes, Amenities
- **Routes**: Cities, Routes, Stops, BoardingPoints, DroppingPoints
- **Scheduling**: Schedules, ScheduleStops, SeatLayouts
- **Bookings**: Bookings, BookingPassengers, Payments, Refunds, Tickets
- **Marketing**: Coupons
- **Engagement**: Reviews, Notifications
- **Finance**: WalletTransactions
- **Policies**: CancellationPolicies

## Setup Instructions

### Prerequisites
- Visual Studio 2022 (or later)
- .NET Framework 4.8 SDK
- MySQL 8.0+
- NuGet packages (restore from packages.config)

### Installation

1. **Clone the repository**
   ```
   git clone <repo-url>
   cd BusTravelManagement
   ```

2. **Set up the database**
   ```sql
   mysql -u root -p < Database/schema.sql
   mysql -u root -p < Database/seed-data.sql
   ```

3. **Configure connection string**
   Edit `Web.config` - update the `DefaultConnection` connection string:
   ```xml
   <connectionString="server=localhost;port=3306;database=BusTravelDB;uid=root;pwd=YOUR_PASSWORD;" />
   ```

4. **Restore NuGet packages**
   ```
   nuget restore
   ```

5. **Build and run**
   Open in Visual Studio 2022, build, and run (F5).

### Default Credentials
| Role | Email | Password |
|------|-------|----------|
| SuperAdmin | admin@bustravel.com | Admin@123 |
| Operator | operator@bustravel.com | Operator@123 |
| Customer | customer@bustravel.com | Customer@123 |

## API Endpoints

The system provides RESTful JSON API endpoints:

| Endpoint | Method | Description |
|----------|--------|-------------|
| /Api/SearchCities?q= | GET | Autocomplete cities |
| /Api/SearchBuses?from=&to=&date= | GET | Search available buses |
| /Api/GetAvailableSeats?scheduleId=&date= | GET | Get seat availability |
| /Api/GetBusReviews?busId= | GET | Get bus reviews |
| /Api/GetPopularCities | GET | Popular cities list |
| /Api/GetBookingStatus?bookingNumber= | GET | Check booking status |
| /Api/ValidateCoupon | POST | Validate coupon code |

## Security

- Passwords hashed with PBKDF2 (100,000 iterations, SHA-256)
- Forms authentication with encrypted tickets
- Role-based authorization filters
- Anti-forgery token protection
- SQL injection prevention via Entity Framework
- XSS protection via Razor encoding
- Request validation enabled
- Security headers (X-Content-Type-Options, X-Frame-Options, X-XSS-Protection)

## License

Proprietary - All rights reserved.
