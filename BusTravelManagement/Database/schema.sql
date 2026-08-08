-- ============================================================================
-- Bus Travel Management System - MySQL 8 Database Schema (CORRECTED)
-- ============================================================================
-- Database: BusTravelDB
-- Engine: InnoDB
-- Charset: utf8mb4
-- Collation: utf8mb4_unicode_ci
-- ============================================================================

CREATE DATABASE IF NOT EXISTS BusTravelDB
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE BusTravelDB;

-- ============================================================================
-- 1. Roles
-- ============================================================================
CREATE TABLE Roles (
    Id              INT             NOT NULL AUTO_INCREMENT,
    Name            VARCHAR(100)    NOT NULL,
    Description     VARCHAR(500)    NULL,
    IsActive        TINYINT(1)      NOT NULL DEFAULT 1,
    CreatedAt       DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE INDEX UQ_Roles_Name (Name),
    INDEX IX_Roles_IsActive (IsActive)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 2. Permissions
-- ============================================================================
CREATE TABLE Permissions (
    Id              INT             NOT NULL AUTO_INCREMENT,
    Name            VARCHAR(100)    NOT NULL,
    Description     VARCHAR(500)    NULL,
    Module          VARCHAR(100)    NOT NULL,
    IsActive        TINYINT(1)      NOT NULL DEFAULT 1,
    CreatedAt       DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE INDEX UQ_Permissions_Name (Name),
    INDEX IX_Permissions_Module (Module),
    INDEX IX_Permissions_IsActive (IsActive)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 3. RolePermissions
-- ============================================================================
CREATE TABLE RolePermissions (
    RoleId          INT             NOT NULL,
    PermissionId    INT             NOT NULL,
    CreatedAt       DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (RoleId, PermissionId),
    INDEX IX_RolePermissions_PermissionId (PermissionId),
    CONSTRAINT FK_RolePermissions_Roles
        FOREIGN KEY (RoleId) REFERENCES Roles(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_RolePermissions_Permissions
        FOREIGN KEY (PermissionId) REFERENCES Permissions(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 4. Users
-- ============================================================================
CREATE TABLE Users (
    Id                      INT             NOT NULL AUTO_INCREMENT,
    FirstName               VARCHAR(100)    NOT NULL,
    LastName                VARCHAR(100)    NOT NULL,
    Email                   VARCHAR(255)    NOT NULL,
    PasswordHash            VARCHAR(500)    NOT NULL,
    PhoneNumber             VARCHAR(20)     NULL,
    DateOfBirth             DATE            NULL,
    Gender                  VARCHAR(10)     NULL,
    ProfileImageUrl         VARCHAR(500)    NULL,
    IsEmailVerified         TINYINT(1)      NOT NULL DEFAULT 0,
    EmailVerificationToken  VARCHAR(500)    NULL,
    ResetPasswordToken      VARCHAR(500)    NULL,
    ResetPasswordTokenExpiry DATETIME       NULL,
    IsActive                TINYINT(1)      NOT NULL DEFAULT 1,
    IsLocked                TINYINT(1)      NOT NULL DEFAULT 0,
    FailedLoginAttempts     INT             NOT NULL DEFAULT 0,
    LastLoginAt             DATETIME        NULL,
    CreatedAt               DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt               DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE INDEX UQ_Users_Email (Email),
    INDEX IX_Users_PhoneNumber (PhoneNumber),
    INDEX IX_Users_IsActive (IsActive),
    INDEX IX_Users_IsLocked (IsLocked),
    INDEX IX_Users_CreatedAt (CreatedAt)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 5. UserRoles
-- ============================================================================
CREATE TABLE UserRoles (
    UserId          INT             NOT NULL,
    RoleId          INT             NOT NULL,
    CreatedAt       DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (UserId, RoleId),
    INDEX IX_UserRoles_RoleId (RoleId),
    CONSTRAINT FK_UserRoles_Users
        FOREIGN KEY (UserId) REFERENCES Users(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_UserRoles_Roles
        FOREIGN KEY (RoleId) REFERENCES Roles(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 6. Operators
-- ============================================================================
CREATE TABLE Operators (
    Id                      INT             NOT NULL AUTO_INCREMENT,
    UserId                  INT             NOT NULL,
    CompanyName             VARCHAR(200)    NOT NULL,
    CompanyRegistrationNumber VARCHAR(100)  NULL,
    ContactPerson           VARCHAR(200)    NULL,
    ContactEmail            VARCHAR(255)    NULL,
    ContactPhone            VARCHAR(20)     NULL,
    Address                 VARCHAR(500)    NULL,
    City                    VARCHAR(100)    NULL,
    State                   VARCHAR(100)    NULL,
    Country                 VARCHAR(100)    NOT NULL DEFAULT 'India',
    PostalCode              VARCHAR(20)     NULL,
    WebsiteUrl              VARCHAR(500)    NULL,
    LogoUrl                 VARCHAR(500)    NULL,
    CommissionPercentage    DECIMAL(5,2)    NOT NULL DEFAULT 0.00,
    IsActive                TINYINT(1)      NOT NULL DEFAULT 1,
    IsVerified              TINYINT(1)      NOT NULL DEFAULT 0,
    CreatedAt               DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt               DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    INDEX IX_Operators_UserId (UserId),
    INDEX IX_Operators_CompanyName (CompanyName),
    INDEX IX_Operators_IsActive (IsActive),
    INDEX IX_Operators_IsVerified (IsVerified),
    CONSTRAINT FK_Operators_Users
        FOREIGN KEY (UserId) REFERENCES Users(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 7. BusTypes
-- ============================================================================
CREATE TABLE BusTypes (
    Id              INT             NOT NULL AUTO_INCREMENT,
    Name            VARCHAR(100)    NOT NULL,
    Description     VARCHAR(500)    NULL,
    IsActive        TINYINT(1)      NOT NULL DEFAULT 1,
    CreatedAt       DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE INDEX UQ_BusTypes_Name (Name),
    INDEX IX_BusTypes_IsActive (IsActive)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 8. Amenities
-- ============================================================================
CREATE TABLE Amenities (
    Id              INT             NOT NULL AUTO_INCREMENT,
    Name            VARCHAR(100)    NOT NULL,
    IconClass       VARCHAR(100)    NULL,
    Description     VARCHAR(500)    NULL,
    IsActive        TINYINT(1)      NOT NULL DEFAULT 1,
    CreatedAt       DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE INDEX UQ_Amenities_Name (Name),
    INDEX IX_Amenities_IsActive (IsActive)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 9. Buses
-- ============================================================================
CREATE TABLE Buses (
    Id              INT             NOT NULL AUTO_INCREMENT,
    OperatorId      INT             NOT NULL,
    BusTypeId       INT             NOT NULL,
    BusNumber       VARCHAR(50)     NOT NULL,
    RegistrationNumber VARCHAR(100) NULL,
    TotalSeats      INT             NOT NULL,
    SeatLayoutType  ENUM('2x2','2x1','1x2','3x2','2x3','Sleeper') NOT NULL DEFAULT '2x2',
    IsAC            TINYINT(1)      NOT NULL DEFAULT 0,
    IsSleeper       TINYINT(1)      NOT NULL DEFAULT 0,
    IsSingleAxle    TINYINT(1)      NOT NULL DEFAULT 1,
    IsActive        TINYINT(1)      NOT NULL DEFAULT 1,
    IsDeleted       TINYINT(1)      NOT NULL DEFAULT 0,
    CreatedAt       DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt       DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE INDEX UQ_Buses_BusNumber (BusNumber),
    INDEX IX_Buses_OperatorId (OperatorId),
    INDEX IX_Buses_BusTypeId (BusTypeId),
    INDEX IX_Buses_IsActive (IsActive),
    INDEX IX_Buses_IsDeleted (IsDeleted),
    INDEX IX_Buses_BusId_IsActive (Id, IsActive),
    CONSTRAINT FK_Buses_Operators
        FOREIGN KEY (OperatorId) REFERENCES Operators(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_Buses_BusTypes
        FOREIGN KEY (BusTypeId) REFERENCES BusTypes(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 10. BusAmenities
-- ============================================================================
CREATE TABLE BusAmenities (
    Id              INT             NOT NULL AUTO_INCREMENT,
    BusId           INT             NOT NULL,
    AmenityId       INT             NOT NULL,
    IsActive        TINYINT(1)      NOT NULL DEFAULT 1,
    CreatedAt       DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE INDEX UQ_BusAmenities (BusId, AmenityId),
    INDEX IX_BusAmenities_AmenityId (AmenityId),
    INDEX IX_BusAmenities_IsActive (IsActive),
    CONSTRAINT FK_BusAmenities_Buses
        FOREIGN KEY (BusId) REFERENCES Buses(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_BusAmenities_Amenities
        FOREIGN KEY (AmenityId) REFERENCES Amenities(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 11. Cities
-- ============================================================================
CREATE TABLE Cities (
    Id              INT             NOT NULL AUTO_INCREMENT,
    Name            VARCHAR(100)    NOT NULL,
    State           VARCHAR(100)    NULL,
    Country         VARCHAR(100)    NOT NULL DEFAULT 'India',
    Latitude        DECIMAL(10,7)   NULL,
    Longitude       DECIMAL(10,7)   NULL,
    IsActive        TINYINT(1)      NOT NULL DEFAULT 1,
    IsPopular       TINYINT(1)      NOT NULL DEFAULT 0,
    CreatedAt       DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    INDEX IX_Cities_Name (Name),
    INDEX IX_Cities_State (State),
    INDEX IX_Cities_IsActive (IsActive),
    INDEX IX_Cities_IsPopular (IsPopular),
    INDEX IX_Cities_Name_State (Name, State)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 12. Routes
-- ============================================================================
CREATE TABLE Routes (
    Id                  INT             NOT NULL AUTO_INCREMENT,
    SourceCityId        INT             NOT NULL,
    DestinationCityId   INT             NOT NULL,
    Distance            DECIMAL(10,2)   NULL,
    DurationMinutes     INT             NULL,
    IsActive            TINYINT(1)      NOT NULL DEFAULT 1,
    CreatedAt           DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    INDEX IX_Routes_SourceCityId (SourceCityId),
    INDEX IX_Routes_DestinationCityId (DestinationCityId),
    INDEX IX_Routes_SourceCityId_DestinationCityId_IsActive (SourceCityId, DestinationCityId, IsActive),
    CONSTRAINT FK_Routes_SourceCity
        FOREIGN KEY (SourceCityId) REFERENCES Cities(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_Routes_DestinationCity
        FOREIGN KEY (DestinationCityId) REFERENCES Cities(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 13. Stops
-- ============================================================================
CREATE TABLE Stops (
    Id              INT             NOT NULL AUTO_INCREMENT,
    RouteId         INT             NOT NULL,
    CityId          INT             NULL,
    StopName        VARCHAR(200)    NOT NULL,
    StopOrder       INT             NOT NULL,
    StopTime        TIME            NULL,
    Address         VARCHAR(500)    NULL,
    Latitude        DECIMAL(10,7)   NULL,
    Longitude       DECIMAL(10,7)   NULL,
    IsActive        TINYINT(1)      NOT NULL DEFAULT 1,
    PRIMARY KEY (Id),
    INDEX IX_Stops_RouteId (RouteId),
    INDEX IX_Stops_CityId (CityId),
    INDEX IX_Stops_StopOrder (StopOrder),
    INDEX IX_Stops_RouteId_StopOrder (RouteId, StopOrder),
    CONSTRAINT FK_Stops_Routes
        FOREIGN KEY (RouteId) REFERENCES Routes(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_Stops_Cities
        FOREIGN KEY (CityId) REFERENCES Cities(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 14. SeatLayouts (moved before Schedules and BoardingPoints to satisfy FK order)
-- ============================================================================
CREATE TABLE SeatLayouts (
    Id              INT             NOT NULL AUTO_INCREMENT,
    BusId           INT             NOT NULL,
    RowNumber       INT             NOT NULL,
    ColumnNumber    INT             NOT NULL,
    SeatNumber      VARCHAR(10)     NOT NULL,
    SeatPosition    ENUM('Window','Aisle','Middle') NOT NULL DEFAULT 'Aisle',
    Deck            ENUM('Lower','Upper') NOT NULL DEFAULT 'Lower',
    IsActive        TINYINT(1)      NOT NULL DEFAULT 1,
    CreatedAt       DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    INDEX IX_SeatLayouts_BusId (BusId),
    INDEX IX_SeatLayouts_BusId_Row (BusId, RowNumber),
    INDEX IX_SeatLayouts_IsActive (IsActive),
    CONSTRAINT FK_SeatLayouts_Buses
        FOREIGN KEY (BusId) REFERENCES Buses(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 15. Schedules (moved before BoardingPoints/DroppingPoints)
-- ============================================================================
CREATE TABLE Schedules (
    Id              INT             NOT NULL AUTO_INCREMENT,
    BusId           INT             NOT NULL,
    RouteId         INT             NOT NULL,
    DepartureTime   TIME            NOT NULL,
    ArrivalTime     TIME            NOT NULL,
    DurationMinutes INT             NULL,
    Frequency       ENUM('Daily','Weekly','Custom') NOT NULL DEFAULT 'Daily',
    DaysOfWeek      VARCHAR(35)     NULL,
    StartDate       DATE            NULL,
    EndDate         DATE            NULL,
    BaseFare        DECIMAL(10,2)   NOT NULL DEFAULT 0.00,
    PerKmRate       DECIMAL(5,2)    NOT NULL DEFAULT 0.00,
    IsActive        TINYINT(1)      NOT NULL DEFAULT 1,
    IsRecurring     TINYINT(1)      NOT NULL DEFAULT 1,
    CreatedAt       DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt       DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    INDEX IX_Schedules_BusId (BusId),
    INDEX IX_Schedules_RouteId (RouteId),
    INDEX IX_Schedules_IsActive (IsActive),
    INDEX IX_Schedules_DepartureTime (DepartureTime),
    CONSTRAINT FK_Schedules_Buses
        FOREIGN KEY (BusId) REFERENCES Buses(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_Schedules_Routes
        FOREIGN KEY (RouteId) REFERENCES Routes(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 16. BoardingPoints (now after Schedules)
-- ============================================================================
CREATE TABLE BoardingPoints (
    Id              INT             NOT NULL AUTO_INCREMENT,
    ScheduleId      INT             NOT NULL,
    StopId          INT             NULL,
    Name            VARCHAR(200)    NOT NULL,
    Address         VARCHAR(500)    NULL,
    Landmark        VARCHAR(200)    NULL,
    Latitude        DECIMAL(10,7)   NULL,
    Longitude       DECIMAL(10,7)   NULL,
    PickupTime      TIME            NULL,
    PickupDayOffset INT             NOT NULL DEFAULT 0,
    IsActive        TINYINT(1)      NOT NULL DEFAULT 1,
    CreatedAt       DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    INDEX IX_BoardingPoints_ScheduleId (ScheduleId),
    INDEX IX_BoardingPoints_StopId (StopId),
    INDEX IX_BoardingPoints_IsActive (IsActive),
    CONSTRAINT FK_BoardingPoints_Schedules
        FOREIGN KEY (ScheduleId) REFERENCES Schedules(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_BoardingPoints_Stops
        FOREIGN KEY (StopId) REFERENCES Stops(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 17. DroppingPoints (now after Schedules)
-- ============================================================================
CREATE TABLE DroppingPoints (
    Id              INT             NOT NULL AUTO_INCREMENT,
    ScheduleId      INT             NOT NULL,
    StopId          INT             NULL,
    Name            VARCHAR(200)    NOT NULL,
    Address         VARCHAR(500)    NULL,
    Landmark        VARCHAR(200)    NULL,
    Latitude        DECIMAL(10,7)   NULL,
    Longitude       DECIMAL(10,7)   NULL,
    DropTime        TIME            NULL,
    DropDayOffset   INT             NOT NULL DEFAULT 0,
    IsActive        TINYINT(1)      NOT NULL DEFAULT 1,
    CreatedAt       DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    INDEX IX_DroppingPoints_ScheduleId (ScheduleId),
    INDEX IX_DroppingPoints_StopId (StopId),
    INDEX IX_DroppingPoints_IsActive (IsActive),
    CONSTRAINT FK_DroppingPoints_Schedules
        FOREIGN KEY (ScheduleId) REFERENCES Schedules(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_DroppingPoints_Stops
        FOREIGN KEY (StopId) REFERENCES Stops(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 18. ScheduleStops (now after Schedules and Stops)
-- ============================================================================
CREATE TABLE ScheduleStops (
    Id                  INT             NOT NULL AUTO_INCREMENT,
    ScheduleId          INT             NOT NULL,
    StopId              INT             NOT NULL,
    ArrivalTime         TIME            NULL,
    DepartureTime       TIME            NULL,
    StopOrder           INT             NOT NULL,
    DistanceFromStart   DECIMAL(10,2)   NULL DEFAULT 0.00,
    FareMultiplier      DECIMAL(5,2)    NOT NULL DEFAULT 1.00,
    PRIMARY KEY (Id),
    INDEX IX_ScheduleStops_ScheduleId (ScheduleId),
    INDEX IX_ScheduleStops_StopId (StopId),
    INDEX IX_ScheduleStops_ScheduleId_StopOrder (ScheduleId, StopOrder),
    CONSTRAINT FK_ScheduleStops_Schedules
        FOREIGN KEY (ScheduleId) REFERENCES Schedules(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_ScheduleStops_Stops
        FOREIGN KEY (StopId) REFERENCES Stops(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 19. Coupons (moved before Bookings)
-- ============================================================================
CREATE TABLE Coupons (
    Id                  INT             NOT NULL AUTO_INCREMENT,
    Code                VARCHAR(50)     NOT NULL,
    Description         VARCHAR(500)    NULL,
    DiscountType        ENUM('Percentage','Flat') NOT NULL DEFAULT 'Flat',
    DiscountValue       DECIMAL(10,2)   NOT NULL,
    MinBookingAmount    DECIMAL(10,2)   NULL DEFAULT 0.00,
    MaxDiscountAmount   DECIMAL(10,2)   NULL,
    UsageLimit          INT             NULL,
    UsedCount           INT             NOT NULL DEFAULT 0,
    PerUserLimit        INT             NOT NULL DEFAULT 1,
    IsActive            TINYINT(1)      NOT NULL DEFAULT 1,
    ValidFrom           DATETIME        NULL,
    ValidTo             DATETIME        NULL,
    CreatedAt           DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE INDEX UQ_Coupons_Code (Code),
    INDEX IX_Coupons_IsActive (IsActive),
    INDEX IX_Coupons_ValidFrom_ValidTo (ValidFrom, ValidTo)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 20. Bookings
-- ============================================================================
CREATE TABLE Bookings (
    Id                  INT             NOT NULL AUTO_INCREMENT,
    BookingNumber       VARCHAR(20)     NOT NULL,
    UserId              INT             NOT NULL,
    ScheduleId          INT             NOT NULL,
    JourneyDate         DATE            NOT NULL,
    SourceStopId        INT             NOT NULL,
    DestinationStopId   INT             NOT NULL,
    BoardingPointId     INT             NULL,
    DroppingPointId     INT             NULL,
    BookingStatus       ENUM('Pending','Confirmed','Cancelled','Completed','Refunded') NOT NULL DEFAULT 'Pending',
    NumberOfSeats       INT             NOT NULL DEFAULT 1,
    TotalFare           DECIMAL(10,2)   NOT NULL DEFAULT 0.00,
    ConvenienceFee      DECIMAL(10,2)   NOT NULL DEFAULT 0.00,
    TaxAmount           DECIMAL(10,2)   NOT NULL DEFAULT 0.00,
    DiscountAmount      DECIMAL(10,2)   NOT NULL DEFAULT 0.00,
    CouponId            INT             NULL,
    CouponDiscount      DECIMAL(10,2)   NOT NULL DEFAULT 0.00,
    NetAmount           DECIMAL(10,2)   NOT NULL DEFAULT 0.00,
    RefundAmount        DECIMAL(10,2)   NOT NULL DEFAULT 0.00,
    CancellationCharge  DECIMAL(10,2)   NOT NULL DEFAULT 0.00,
    ContactName         VARCHAR(100)    NULL,
    ContactPhone        VARCHAR(20)     NULL,
    ContactEmail        VARCHAR(100)    NULL,
    SpecialRequests     TEXT            NULL,
    IsCancelled         TINYINT(1)      NOT NULL DEFAULT 0,
    CancelledAt         DATETIME        NULL,
    CancelledBy         INT             NULL,
    CancellationReason  TEXT            NULL,
    CreatedAt           DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt           DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE INDEX UQ_Bookings_BookingNumber (BookingNumber),
    INDEX IX_Bookings_UserId (UserId),
    INDEX IX_Bookings_ScheduleId (ScheduleId),
    INDEX IX_Bookings_ScheduleId_JourneyDate (ScheduleId, JourneyDate),
    INDEX IX_Bookings_SourceStopId (SourceStopId),
    INDEX IX_Bookings_DestinationStopId (DestinationStopId),
    INDEX IX_Bookings_BoardingPointId (BoardingPointId),
    INDEX IX_Bookings_DroppingPointId (DroppingPointId),
    INDEX IX_Bookings_BookingStatus (BookingStatus),
    INDEX IX_Bookings_JourneyDate (JourneyDate),
    INDEX IX_Bookings_CouponId (CouponId),
    INDEX IX_Bookings_CancelledBy (CancelledBy),
    INDEX IX_Bookings_CreatedAt (CreatedAt),
    CONSTRAINT FK_Bookings_Users
        FOREIGN KEY (UserId) REFERENCES Users(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_Bookings_Schedules
        FOREIGN KEY (ScheduleId) REFERENCES Schedules(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_Bookings_SourceStop
        FOREIGN KEY (SourceStopId) REFERENCES Stops(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_Bookings_DestinationStop
        FOREIGN KEY (DestinationStopId) REFERENCES Stops(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_Bookings_BoardingPoint
        FOREIGN KEY (BoardingPointId) REFERENCES BoardingPoints(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_Bookings_DroppingPoint
        FOREIGN KEY (DroppingPointId) REFERENCES DroppingPoints(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_Bookings_Coupons
        FOREIGN KEY (CouponId) REFERENCES Coupons(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_Bookings_CancelledByUser
        FOREIGN KEY (CancelledBy) REFERENCES Users(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 21. BookingPassengers
-- ============================================================================
CREATE TABLE BookingPassengers (
    Id                  INT             NOT NULL AUTO_INCREMENT,
    BookingId           INT             NOT NULL,
    SeatId              INT             NULL,
    PassengerName       VARCHAR(200)    NOT NULL,
    Age                 INT             NOT NULL,
    Gender              VARCHAR(10)     NULL,
    IsLadiesSeat        TINYINT(1)      NOT NULL DEFAULT 0,
    IdCardType          VARCHAR(50)     NULL,
    IdCardNumber        VARCHAR(100)    NULL,
    Fare                DECIMAL(10,2)   NOT NULL DEFAULT 0.00,
    CancellationCharge  DECIMAL(10,2)   NOT NULL DEFAULT 0.00,
    IsCancelled         TINYINT(1)      NOT NULL DEFAULT 0,
    PRIMARY KEY (Id),
    INDEX IX_BookingPassengers_BookingId (BookingId),
    INDEX IX_BookingPassengers_SeatId (SeatId),
    CONSTRAINT FK_BookingPassengers_Bookings
        FOREIGN KEY (BookingId) REFERENCES Bookings(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_BookingPassengers_SeatLayouts
        FOREIGN KEY (SeatId) REFERENCES SeatLayouts(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 22. Payments
-- ============================================================================
CREATE TABLE Payments (
    Id                  INT             NOT NULL AUTO_INCREMENT,
    BookingId           INT             NOT NULL,
    PaymentReference    VARCHAR(100)    NOT NULL,
    PaymentMethod       VARCHAR(50)     NOT NULL,
    PaymentGateway      ENUM('Mock','Stripe','Razorpay','PayPal') NOT NULL DEFAULT 'Mock',
    GatewayTransactionId VARCHAR(200)   NULL,
    Amount              DECIMAL(10,2)   NOT NULL,
    Currency            VARCHAR(3)      NOT NULL DEFAULT 'INR',
    PaymentStatus       ENUM('Pending','Success','Failed','Refunded') NOT NULL DEFAULT 'Pending',
    FailureReason       TEXT            NULL,
    PaidAt              DATETIME        NULL,
    RefundedAt          DATETIME        NULL,
    CreatedAt           DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE INDEX UQ_Payments_PaymentReference (PaymentReference),
    INDEX IX_Payments_BookingId (BookingId),
    INDEX IX_Payments_PaymentStatus (PaymentStatus),
    INDEX IX_Payments_GatewayTransactionId (GatewayTransactionId),
    INDEX IX_Payments_PaymentMethod (PaymentMethod),
    CONSTRAINT FK_Payments_Bookings
        FOREIGN KEY (BookingId) REFERENCES Bookings(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 23. Refunds
-- ============================================================================
CREATE TABLE Refunds (
    Id                  INT             NOT NULL AUTO_INCREMENT,
    PaymentId           INT             NOT NULL,
    BookingId           INT             NOT NULL,
    RefundReference     VARCHAR(100)    NOT NULL,
    Amount              DECIMAL(10,2)   NOT NULL,
    RefundType          ENUM('Full','Partial') NOT NULL DEFAULT 'Full',
    Reason              TEXT            NULL,
    ProcessedAt         DATETIME        NULL,
    CreatedAt           DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE INDEX UQ_Refunds_RefundReference (RefundReference),
    INDEX IX_Refunds_PaymentId (PaymentId),
    INDEX IX_Refunds_BookingId (BookingId),
    CONSTRAINT FK_Refunds_Payments
        FOREIGN KEY (PaymentId) REFERENCES Payments(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_Refunds_Bookings
        FOREIGN KEY (BookingId) REFERENCES Bookings(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 24. Tickets
-- ============================================================================
CREATE TABLE Tickets (
    Id                  INT             NOT NULL AUTO_INCREMENT,
    BookingId           INT             NOT NULL,
    TicketNumber        VARCHAR(50)     NOT NULL,
    QRCodeData          TEXT            NULL,
    PdfPath             VARCHAR(500)    NULL,
    CancellationPolicy  TEXT            NULL,
    CancellationDeadline DATETIME       NULL,
    GeneratedAt         DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedAt           DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE INDEX UQ_Tickets_TicketNumber (TicketNumber),
    INDEX IX_Tickets_BookingId (BookingId),
    CONSTRAINT FK_Tickets_Bookings
        FOREIGN KEY (BookingId) REFERENCES Bookings(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 25. Reviews
-- ============================================================================
CREATE TABLE Reviews (
    Id                  INT             NOT NULL AUTO_INCREMENT,
    BookingId           INT             NOT NULL,
    UserId              INT             NOT NULL,
    BusId               INT             NOT NULL,
    Rating              INT             NOT NULL,
    ReviewText          TEXT            NULL,
    IsApproved          TINYINT(1)      NOT NULL DEFAULT 0,
    IsAbuseReported     TINYINT(1)      NOT NULL DEFAULT 0,
    AbuseReportReason   TEXT            NULL,
    CreatedAt           DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    INDEX IX_Reviews_BookingId (BookingId),
    INDEX IX_Reviews_UserId (UserId),
    INDEX IX_Reviews_BusId (BusId),
    INDEX IX_Reviews_Rating (Rating),
    INDEX IX_Reviews_IsApproved (IsApproved),
    CONSTRAINT CK_Reviews_Rating CHECK (Rating >= 1 AND Rating <= 5),
    CONSTRAINT FK_Reviews_Bookings
        FOREIGN KEY (BookingId) REFERENCES Bookings(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_Reviews_Users
        FOREIGN KEY (UserId) REFERENCES Users(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_Reviews_Buses
        FOREIGN KEY (BusId) REFERENCES Buses(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 26. Notifications
-- ============================================================================
CREATE TABLE Notifications (
    Id                  INT             NOT NULL AUTO_INCREMENT,
    UserId              INT             NOT NULL,
    Title               VARCHAR(200)    NOT NULL,
    Message             TEXT            NOT NULL,
    NotificationType    ENUM('Email','SMS','System') NOT NULL DEFAULT 'System',
    ReferenceType       VARCHAR(50)     NULL,
    ReferenceId         INT             NULL,
    IsRead              TINYINT(1)      NOT NULL DEFAULT 0,
    SentAt              DATETIME        NULL,
    ReadAt              DATETIME        NULL,
    CreatedAt           DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    INDEX IX_Notifications_UserId (UserId),
    INDEX IX_Notifications_IsRead (IsRead),
    INDEX IX_Notifications_NotificationType (NotificationType),
    INDEX IX_Notifications_CreatedAt (CreatedAt),
    CONSTRAINT FK_Notifications_Users
        FOREIGN KEY (UserId) REFERENCES Users(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 27. AuditLogs
-- ============================================================================
CREATE TABLE AuditLogs (
    Id                  INT             NOT NULL AUTO_INCREMENT,
    Action              VARCHAR(100)    NOT NULL,
    EntityType          VARCHAR(100)    NOT NULL,
    EntityId            INT             NOT NULL,
    UserId              INT             NULL,
    OldValues           TEXT            NULL,
    NewValues           TEXT            NULL,
    IpAddress           VARCHAR(45)     NULL,
    UserAgent           VARCHAR(500)    NULL,
    CreatedAt           DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    INDEX IX_AuditLogs_UserId (UserId),
    INDEX IX_AuditLogs_EntityType (EntityType),
    INDEX IX_AuditLogs_EntityType_EntityId (EntityType, EntityId),
    INDEX IX_AuditLogs_Action (Action),
    INDEX IX_AuditLogs_CreatedAt (CreatedAt),
    CONSTRAINT FK_AuditLogs_Users
        FOREIGN KEY (UserId) REFERENCES Users(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 28. SavedPassengers
-- ============================================================================
CREATE TABLE SavedPassengers (
    Id                  INT             NOT NULL AUTO_INCREMENT,
    UserId              INT             NOT NULL,
    FullName            VARCHAR(200)    NOT NULL,
    Age                 INT             NOT NULL,
    Gender              VARCHAR(10)     NULL,
    IdCardType          VARCHAR(50)     NULL,
    IdCardNumber        VARCHAR(100)    NULL,
    PhoneNumber         VARCHAR(20)     NULL,
    IsActive            TINYINT(1)      NOT NULL DEFAULT 1,
    CreatedAt           DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    INDEX IX_SavedPassengers_UserId (UserId),
    INDEX IX_SavedPassengers_IsActive (IsActive),
    CONSTRAINT FK_SavedPassengers_Users
        FOREIGN KEY (UserId) REFERENCES Users(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 29. WalletTransactions
-- ============================================================================
CREATE TABLE WalletTransactions (
    Id                  INT             NOT NULL AUTO_INCREMENT,
    UserId              INT             NOT NULL,
    TransactionType     ENUM('Credit','Debit') NOT NULL,
    Amount              DECIMAL(10,2)   NOT NULL,
    BalanceAfter        DECIMAL(10,2)   NOT NULL,
    Description         VARCHAR(500)    NULL,
    ReferenceType       VARCHAR(50)     NULL,
    ReferenceId         INT             NULL,
    CreatedAt           DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    INDEX IX_WalletTransactions_UserId (UserId),
    INDEX IX_WalletTransactions_ReferenceType_ReferenceId (ReferenceType, ReferenceId),
    INDEX IX_WalletTransactions_CreatedAt (CreatedAt),
    CONSTRAINT FK_WalletTransactions_Users
        FOREIGN KEY (UserId) REFERENCES Users(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 30. CancellationPolicies
-- ============================================================================
CREATE TABLE CancellationPolicies (
    Id                      INT             NOT NULL AUTO_INCREMENT,
    OperatorId              INT             NOT NULL,
    HoursBeforeDeparture    INT             NOT NULL,
    RefundPercentage        DECIMAL(5,2)    NOT NULL,
    IsActive                TINYINT(1)      NOT NULL DEFAULT 1,
    CreatedAt               DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    INDEX IX_CancellationPolicies_OperatorId (OperatorId),
    INDEX IX_CancellationPolicies_IsActive (IsActive),
    INDEX IX_CancellationPolicies_CreatedAt (CreatedAt),
    CONSTRAINT FK_CancellationPolicies_Operators
        FOREIGN KEY (OperatorId) REFERENCES Operators(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- 31. BusGalleries
-- ============================================================================
CREATE TABLE BusGalleries (
    Id              INT             NOT NULL AUTO_INCREMENT,
    BusId           INT             NOT NULL,
    ImageUrl        VARCHAR(500)    NOT NULL,
    Caption         VARCHAR(200)    NULL,
    SortOrder       INT             NOT NULL DEFAULT 0,
    IsActive        TINYINT(1)      NOT NULL DEFAULT 1,
    CreatedAt       DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    INDEX IX_BusGalleries_BusId (BusId),
    INDEX IX_BusGalleries_IsActive (IsActive),
    INDEX IX_BusGalleries_SortOrder (SortOrder),
    CONSTRAINT FK_BusGalleries_Buses
        FOREIGN KEY (BusId) REFERENCES Buses(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- VIEWS
-- ============================================================================

-- ----------------------------------------------------------------------------
-- vw_AvailableSeats: Shows available seats for each schedule
-- ----------------------------------------------------------------------------
CREATE OR REPLACE VIEW vw_AvailableSeats AS
WITH BookedSeats AS (
    SELECT DISTINCT
        bp.BookingId,
        b.ScheduleId,
        bp.SeatId,
        b.JourneyDate
    FROM BookingPassengers bp
    INNER JOIN Bookings b ON bp.BookingId = b.Id
    WHERE bp.IsCancelled = 0
      AND b.BookingStatus IN ('Confirmed', 'Pending')
)
SELECT
    s.Id AS ScheduleId,
    s.DepartureTime,
    s.ArrivalTime,
    b.Id AS BusId,
    b.BusNumber,
    sl.Id AS SeatId,
    sl.SeatNumber,
    sl.RowNumber,
    sl.ColumnNumber,
    sl.SeatPosition,
    sl.Deck,
    CASE
        WHEN bs.SeatId IS NOT NULL THEN 0
        ELSE 1
    END AS IsAvailable
FROM Schedules s
INNER JOIN Buses b ON s.BusId = b.Id
INNER JOIN SeatLayouts sl ON sl.BusId = b.Id AND sl.IsActive = 1
LEFT JOIN BookedSeats bs
    ON bs.ScheduleId = s.Id
    AND bs.SeatId = sl.Id;

-- ----------------------------------------------------------------------------
-- vw_RevenueReport: Daily/monthly revenue aggregation
-- ----------------------------------------------------------------------------
CREATE OR REPLACE VIEW vw_RevenueReport AS
SELECT
    DATE(p.PaidAt) AS TransactionDate,
    YEAR(p.PaidAt) AS RevenueYear,
    MONTH(p.PaidAt) AS RevenueMonth,
    DAY(p.PaidAt) AS RevenueDay,
    p.Currency,
    COUNT(DISTINCT p.Id) AS TotalTransactions,
    COUNT(DISTINCT p.BookingId) AS TotalBookings,
    SUM(CASE WHEN p.PaymentStatus = 'Success' THEN p.Amount ELSE 0 END) AS GrossRevenue,
    SUM(CASE WHEN p.PaymentStatus = 'Refunded' THEN p.Amount ELSE 0 END) AS TotalRefunds,
    SUM(CASE WHEN p.PaymentStatus = 'Success' THEN p.Amount ELSE 0 END)
    - SUM(CASE WHEN p.PaymentStatus = 'Refunded' THEN p.Amount ELSE 0 END) AS NetRevenue,
    p.PaymentMethod,
    p.PaymentGateway
FROM Payments p
WHERE p.PaidAt IS NOT NULL
GROUP BY
    DATE(p.PaidAt),
    YEAR(p.PaidAt),
    MONTH(p.PaidAt),
    DAY(p.PaidAt),
    p.Currency,
    p.PaymentMethod,
    p.PaymentGateway;

-- ----------------------------------------------------------------------------
-- vw_BookingSummary: Comprehensive booking details view
-- ----------------------------------------------------------------------------
CREATE OR REPLACE VIEW vw_BookingSummary AS
SELECT
    b.Id AS BookingId,
    b.BookingNumber,
    b.JourneyDate,
    b.BookingStatus,
    b.NumberOfSeats,
    b.TotalFare,
    b.ConvenienceFee,
    b.TaxAmount,
    b.DiscountAmount,
    b.CouponDiscount,
    b.NetAmount,
    b.RefundAmount,
    b.CancellationCharge,
    b.IsCancelled,
    b.CancelledAt,
    b.CancellationReason,
    b.CreatedAt AS BookingCreatedAt,
    u.Id AS UserId,
    u.FirstName,
    u.LastName,
    u.Email AS UserEmail,
    u.PhoneNumber AS UserPhone,
    b.ContactName,
    b.ContactPhone,
    b.ContactEmail,
    s.Id AS ScheduleId,
    s.DepartureTime,
    s.ArrivalTime,
    s.Frequency,
    src.Name AS SourceCity,
    dest.Name AS DestinationCity,
    bs.BusNumber,
    bs.TotalSeats,
    bs.SeatLayoutType,
    bs.IsAC,
    bs.IsSleeper,
    op.CompanyName AS OperatorName
FROM Bookings b
INNER JOIN Users u ON b.UserId = u.Id
INNER JOIN Schedules s ON b.ScheduleId = s.Id
INNER JOIN Routes r ON s.RouteId = r.Id
INNER JOIN Cities src ON r.SourceCityId = src.Id
INNER JOIN Cities dest ON r.DestinationCityId = dest.Id
INNER JOIN Buses bs ON s.BusId = bs.Id
INNER JOIN Operators op ON bs.OperatorId = op.Id;

-- ----------------------------------------------------------------------------
-- vw_BusOccupancy: Bus occupancy stats
-- ----------------------------------------------------------------------------
CREATE OR REPLACE VIEW vw_BusOccupancy AS
SELECT
    s.Id AS ScheduleId,
    b.Id AS BusId,
    b.BusNumber,
    b.TotalSeats,
    s.DepartureTime,
    s.ArrivalTime,
    r.SourceCityId,
    r.DestinationCityId,
    src.Name AS SourceCity,
    dest.Name AS DestinationCity,
    bk.JourneyDate,
    COUNT(DISTINCT bk.Id) AS TotalBookings,
    COALESCE(SUM(bk.NumberOfSeats), 0) AS SeatsBooked,
    b.TotalSeats - COALESCE(SUM(bk.NumberOfSeats), 0) AS SeatsAvailable,
    ROUND(
        (COALESCE(SUM(bk.NumberOfSeats), 0) / b.TotalSeats) * 100,
        2
    ) AS OccupancyPercentage,
    COALESCE(SUM(bk.NetAmount), 0) AS TotalRevenue
FROM Schedules s
INNER JOIN Buses b ON s.BusId = b.Id
INNER JOIN Routes r ON s.RouteId = r.Id
INNER JOIN Cities src ON r.SourceCityId = src.Id
INNER JOIN Cities dest ON r.DestinationCityId = dest.Id
LEFT JOIN Bookings bk
    ON bk.ScheduleId = s.Id
    AND bk.BookingStatus IN ('Confirmed', 'Completed')
GROUP BY
    s.Id,
    b.Id,
    b.BusNumber,
    b.TotalSeats,
    s.DepartureTime,
    s.ArrivalTime,
    r.SourceCityId,
    r.DestinationCityId,
    src.Name,
    dest.Name,
    bk.JourneyDate;

-- ============================================================================
-- STORED PROCEDURES
-- ============================================================================

-- ----------------------------------------------------------------------------
-- sp_SearchBuses: Full-text search with filters
-- ----------------------------------------------------------------------------
DELIMITER //

DROP PROCEDURE IF EXISTS sp_SearchBuses;
CREATE PROCEDURE sp_SearchBuses(
    IN p_sourceCityId        INT,
    IN p_destinationCityId   INT,
    IN p_journeyDate         DATE
)
BEGIN
    DECLARE v_dayOfWeek VARCHAR(10);
    SET v_dayOfWeek = LEFT(LOWER(DAYNAME(p_journeyDate)), 3);

    SELECT
        s.Id AS ScheduleId,
        b.Id AS BusId,
        b.BusNumber,
        b.TotalSeats,
        b.SeatLayoutType,
        b.IsAC,
        b.IsSleeper,
        bt.Name AS BusTypeName,
        op.CompanyName AS OperatorName,
        op.LogoUrl AS OperatorLogo,
        op.CommissionPercentage,
        r.SourceCityId,
        src.Name AS SourceCity,
        r.DestinationCityId,
        dest.Name AS DestinationCity,
        r.Distance,
        s.DepartureTime,
        s.ArrivalTime,
        s.DurationMinutes,
        s.BaseFare,
        s.PerKmRate,
        ROUND(
            s.BaseFare + (r.Distance * s.PerKmRate),
            2
        ) AS EstimatedFare,
        s.Frequency,
        s.DaysOfWeek,
        (SELECT COUNT(*)
         FROM SeatLayouts sl2
         WHERE sl2.BusId = b.Id AND sl2.IsActive = 1
        ) AS TotalSeatsConfigured,
        (SELECT COUNT(DISTINCT ba.AmenityId)
         FROM BusAmenities ba
         WHERE ba.BusId = b.Id AND ba.IsActive = 1
        ) AS AmenitiesCount
    FROM Schedules s
    INNER JOIN Routes r ON s.RouteId = r.Id
    INNER JOIN Cities src ON r.SourceCityId = src.Id
    INNER JOIN Cities dest ON r.DestinationCityId = dest.Id
    INNER JOIN Buses b ON s.BusId = b.Id AND b.IsActive = 1 AND b.IsDeleted = 0
    INNER JOIN Operators op ON b.OperatorId = op.Id AND op.IsActive = 1
    INNER JOIN BusTypes bt ON b.BusTypeId = bt.Id
    WHERE r.SourceCityId = p_sourceCityId
      AND r.DestinationCityId = p_destinationCityId
      AND r.IsActive = 1
      AND s.IsActive = 1
      AND (
          (s.Frequency = 'Daily')
          OR (s.Frequency = 'Weekly' AND FIND_IN_SET(v_dayOfWeek, LOWER(s.DaysOfWeek)) > 0)
          OR (s.Frequency = 'Custom'
              AND s.StartDate IS NOT NULL
              AND s.EndDate IS NOT NULL
              AND p_journeyDate BETWEEN s.StartDate AND s.EndDate
              AND FIND_IN_SET(v_dayOfWeek, LOWER(s.DaysOfWeek)) > 0)
      )
    ORDER BY s.DepartureTime;
END //

-- ----------------------------------------------------------------------------
-- sp_GetAvailableSeats: Get available seats for a schedule on a given date
-- ----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_GetAvailableSeats;
CREATE PROCEDURE sp_GetAvailableSeats(
    IN p_scheduleId  INT,
    IN p_journeyDate DATE
)
BEGIN
    SELECT
        sl.Id AS SeatId,
        sl.RowNumber,
        sl.ColumnNumber,
        sl.SeatNumber,
        sl.SeatPosition,
        sl.Deck,
        CASE
            WHEN bp.Id IS NOT NULL THEN 0
            ELSE 1
        END AS IsAvailable
    FROM Schedules s
    INNER JOIN Buses b ON s.BusId = b.Id
    INNER JOIN SeatLayouts sl ON sl.BusId = b.Id AND sl.IsActive = 1
    LEFT JOIN Bookings bk
        ON bk.ScheduleId = s.Id
        AND bk.JourneyDate = p_journeyDate
        AND bk.BookingStatus IN ('Confirmed', 'Pending')
    LEFT JOIN BookingPassengers bp
        ON bp.BookingId = bk.Id
        AND bp.SeatId = sl.Id
        AND bp.IsCancelled = 0
    WHERE s.Id = p_scheduleId
    ORDER BY sl.Deck, sl.RowNumber, sl.ColumnNumber;
END //

-- ----------------------------------------------------------------------------
-- sp_GenerateBookingNumber: Generate unique booking number
-- ----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_GenerateBookingNumber;
CREATE PROCEDURE sp_GenerateBookingNumber(
    OUT p_bookingNumber VARCHAR(20)
)
BEGIN
    DECLARE v_datePart VARCHAR(8);
    DECLARE v_randomPart VARCHAR(6);
    DECLARE v_attempts INT DEFAULT 0;
    DECLARE v_maxAttempts INT DEFAULT 10;

    SET v_datePart = DATE_FORMAT(NOW(), '%Y%m%d');

    -- Label the repeat loop so LEAVE can reference it
    generate_booking_number: REPEAT
        SET v_randomPart = LPAD(FLOOR(RAND() * 999999), 6, '0');
        SET p_bookingNumber = CONCAT('BT', v_datePart, v_randomPart);
        SET v_attempts = v_attempts + 1;

        IF NOT EXISTS (SELECT 1 FROM Bookings WHERE BookingNumber = p_bookingNumber) THEN
            LEAVE generate_booking_number;
        END IF;
    UNTIL v_attempts >= v_maxAttempts END REPEAT;

    IF v_attempts >= v_maxAttempts THEN
        -- Fallback: use UUID-based suffix
        SET p_bookingNumber = CONCAT('BT', v_datePart, '-', UPPER(SUBSTRING(MD5(RAND()), 1, 8)));
    END IF;
END //

-- ----------------------------------------------------------------------------
-- sp_CalculateFare: Calculate fare for a booking
-- ----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_CalculateFare;
CREATE PROCEDURE sp_CalculateFare(
    IN  p_scheduleId      INT,
    IN  p_sourceStopId    INT,
    IN  p_destStopId      INT,
    IN  p_numberOfSeats   INT,
    IN  p_couponCode      VARCHAR(50),
    OUT p_totalFare       DECIMAL(10,2),
    OUT p_discountAmount  DECIMAL(10,2),
    OUT p_convenienceFee  DECIMAL(10,2),
    OUT p_taxAmount       DECIMAL(10,2),
    OUT p_netAmount       DECIMAL(10,2)
)
BEGIN
    DECLARE v_baseFare DECIMAL(10,2);
    DECLARE v_perKmRate DECIMAL(5,2);
    DECLARE v_distance DECIMAL(10,2);
    DECLARE v_sourceMultiplier DECIMAL(5,2) DEFAULT 1.00;
    DECLARE v_destMultiplier DECIMAL(5,2) DEFAULT 1.00;
    DECLARE v_fareMultiplier DECIMAL(5,2);
    DECLARE v_couponDiscount DECIMAL(10,2) DEFAULT 0;
    DECLARE v_couponType VARCHAR(20);
    DECLARE v_couponValue DECIMAL(10,2);
    DECLARE v_maxDiscount DECIMAL(10,2);
    DECLARE v_minAmount DECIMAL(10,2);
    DECLARE v_baseCalculated DECIMAL(10,2);
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        SET p_totalFare = 0;
        SET p_discountAmount = 0;
        SET p_convenienceFee = 0;
        SET p_taxAmount = 0;
        SET p_netAmount = 0;
    END;

    -- Get schedule details
    SELECT BaseFare, PerKmRate
    INTO v_baseFare, v_perKmRate
    FROM Schedules
    WHERE Id = p_scheduleId AND IsActive = 1;

    -- Get distance between stops
    SELECT ABS(ds1.DistanceFromStart - ds2.DistanceFromStart)
    INTO v_distance
    FROM ScheduleStops ds1
    INNER JOIN ScheduleStops ds2 ON ds1.ScheduleId = ds2.ScheduleId
    WHERE ds1.ScheduleId = p_scheduleId
      AND ds1.StopId = p_sourceStopId
      AND ds2.StopId = p_destStopId;

    -- Get fare multiplier from source stop
    SELECT COALESCE(FareMultiplier, 1.00)
    INTO v_sourceMultiplier
    FROM ScheduleStops
    WHERE ScheduleId = p_scheduleId AND StopId = p_sourceStopId;

    -- Get fare multiplier from destination stop
    SELECT COALESCE(FareMultiplier, 1.00)
    INTO v_destMultiplier
    FROM ScheduleStops
    WHERE ScheduleId = p_scheduleId AND StopId = p_destStopId;

    SET v_fareMultiplier = GREATEST(v_sourceMultiplier, v_destMultiplier);

    -- Calculate base fare
    SET v_baseCalculated = (v_baseFare + COALESCE(v_distance, 0) * v_perKmRate) * v_fareMultiplier;

    -- Apply coupon if provided
    IF p_couponCode IS NOT NULL AND p_couponCode != '' THEN
        SELECT
            DiscountType,
            DiscountValue,
            COALESCE(MaxDiscountAmount, DiscountValue),
            MinBookingAmount
        INTO
            v_couponType,
            v_couponValue,
            v_maxDiscount,
            v_minAmount
        FROM Coupons
        WHERE Code = p_couponCode
          AND IsActive = 1
          AND (ValidFrom IS NULL OR ValidFrom <= NOW())
          AND (ValidTo IS NULL OR ValidTo >= NOW())
          AND (UsageLimit IS NULL OR UsedCount < UsageLimit);

        IF v_minAmount IS NULL OR v_baseCalculated >= v_minAmount THEN
            IF v_couponType = 'Percentage' THEN
                SET v_couponDiscount = LEAST(
                    (v_baseCalculated * v_couponValue / 100),
                    v_maxDiscount
                );
            ELSE
                SET v_couponDiscount = LEAST(v_couponValue, v_maxDiscount);
            END IF;
        END IF;
    END IF;

    -- Calculate total fare
    SET p_totalFare = v_baseCalculated * p_numberOfSeats;
    SET p_discountAmount = v_couponDiscount * p_numberOfSeats;

    -- Convenience fee: 2% of net fare after discount
    SET p_convenienceFee = ROUND((p_totalFare - p_discountAmount) * 0.02, 2);

    -- Tax: 5% of (total fare - discount + convenience fee)
    SET p_taxAmount = ROUND((p_totalFare - p_discountAmount + p_convenienceFee) * 0.05, 2);

    -- Net amount
    SET p_netAmount = p_totalFare - p_discountAmount + p_convenienceFee + p_taxAmount;
END //

DELIMITER ;

-- ============================================================================
-- END OF SCHEMA
-- ============================================================================
