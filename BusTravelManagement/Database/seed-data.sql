-- ============================================================
-- BusTravel Management System - Seed Data
-- ============================================================

-- ===== Roles =====
INSERT INTO Roles (Name, Description, IsActive, CreatedAt) VALUES
('SuperAdmin', 'Super administrator with full system access', 1, NOW()),
('Admin', 'Administrator with management access', 1, NOW()),
('Operator', 'Bus operator who manages buses and schedules', 1, NOW()),
('Customer', 'Regular customer who books tickets', 1, NOW());

-- ===== Permissions =====
INSERT INTO Permissions (Name, Description, Module, IsActive, CreatedAt) VALUES
('users.view', 'View users', 'Users', 1, NOW()),
('users.create', 'Create users', 'Users', 1, NOW()),
('users.edit', 'Edit users', 'Users', 1, NOW()),
('users.delete', 'Delete users', 'Users', 1, NOW()),
('operators.view', 'View operators', 'Operators', 1, NOW()),
('operators.approve', 'Approve operators', 'Operators', 1, NOW()),
('buses.view', 'View buses', 'Buses', 1, NOW()),
('buses.create', 'Create buses', 'Buses', 1, NOW()),
('buses.edit', 'Edit buses', 'Buses', 1, NOW()),
('buses.delete', 'Delete buses', 'Buses', 1, NOW()),
('schedules.view', 'View schedules', 'Schedules', 1, NOW()),
('schedules.create', 'Create schedules', 'Schedules', 1, NOW()),
('schedules.edit', 'Edit schedules', 'Schedules', 1, NOW()),
('bookings.view', 'View bookings', 'Bookings', 1, NOW()),
('bookings.cancel', 'Cancel bookings', 'Bookings', 1, NOW()),
('reports.view', 'View reports', 'Reports', 1, NOW()),
('coupons.manage', 'Manage coupons', 'Marketing', 1, NOW()),
('reviews.moderate', 'Moderate reviews', 'Reviews', 1, NOW()),
('settings.view', 'View settings', 'Settings', 1, NOW()),
('settings.edit', 'Edit settings', 'Settings', 1, NOW());

-- ===== Role-Permission Mapping =====
INSERT INTO RolePermissions (RoleId, PermissionId, CreatedAt)
SELECT r.Id, p.Id, NOW()
FROM Roles r, Permissions p
WHERE r.Name = 'SuperAdmin';

INSERT INTO RolePermissions (RoleId, PermissionId, CreatedAt)
SELECT r.Id, p.Id, NOW()
FROM Roles r, Permissions p
WHERE r.Name = 'Admin'
AND p.Name IN ('users.view', 'users.create', 'users.edit',
    'operators.view', 'operators.approve',
    'buses.view',
    'schedules.view',
    'bookings.view', 'bookings.cancel',
    'reports.view',
    'coupons.manage',
    'reviews.moderate',
    'settings.view', 'settings.edit');

INSERT INTO RolePermissions (RoleId, PermissionId, CreatedAt)
SELECT r.Id, p.Id, NOW()
FROM Roles r, Permissions p
WHERE r.Name = 'Operator'
AND p.Name IN ('buses.view', 'buses.create', 'buses.edit', 'buses.delete',
    'schedules.view', 'schedules.create', 'schedules.edit',
    'bookings.view',
    'reports.view');

INSERT INTO RolePermissions (RoleId, PermissionId, CreatedAt)
SELECT r.Id, p.Id, NOW()
FROM Roles r, Permissions p
WHERE r.Name = 'Customer'
AND p.Name IN ('bookings.view');

-- ===== Admin User (Password: Admin@123) =====
INSERT INTO Users (FirstName, LastName, Email, PasswordHash, PhoneNumber, IsEmailVerified, IsActive, IsLocked, CreatedAt)
VALUES ('Super', 'Admin', 'admin@bustravel.com',
    '6jgjlMjjFz9yfPMuwOaEFi5dFVrVLC6Mn75RCscMS7NHrDdkeVBCw/1j0+f9hT53', -- PBKDF2-SHA256, 100k iterations (Admin@123)
    '+91-9876543210', 1, 1, 0, NOW());

INSERT INTO UserRoles (UserId, RoleId, CreatedAt)
SELECT u.Id, r.Id, NOW()
FROM Users u, Roles r
WHERE u.Email = 'admin@bustravel.com' AND r.Name = 'SuperAdmin';

-- ===== Test Operator User (Password: Operator@123) =====
INSERT INTO Users (FirstName, LastName, Email, PasswordHash, PhoneNumber, IsEmailVerified, IsActive, IsLocked, CreatedAt)
VALUES ('Rajesh', 'Kumar', 'operator@bustravel.com',
    'AIyFfYNC6W2YwPJBxCk8HQ==.1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p',
    '+91-9876543211', 1, 1, 0, NOW());

INSERT INTO UserRoles (UserId, RoleId, CreatedAt)
SELECT u.Id, r.Id, NOW()
FROM Users u, Roles r
WHERE u.Email = 'operator@bustravel.com' AND r.Name = 'Operator';

-- ===== Test Customer User (Password: Customer@123) =====
INSERT INTO Users (FirstName, LastName, Email, PasswordHash, PhoneNumber, IsEmailVerified, IsActive, IsLocked, CreatedAt)
VALUES ('Amit', 'Sharma', 'customer@bustravel.com',
    'AIyFfYNC6W2YwPJBxCk8HQ==.1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p',
    '+91-9876543212', 1, 1, 0, NOW());

INSERT INTO UserRoles (UserId, RoleId, CreatedAt)
SELECT u.Id, r.Id, NOW()
FROM Users u, Roles r
WHERE u.Email = 'customer@bustravel.com' AND r.Name = 'Customer';

-- ===== Test Operator =====
INSERT INTO Operators (UserId, CompanyName, CompanyRegistrationNumber, ContactPerson, ContactEmail, ContactPhone, Address, City, State, Country, PostalCode, LogoUrl, CommissionPercentage, IsActive, IsVerified, CreatedAt)
SELECT u.Id, 'Rajdhani Express Travels', 'REG-OP-001', 'Rajesh Kumar', 'operator@bustravel.com', '+91-9876543211',
    '42, Transport Nagar', 'Delhi', 'Delhi', 'India', '110001', '/Content/images/default-operator.png',
    10.00, 1, 1, NOW()
FROM Users u WHERE u.Email = 'operator@bustravel.com';

-- ===== Second Operator =====
INSERT INTO Users (FirstName, LastName, Email, PasswordHash, PhoneNumber, IsEmailVerified, IsActive, IsLocked, CreatedAt)
VALUES ('Suresh', 'Patel', 'suresh@bustravel.com',
    'AIyFfYNC6W2YwPJBxCk8HQ==.1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p',
    '+91-9876543213', 1, 1, 0, NOW());

INSERT INTO UserRoles (UserId, RoleId, CreatedAt)
SELECT u.Id, r.Id, NOW()
FROM Users u, Roles r
WHERE u.Email = 'suresh@bustravel.com' AND r.Name = 'Operator';

INSERT INTO Operators (UserId, CompanyName, CompanyRegistrationNumber, ContactPerson, ContactEmail, ContactPhone, Address, City, State, Country, PostalCode, CommissionPercentage, IsActive, IsVerified, CreatedAt)
SELECT u.Id, 'GreenLine Express', 'REG-OP-002', 'Suresh Patel', 'suresh@bustravel.com', '+91-9876543213',
    '78, Bus Terminal Road', 'Mumbai', 'Maharashtra', 'India', '400001',
    12.00, 1, 1, NOW()
FROM Users u WHERE u.Email = 'suresh@bustravel.com';

-- ===== Bus Types =====
INSERT INTO BusTypes (Name, Description, IsActive, CreatedAt) VALUES
('AC Sleeper', 'Air conditioned sleeper bus with comfortable berths', 1, NOW()),
('AC Semi-Sleeper', 'Air conditioned bus with reclining seats', 1, NOW()),
('Non-AC Sleeper', 'Non air conditioned sleeper bus', 1, NOW()),
('Non-AC Seater', 'Standard non air conditioned seater bus', 1, NOW()),
('Volvo AC', 'Volvo multi-axle air conditioned bus', 1, NOW()),
('AC Seater', 'Air conditioned seater bus with pushback seats', 1, NOW()),
('Mini Bus', 'Small capacity bus for short routes', 1, NOW()),
('Electric AC', 'Eco-friendly electric air conditioned bus', 1, NOW());

-- ===== Amenities =====
INSERT INTO Amenities (Name, IconClass, Description, IsActive, CreatedAt) VALUES
('Charging Point', 'bi-plug', 'Mobile/USB charging at every seat', 1, NOW()),
('WiFi', 'bi-wifi', 'Onboard WiFi internet connectivity', 1, NOW()),
('Blanket', 'bi-snow', 'Blanket provided for sleeper buses', 1, NOW()),
('Water Bottle', 'bi-droplet', 'Complimentary drinking water', 1, NOW()),
('Reading Light', 'bi-lightbulb', 'Individual reading lights', 1, NOW()),
('Emergency Exit', 'bi-shield', 'Emergency exit and safety equipment', 1, NOW()),
('First Aid', 'bi-heart-pulse', 'First aid kit available', 1, NOW()),
('GPS Tracking', 'bi-geo-alt', 'Real time GPS tracking', 1, NOW()),
('Movie', 'bi-tv', 'Entertainment system with movies', 1, NOW()),
('Snacks', 'bi-cup-hot', 'Complimentary snacks and beverages', 1, NOW()),
('Toilet', 'bi-building', 'Onboard restroom facility', 1, NOW()),
('CCTV', 'bi-camera-video', 'CCTV surveillance', 1, NOW()),
('Hand Sanitizer', 'bi-hand-thumbs-up', 'Sanitizer dispenser available', 1, NOW()),
('Pillow', 'bi-emoji-sunglasses', 'Travel pillow provided', 1, NOW()),
('Curtains', 'bi-eye-slash', 'Privacy curtains', 1, NOW());

-- ===== Cities =====
INSERT INTO Cities (Name, State, Country, Latitude, Longitude, IsActive, IsPopular, CreatedAt) VALUES
('Delhi', 'Delhi', 'India', 28.704060, 77.102493, 1, 1, NOW()),
('Mumbai', 'Maharashtra', 'India', 19.076090, 72.877426, 1, 1, NOW()),
('Bangalore', 'Karnataka', 'India', 12.971599, 77.594566, 1, 1, NOW()),
('Chennai', 'Tamil Nadu', 'India', 13.082680, 80.270718, 1, 1, NOW()),
('Kolkata', 'West Bengal', 'India', 22.572645, 88.363892, 1, 1, NOW()),
('Hyderabad', 'Telangana', 'India', 17.385044, 78.486671, 1, 1, NOW()),
('Pune', 'Maharashtra', 'India', 18.520430, 73.856744, 1, 1, NOW()),
('Ahmedabad', 'Gujarat', 'India', 23.022505, 72.571365, 1, 1, NOW()),
('Jaipur', 'Rajasthan', 'India', 26.912434, 75.787270, 1, 1, NOW()),
('Lucknow', 'Uttar Pradesh', 'India', 26.846694, 80.946167, 1, 1, NOW()),
('Chandigarh', 'Chandigarh', 'India', 30.733315, 76.779419, 1, 1, NOW()),
('Indore', 'Madhya Pradesh', 'India', 22.719568, 75.857727, 1, 0, NOW()),
('Bhopal', 'Madhya Pradesh', 'India', 23.259933, 77.412613, 1, 0, NOW()),
('Agra', 'Uttar Pradesh', 'India', 27.176670, 78.008072, 1, 0, NOW()),
('Varanasi', 'Uttar Pradesh', 'India', 25.317645, 82.973915, 1, 0, NOW()),
('Surat', 'Gujarat', 'India', 21.170240, 72.831062, 1, 0, NOW()),
('Nagpur', 'Maharashtra', 'India', 21.145800, 79.088155, 1, 0, NOW()),
('Patna', 'Bihar', 'India', 25.594095, 85.137566, 1, 0, NOW()),
('Bhubaneswar', 'Odisha', 'India', 20.296059, 85.824539, 1, 0, NOW()),
('Coimbatore', 'Tamil Nadu', 'India', 11.016844, 76.955833, 1, 0, NOW()),
('Gurgaon', 'Haryana', 'India', 28.459497, 77.026634, 1, 0, NOW()),
('Manesar', 'Haryana', 'India', 28.350000, 76.933300, 1, 0, NOW()),
('Neemrana', 'Rajasthan', 'India', 27.980000, 76.880000, 1, 0, NOW()),
('Shahpura', 'Rajasthan', 'India', 27.391300, 75.958700, 1, 0, NOW()),
('Chomu', 'Rajasthan', 'India', 27.173100, 75.721900, 1, 0, NOW()),
('Ajmer', 'Rajasthan', 'India', 26.449923, 74.638916, 1, 0, NOW()),
('Vadodara', 'Gujarat', 'India', 22.307159, 73.181221, 1, 0, NOW()),
('Panvel', 'Maharashtra', 'India', 19.000000, 73.100000, 1, 0, NOW()),
('Lonavala', 'Maharashtra', 'India', 18.756200, 73.407800, 1, 0, NOW()),
('Panipat', 'Haryana', 'India', 29.388100, 76.968900, 1, 0, NOW()),
('Karnal', 'Haryana', 'India', 29.688700, 76.989800, 1, 0, NOW()),
('Ambala', 'Haryana', 'India', 30.378200, 76.777700, 1, 0, NOW()),
('Kurnool', 'Andhra Pradesh', 'India', 15.828100, 78.037300, 1, 0, NOW()),
('Anantapur', 'Andhra Pradesh', 'India', 14.681900, 77.600600, 1, 0, NOW()),
('Gwalior', 'Madhya Pradesh', 'India', 26.218300, 78.182800, 1, 0, NOW()),
('Jhansi', 'Uttar Pradesh', 'India', 25.448400, 78.568500, 1, 0, NOW());

-- ===== Routes =====
INSERT INTO Routes (SourceCityId, DestinationCityId, Distance, DurationMinutes, IsActive, CreatedAt)
SELECT s.Id, d.Id, 260, 360, 1, NOW() FROM Cities s, Cities d WHERE s.Name='Delhi' AND d.Name='Jaipur';

INSERT INTO Routes (SourceCityId, DestinationCityId, Distance, DurationMinutes, IsActive, CreatedAt)
SELECT s.Id, d.Id, 1400, 1080, 1, NOW() FROM Cities s, Cities d WHERE s.Name='Delhi' AND d.Name='Mumbai';

INSERT INTO Routes (SourceCityId, DestinationCityId, Distance, DurationMinutes, IsActive, CreatedAt)
SELECT s.Id, d.Id, 2100, 1440, 1, NOW() FROM Cities s, Cities d WHERE s.Name='Delhi' AND d.Name='Bangalore';

INSERT INTO Routes (SourceCityId, DestinationCityId, Distance, DurationMinutes, IsActive, CreatedAt)
SELECT s.Id, d.Id, 500, 480, 1, NOW() FROM Cities s, Cities d WHERE s.Name='Mumbai' AND d.Name='Pune';

INSERT INTO Routes (SourceCityId, DestinationCityId, Distance, DurationMinutes, IsActive, CreatedAt)
SELECT s.Id, d.Id, 710, 600, 1, NOW() FROM Cities s, Cities d WHERE s.Name='Mumbai' AND d.Name='Ahmedabad';

INSERT INTO Routes (SourceCityId, DestinationCityId, Distance, DurationMinutes, IsActive, CreatedAt)
SELECT s.Id, d.Id, 560, 540, 1, NOW() FROM Cities s, Cities d WHERE s.Name='Bangalore' AND d.Name='Chennai';

INSERT INTO Routes (SourceCityId, DestinationCityId, Distance, DurationMinutes, IsActive, CreatedAt)
SELECT s.Id, d.Id, 1550, 1200, 1, NOW() FROM Cities s, Cities d WHERE s.Name='Kolkata' AND d.Name='Delhi';

INSERT INTO Routes (SourceCityId, DestinationCityId, Distance, DurationMinutes, IsActive, CreatedAt)
SELECT s.Id, d.Id, 1500, 1140, 1, NOW() FROM Cities s, Cities d WHERE s.Name='Delhi' AND d.Name='Hyderabad';

INSERT INTO Routes (SourceCityId, DestinationCityId, Distance, DurationMinutes, IsActive, CreatedAt)
SELECT s.Id, d.Id, 520, 420, 1, NOW() FROM Cities s, Cities d WHERE s.Name='Delhi' AND d.Name='Chandigarh';

INSERT INTO Routes (SourceCityId, DestinationCityId, Distance, DurationMinutes, IsActive, CreatedAt)
SELECT s.Id, d.Id, 550, 480, 1, NOW() FROM Cities s, Cities d WHERE s.Name='Hyderabad' AND d.Name='Bangalore';

-- ===== Stops for Routes =====
-- Delhi → Jaipur route
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'ISBT Kashmere Gate', 1, '06:00:00', 'ISBT Kashmere Gate, Delhi', 1
FROM Routes r, Cities c WHERE c.Name='Delhi'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Jaipur');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Dhaula Kuan', 2, '06:30:00', 'Dhaula Kuan Bus Stop, Delhi', 1
FROM Routes r, Cities c WHERE c.Name='Delhi'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Jaipur');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Gurgaon', 3, '07:00:00', 'Gurgaon Bus Stand', 1
FROM Routes r, Cities c WHERE c.Name='Gurgaon'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Jaipur');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Manesar', 4, '07:30:00', 'Manesar Highway Stop', 1
FROM Routes r, Cities c WHERE c.Name='Manesar'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Jaipur');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Neemrana', 5, '08:00:00', 'Neemrana Bus Stop', 1
FROM Routes r, Cities c WHERE c.Name='Neemrana'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Jaipur');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Shahpura', 6, '08:30:00', 'Shahpura Bus Stand, Jaipur', 1
FROM Routes r, Cities c WHERE c.Name='Shahpura'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Jaipur');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Chomu', 7, '09:00:00', 'Chomu Bus Stop, Jaipur', 1
FROM Routes r, Cities c WHERE c.Name='Chomu'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Jaipur');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Sindhi Camp', 8, '10:00:00', 'Sindhi Camp Bus Stand, Jaipur', 1
FROM Routes r, Cities c WHERE c.Name='Jaipur'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Jaipur');

-- Delhi → Mumbai route
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'ISBT Kashmere Gate', 1, '06:00:00', 'ISBT Kashmere Gate, Delhi', 1
FROM Routes r, Cities c WHERE c.Name='Delhi'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Mumbai');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Dhaula Kuan', 2, '06:30:00', 'Dhaula Kuan Bus Stop, Delhi', 1
FROM Routes r, Cities c WHERE c.Name='Delhi'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Mumbai');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Gurgaon', 3, '07:00:00', 'Gurgaon Bus Stand', 1
FROM Routes r, Cities c WHERE c.Name='Gurgaon'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Mumbai');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Jaipur', 4, '09:00:00', 'Jaipur Bypass', 1
FROM Routes r, Cities c WHERE c.Name='Jaipur'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Mumbai');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Ajmer', 5, '11:00:00', 'Ajmer Bus Stand', 1
FROM Routes r, Cities c WHERE c.Name='Ajmer'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Mumbai');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Ahmedabad', 6, '14:00:00', 'Ahmedabad Bus Stand', 1
FROM Routes r, Cities c WHERE c.Name='Ahmedabad'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Mumbai');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Vadodara', 7, '15:00:00', 'Vadodara Central Bus Stand', 1
FROM Routes r, Cities c WHERE c.Name='Vadodara'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Mumbai');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Surat', 8, '16:00:00', 'Surat Bus Terminal', 1
FROM Routes r, Cities c WHERE c.Name='Surat'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Mumbai');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Mumbai', 9, '18:00:00', 'Mumbai Central Bus Terminal', 1
FROM Routes r, Cities c WHERE c.Name='Mumbai'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Mumbai');

-- Mumbai → Pune route
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Mumbai Central', 1, '06:00:00', 'Mumbai Central Bus Terminal', 1
FROM Routes r, Cities c WHERE c.Name='Mumbai'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Mumbai' AND dc.Name='Pune');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Panvel', 2, '06:30:00', 'Panvel Bus Stand', 1
FROM Routes r, Cities c WHERE c.Name='Panvel'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Mumbai' AND dc.Name='Pune');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Lonavala', 3, '07:30:00', 'Lonavala Bus Stand', 1
FROM Routes r, Cities c WHERE c.Name='Lonavala'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Mumbai' AND dc.Name='Pune');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Pune', 4, '09:00:00', 'Pune Station Bus Stand', 1
FROM Routes r, Cities c WHERE c.Name='Pune'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Mumbai' AND dc.Name='Pune');

-- Mumbai → Ahmedabad route
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Mumbai Central', 1, '06:00:00', 'Mumbai Central Bus Terminal', 1
FROM Routes r, Cities c WHERE c.Name='Mumbai'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Mumbai' AND dc.Name='Ahmedabad');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Surat', 2, '08:00:00', 'Surat Bus Terminal', 1
FROM Routes r, Cities c WHERE c.Name='Surat'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Mumbai' AND dc.Name='Ahmedabad');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Vadodara', 3, '09:30:00', 'Vadodara Central Bus Stand', 1
FROM Routes r, Cities c WHERE c.Name='Vadodara'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Mumbai' AND dc.Name='Ahmedabad');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Ahmedabad', 4, '12:00:00', 'Ahmedabad Bus Stand', 1
FROM Routes r, Cities c WHERE c.Name='Ahmedabad'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Mumbai' AND dc.Name='Ahmedabad');

-- Delhi → Hyderabad route
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'ISBT Kashmere Gate', 1, '06:00:00', 'ISBT Kashmere Gate, Delhi', 1
FROM Routes r, Cities c WHERE c.Name='Delhi'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Hyderabad');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Agra', 2, '08:00:00', 'Agra Bus Stand', 1
FROM Routes r, Cities c WHERE c.Name='Agra'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Hyderabad');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Gwalior', 3, '09:30:00', 'Gwalior Bus Stand', 1
FROM Routes r, Cities c WHERE c.Name='Gwalior'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Hyderabad');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Jhansi', 4, '11:00:00', 'Jhansi Bus Stand', 1
FROM Routes r, Cities c WHERE c.Name='Jhansi'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Hyderabad');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Nagpur', 5, '15:00:00', 'Nagpur Bus Station', 1
FROM Routes r, Cities c WHERE c.Name='Nagpur'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Hyderabad');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Hyderabad', 6, '18:00:00', 'Hyderabad MGBS Bus Stand', 1
FROM Routes r, Cities c WHERE c.Name='Hyderabad'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Hyderabad');

-- Delhi → Chandigarh route
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'ISBT Kashmere Gate', 1, '06:00:00', 'ISBT Kashmere Gate, Delhi', 1
FROM Routes r, Cities c WHERE c.Name='Delhi'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Chandigarh');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Panipat', 2, '07:00:00', 'Panipat Bus Stand', 1
FROM Routes r, Cities c WHERE c.Name='Panipat'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Chandigarh');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Karnal', 3, '07:30:00', 'Karnal Bus Stand', 1
FROM Routes r, Cities c WHERE c.Name='Karnal'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Chandigarh');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Ambala', 4, '08:30:00', 'Ambala Bus Stand', 1
FROM Routes r, Cities c WHERE c.Name='Ambala'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Chandigarh');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Chandigarh', 5, '10:00:00', 'Chandigarh ISBT Sector 43', 1
FROM Routes r, Cities c WHERE c.Name='Chandigarh'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Chandigarh');

-- Hyderabad → Bangalore route
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Hyderabad MGBS', 1, '06:00:00', 'Hyderabad MGBS Bus Stand', 1
FROM Routes r, Cities c WHERE c.Name='Hyderabad'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Hyderabad' AND dc.Name='Bangalore');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Kurnool', 2, '08:00:00', 'Kurnool Bus Stand', 1
FROM Routes r, Cities c WHERE c.Name='Kurnool'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Hyderabad' AND dc.Name='Bangalore');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Anantapur', 3, '09:30:00', 'Anantapur Bus Stand', 1
FROM Routes r, Cities c WHERE c.Name='Anantapur'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Hyderabad' AND dc.Name='Bangalore');
INSERT INTO Stops (RouteId, CityId, StopName, StopOrder, StopTime, Address, IsActive)
SELECT r.Id, c.Id, 'Bangalore', 4, '12:00:00', 'Bangalore Kempegowda Bus Station', 1
FROM Routes r, Cities c WHERE c.Name='Bangalore'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Hyderabad' AND dc.Name='Bangalore');

-- ===== Buses =====
INSERT INTO Buses (OperatorId, BusTypeId, BusNumber, RegistrationNumber, TotalSeats, SeatLayoutType, IsAC, IsSleeper, IsSingleAxle, IsActive, CreatedAt)
SELECT o.Id, bt.Id, 'RJ-01-AB-1234', 'DL-01-AB-2024-001', 40, '2x2', 1, 0, 1, 1, NOW()
FROM Operators o, BusTypes bt WHERE o.CompanyName='Rajdhani Express Travels' AND bt.Name='AC Semi-Sleeper';

INSERT INTO Buses (OperatorId, BusTypeId, BusNumber, RegistrationNumber, TotalSeats, SeatLayoutType, IsAC, IsSleeper, IsSingleAxle, IsActive, CreatedAt)
SELECT o.Id, bt.Id, 'RJ-01-CD-5678', 'DL-01-AB-2024-002', 36, 'Sleeper', 1, 1, 1, 1, NOW()
FROM Operators o, BusTypes bt WHERE o.CompanyName='Rajdhani Express Travels' AND bt.Name='AC Sleeper';

INSERT INTO Buses (OperatorId, BusTypeId, BusNumber, RegistrationNumber, TotalSeats, SeatLayoutType, IsAC, IsSleeper, IsSingleAxle, IsActive, CreatedAt)
SELECT o.Id, bt.Id, 'RJ-01-EF-9012', 'DL-01-AB-2024-003', 50, '2x3', 0, 0, 1, 1, NOW()
FROM Operators o, BusTypes bt WHERE o.CompanyName='Rajdhani Express Travels' AND bt.Name='Non-AC Seater';

INSERT INTO Buses (OperatorId, BusTypeId, BusNumber, RegistrationNumber, TotalSeats, SeatLayoutType, IsAC, IsSleeper, IsSingleAxle, IsActive, CreatedAt)
SELECT o.Id, bt.Id, 'GL-02-GH-3456', 'MH-02-AB-2024-001', 40, '2x2', 1, 0, 0, 1, NOW()
FROM Operators o, BusTypes bt WHERE o.CompanyName='GreenLine Express' AND bt.Name='Volvo AC';

INSERT INTO Buses (OperatorId, BusTypeId, BusNumber, RegistrationNumber, TotalSeats, SeatLayoutType, IsAC, IsSleeper, IsSingleAxle, IsActive, CreatedAt)
SELECT o.Id, bt.Id, 'GL-02-IJ-7890', 'MH-02-AB-2024-002', 32, 'Sleeper', 1, 1, 1, 1, NOW()
FROM Operators o, BusTypes bt WHERE o.CompanyName='GreenLine Express' AND bt.Name='AC Sleeper';

-- ===== Bus Amenities =====
INSERT INTO BusAmenities (BusId, AmenityId, IsActive, CreatedAt)
SELECT b.Id, a.Id, 1, NOW()
FROM Buses b, Amenities a
WHERE b.BusNumber = 'RJ-01-AB-1234' AND a.Name IN ('Charging Point', 'WiFi', 'Reading Light', 'GPS Tracking', 'CCTV', 'Hand Sanitizer', 'Curtains');

INSERT INTO BusAmenities (BusId, AmenityId, IsActive, CreatedAt)
SELECT b.Id, a.Id, 1, NOW()
FROM Buses b, Amenities a
WHERE b.BusNumber = 'RJ-01-CD-5678' AND a.Name IN ('Charging Point', 'Blanket', 'Reading Light', 'Emergency Exit', 'First Aid', 'GPS Tracking', 'CCTV', 'Hand Sanitizer', 'Pillow', 'Curtains');

INSERT INTO BusAmenities (BusId, AmenityId, IsActive, CreatedAt)
SELECT b.Id, a.Id, 1, NOW()
FROM Buses b, Amenities a
WHERE b.BusNumber = 'GL-02-GH-3456' AND a.Name IN ('Charging Point', 'WiFi', 'Blanket', 'Water Bottle', 'Reading Light', 'First Aid', 'GPS Tracking', 'Movie', 'Snacks', 'CCTV', 'Hand Sanitizer', 'Pillow', 'Curtains');

-- ===== Schedules =====
INSERT INTO Schedules (BusId, RouteId, DepartureTime, ArrivalTime, DurationMinutes, Frequency, DaysOfWeek, BaseFare, PerKmRate, IsActive, IsRecurring, CreatedAt)
SELECT b.Id, r.Id, '22:00:00', '04:00:00', 360, 'Daily', 'Mon,Tue,Wed,Thu,Fri,Sat,Sun', 500, 2.00, 1, 1, NOW()
FROM Buses b, Routes r
WHERE b.BusNumber = 'RJ-01-AB-1234'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Jaipur');

INSERT INTO Schedules (BusId, RouteId, DepartureTime, ArrivalTime, DurationMinutes, Frequency, DaysOfWeek, BaseFare, PerKmRate, IsActive, IsRecurring, CreatedAt)
SELECT b.Id, r.Id, '20:00:00', '14:00:00', 1080, 'Daily', 'Mon,Tue,Wed,Thu,Fri,Sat,Sun', 1200, 1.50, 1, 1, NOW()
FROM Buses b, Routes r
WHERE b.BusNumber = 'RJ-01-CD-5678'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Delhi' AND dc.Name='Mumbai');

INSERT INTO Schedules (BusId, RouteId, DepartureTime, ArrivalTime, DurationMinutes, Frequency, DaysOfWeek, BaseFare, PerKmRate, IsActive, IsRecurring, CreatedAt)
SELECT b.Id, r.Id, '06:00:00', '12:00:00', 360, 'Daily', 'Mon,Tue,Wed,Thu,Fri,Sat,Sun', 350, 1.50, 1, 1, NOW()
FROM Buses b, Routes r
WHERE b.BusNumber = 'GL-02-GH-3456'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Mumbai' AND dc.Name='Pune');

INSERT INTO Schedules (BusId, RouteId, DepartureTime, ArrivalTime, DurationMinutes, Frequency, DaysOfWeek, BaseFare, PerKmRate, IsActive, IsRecurring, CreatedAt)
SELECT b.Id, r.Id, '21:00:00', '07:00:00', 600, 'Daily', 'Mon,Tue,Wed,Thu,Fri,Sat,Sun', 800, 2.00, 1, 1, NOW()
FROM Buses b, Routes r
WHERE b.BusNumber = 'RJ-01-AB-1234'
AND r.Id = (SELECT r2.Id FROM Routes r2 JOIN Cities sc ON r2.SourceCityId=sc.Id JOIN Cities dc ON r2.DestinationCityId=dc.Id WHERE sc.Name='Mumbai' AND dc.Name='Ahmedabad');

-- ===== Seat Layouts =====
-- Generate seats for bus RJ-01-AB-1234 (40 seats, 2x2 layout, 10 rows)
INSERT INTO SeatLayouts (BusId, RowNumber, ColumnNumber, SeatNumber, SeatPosition, Deck, IsActive, CreatedAt)
SELECT b.Id, row_num, col_num,
    CONCAT(CHAR(65 + row_num - 1), col_num + 1),
    CASE WHEN col_num IN (0, 3) THEN 'Window' WHEN col_num IN (1, 2) THEN 'Aisle' ELSE 'Middle' END,
    'Lower', 1, NOW()
FROM Buses b
CROSS JOIN (
    SELECT 1 AS row_num UNION SELECT 2 UNION SELECT 3 UNION SELECT 4 UNION SELECT 5
    UNION SELECT 6 UNION SELECT 7 UNION SELECT 8 UNION SELECT 9 UNION SELECT 10
) row_nums
CROSS JOIN (
    SELECT 0 AS col_num UNION SELECT 1 UNION SELECT 2 UNION SELECT 3
) col_nums
WHERE b.BusNumber = 'RJ-01-AB-1234'
AND NOT (row_num = 10 AND col_num IN (2, 3)); -- Last row has only 2 seats due to door

-- ===== Boarding Points =====
INSERT INTO BoardingPoints (ScheduleId, Name, Address, PickupTime, IsActive, CreatedAt)
SELECT s.Id, 'ISBT Kashmere Gate', 'ISBT Bus Terminal, Kashmere Gate, Delhi', '21:30:00', 1, NOW()
FROM Schedules s JOIN Buses b ON s.BusId=b.Id WHERE b.BusNumber='RJ-01-AB-1234';

INSERT INTO BoardingPoints (ScheduleId, Name, Address, PickupTime, IsActive, CreatedAt)
SELECT s.Id, 'Karol Bagh', 'Bus Stand, Karol Bagh, Delhi', '21:45:00', 1, NOW()
FROM Schedules s JOIN Buses b ON s.BusId=b.Id WHERE b.BusNumber='RJ-01-AB-1234';

INSERT INTO BoardingPoints (ScheduleId, Name, Address, PickupTime, IsActive, CreatedAt)
SELECT s.Id, 'Dhaula Kuan', 'Dhaula Kuan Bus Stop, Delhi', '22:15:00', 1, NOW()
FROM Schedules s JOIN Buses b ON s.BusId=b.Id WHERE b.BusNumber='RJ-01-AB-1234';

-- ===== Dropping Points =====
INSERT INTO DroppingPoints (ScheduleId, Name, Address, DropTime, IsActive, CreatedAt)
SELECT s.Id, 'Sindhi Camp', 'Sindhi Camp Bus Stand, Jaipur', '04:00:00', 1, NOW()
FROM Schedules s JOIN Buses b ON s.BusId=b.Id WHERE b.BusNumber='RJ-01-AB-1234';

INSERT INTO DroppingPoints (ScheduleId, Name, Address, DropTime, IsActive, CreatedAt)
SELECT s.Id, 'Gandhi Nagar', 'Gandhi Nagar Bus Stand, Jaipur', '04:15:00', 1, NOW()
FROM Schedules s JOIN Buses b ON s.BusId=b.Id WHERE b.BusNumber='RJ-01-AB-1234';

-- ===== Coupons =====
INSERT INTO Coupons (Code, Description, DiscountType, DiscountValue, MinBookingAmount, MaxDiscountAmount, UsageLimit, PerUserLimit, IsActive, ValidFrom, ValidTo, CreatedAt)
VALUES ('WELCOME50', 'Welcome discount for new users - 50% off up to ₹200', 'Percentage', 50.00, 500, 200.00, 1000, 1, 1, NOW(), DATE_ADD(NOW(), INTERVAL 6 MONTH), NOW());

INSERT INTO Coupons (Code, Description, DiscountType, DiscountValue, MinBookingAmount, MaxDiscountAmount, UsageLimit, PerUserLimit, IsActive, ValidFrom, ValidTo, CreatedAt)
VALUES ('FLAT200', 'Flat ₹200 off on bookings above ₹1000', 'Flat', 200.00, 1000, 200.00, 500, 1, 1, NOW(), DATE_ADD(NOW(), INTERVAL 3 MONTH), NOW());

INSERT INTO Coupons (Code, Description, DiscountType, DiscountValue, MinBookingAmount, MaxDiscountAmount, UsageLimit, PerUserLimit, IsActive, ValidFrom, ValidTo, CreatedAt)
VALUES ('BUS25', '25% discount on all bus bookings', 'Percentage', 25.00, 300, 150.00, 2000, 3, 1, NOW(), DATE_ADD(NOW(), INTERVAL 1 MONTH), NOW());

-- ===== Cancellation Policies =====
INSERT INTO CancellationPolicies (OperatorId, HoursBeforeDeparture, RefundPercentage, IsActive, CreatedAt)
SELECT o.Id, 48, 90, 1, NOW() FROM Operators o WHERE o.CompanyName='Rajdhani Express Travels';

INSERT INTO CancellationPolicies (OperatorId, HoursBeforeDeparture, RefundPercentage, IsActive, CreatedAt)
SELECT o.Id, 24, 75, 1, NOW() FROM Operators o WHERE o.CompanyName='Rajdhani Express Travels';

INSERT INTO CancellationPolicies (OperatorId, HoursBeforeDeparture, RefundPercentage, IsActive, CreatedAt)
SELECT o.Id, 12, 50, 1, NOW() FROM Operators o WHERE o.CompanyName='Rajdhani Express Travels';

INSERT INTO CancellationPolicies (OperatorId, HoursBeforeDeparture, RefundPercentage, IsActive, CreatedAt)
SELECT o.Id, 48, 85, 1, NOW() FROM Operators o WHERE o.CompanyName='GreenLine Express';

INSERT INTO CancellationPolicies (OperatorId, HoursBeforeDeparture, RefundPercentage, IsActive, CreatedAt)
SELECT o.Id, 24, 70, 1, NOW() FROM Operators o WHERE o.CompanyName='GreenLine Express';

-- ===== Sample Bookings =====
INSERT INTO Bookings (BookingNumber, UserId, ScheduleId, JourneyDate, SourceStopId, DestinationStopId,
    BoardingPointId, DroppingPointId, BookingStatus, NumberOfSeats, TotalFare, ConvenienceFee,
    TaxAmount, NetAmount, ContactName, ContactPhone, ContactEmail, CreatedAt)
SELECT 'BT202607300001', u.Id, s.Id, DATE_ADD(CURDATE(), INTERVAL 3 DAY),
    (SELECT Id FROM Stops WHERE RouteId = s.RouteId ORDER BY StopOrder LIMIT 1),
    (SELECT Id FROM Stops WHERE RouteId = s.RouteId ORDER BY StopOrder DESC LIMIT 1),
    (SELECT Id FROM BoardingPoints WHERE ScheduleId = s.Id LIMIT 1),
    (SELECT Id FROM DroppingPoints WHERE ScheduleId = s.Id LIMIT 1),
    'Confirmed', 2, 1000.00, 20.00, 50.00, 1070.00,
    'Amit Sharma', '+91-9876543212', 'customer@bustravel.com', NOW()
FROM Users u, Schedules s
WHERE u.Email = 'customer@bustravel.com'
AND s.Id = (SELECT s2.Id FROM Schedules s2 JOIN Buses b ON s2.BusId=b.Id WHERE b.BusNumber='RJ-01-AB-1234' LIMIT 1);

-- ===== Sample Payment =====
INSERT INTO Payments (BookingId, PaymentReference, PaymentMethod, PaymentGateway, GatewayTransactionId, Amount, Currency, PaymentStatus, PaidAt, CreatedAt)
SELECT b.Id, 'PAY202607300001', 'Credit Card', 'Mock', 'MOCK-TXN-001', b.NetAmount, 'INR', 'Success', NOW(), NOW()
FROM Bookings b WHERE b.BookingNumber = 'BT202607300001';

-- ===== Sample Ticket =====
INSERT INTO Tickets (BookingId, TicketNumber, QRCodeData, CancellationPolicy, GeneratedAt, CreatedAt)
SELECT b.Id, 'TKT20260730A1B2C3D4',
    CONCAT('QR|', b.BookingNumber, '|', b.ScheduleId, '|', b.JourneyDate),
    CONCAT('Cancellation Policy: 75% refund if cancelled 24hrs before departure'), NOW(), NOW()
FROM Bookings b WHERE b.BookingNumber = 'BT202607300001';
