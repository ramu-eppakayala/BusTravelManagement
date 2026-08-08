using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BusTravelManagement.Models.Entities;
using BusTravelManagement.Models.ViewModels;
using BusTravelManagement.Repositories.Interfaces;
using BusTravelManagement.Services.Interfaces;

namespace BusTravelManagement.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public BookingResultViewModel InitiateBooking(BookingRequestViewModel model)
        {
            var result = new BookingResultViewModel
            {
                Success = false,
                Errors = new List<string>()
            };

            try
            {
                // Validate schedule
                var schedule = _unitOfWork.Query<Schedule>()
                    .Include(s => s.Bus)
                    .Include(s => s.Bus.Operator)
                    .FirstOrDefault(s => s.Id == model.ScheduleId);

                if (schedule == null || !schedule.IsActive)
                {
                    result.Errors.Add("Schedule not found or inactive.");
                    return result;
                }

                // Validate boarding and dropping points
                var boardingPoint = _unitOfWork.Query<BoardingPoint>()
                    .FirstOrDefault(bp => bp.Id == model.BoardingPointId && bp.ScheduleId == model.ScheduleId);
                if (boardingPoint == null)
                {
                    result.Errors.Add("Invalid boarding point.");
                    return result;
                }

                var droppingPoint = _unitOfWork.Query<DroppingPoint>()
                    .FirstOrDefault(dp => dp.Id == model.DroppingPointId && dp.ScheduleId == model.ScheduleId);
                if (droppingPoint == null)
                {
                    result.Errors.Add("Invalid dropping point.");
                    return result;
                }

                // Validate seat availability
                var seatLayouts = _unitOfWork.Query<SeatLayout>()
                    .Where(sl => model.SeatLayoutIds.Contains(sl.Id) && sl.IsActive)
                    .ToList();

                if (seatLayouts.Count != model.SeatLayoutIds.Count)
                {
                    result.Errors.Add("One or more selected seats are invalid.");
                    return result;
                }

                // Check if seats are already booked or on hold
                foreach (var seatLayoutId in model.SeatLayoutIds)
                {
                    var existingSeat = _unitOfWork.Query<Seat>()
                        .FirstOrDefault(s => s.SeatLayoutId == seatLayoutId
                                          && s.ScheduleId == model.ScheduleId
                                          && s.JourneyDate == model.JourneyDate);

                    if (existingSeat != null)
                    {
                        if (existingSeat.Status == "Booked" || existingSeat.Status == "Reserved")
                        {
                            result.Errors.Add($"Seat {existingSeat.SeatNumber} is already booked.");
                        }
                        else if (existingSeat.IsOnHold && existingSeat.HoldExpiry > DateTime.UtcNow)
                        {
                            result.Errors.Add($"Seat {existingSeat.SeatNumber} is currently on hold.");
                        }
                    }
                }

                if (result.Errors.Any())
                {
                    return result;
                }

                // Calculate fare
                var fareService = new SearchService(_unitOfWork);
                var fare = fareService.CalculateFare(
                    model.ScheduleId,
                    model.SourceStopId,
                    model.DestinationStopId,
                    model.SeatLayoutIds.Count);

                // Generate booking number
                var bookingNumber = GenerateBookingNumber();

                // Create booking
                var booking = new Booking
                {
                    BookingNumber = bookingNumber,
                    UserId = 0, // Will be set by controller after user is determined
                    ScheduleId = model.ScheduleId,
                    JourneyDate = model.JourneyDate,
                    SourceStopId = model.SourceStopId,
                    DestinationStopId = model.DestinationStopId,
                    BoardingPointId = model.BoardingPointId,
                    DroppingPointId = model.DroppingPointId,
                    BookingStatus = "Pending",
                    NumberOfSeats = model.SeatLayoutIds.Count,
                    TotalFare = fare.TotalFare,
                    ConvenienceFee = fare.ConvenienceFee,
                    TaxAmount = fare.TaxAmount,
                    DiscountAmount = fare.DiscountAmount,
                    CouponDiscount = fare.CouponDiscount,
                    NetAmount = fare.NetAmount,
                    CreatedAt = DateTime.UtcNow
                };

                _unitOfWork.Bookings.Add(booking);
                _unitOfWork.SaveChanges();

                // Hold seats temporarily
                foreach (var seatLayoutId in model.SeatLayoutIds)
                {
                    var layout = seatLayouts.First(sl => sl.Id == seatLayoutId);

                    var seat = _unitOfWork.Query<Seat>()
                        .FirstOrDefault(s => s.SeatLayoutId == seatLayoutId
                                          && s.ScheduleId == model.ScheduleId
                                          && s.JourneyDate == model.JourneyDate);

                    if (seat == null)
                    {
                        seat = new Seat
                        {
                            SeatLayoutId = seatLayoutId,
                            ScheduleId = model.ScheduleId,
                            BookingId = booking.Id,
                            SeatNumber = layout.SeatNumber,
                            Status = "Available",
                            JourneyDate = model.JourneyDate,
                            IsOnHold = true,
                            HoldExpiry = DateTime.UtcNow.AddMinutes(15), // 15 minute hold
                            IsLadiesSeat = false,
                            Fare = fare.PerSeatFare,
                            CreatedAt = DateTime.UtcNow
                        };
                        _unitOfWork.Query<Seat>().Add(seat);
                    }
                    else
                    {
                        seat.IsOnHold = true;
                        seat.HoldExpiry = DateTime.UtcNow.AddMinutes(15);
                        seat.BookingId = booking.Id;
                    }
                }
                _unitOfWork.SaveChanges();

                result.Success = true;
                result.Message = "Booking initiated successfully. Please complete payment within 15 minutes.";
                result.BookingNumber = booking.BookingNumber;
                result.BookingId = booking.Id;
                result.Amount = booking.NetAmount;
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(BookingService));
                log.Error("InitiateBooking error", ex);
                result.Errors.Add("An error occurred while initiating the booking. Please try again.");
            }

            return result;
        }

        public BookingResultViewModel ConfirmBooking(int userId, ConfirmBookingViewModel model)
        {
            var result = new BookingResultViewModel
            {
                Success = false,
                Errors = new List<string>()
            };

            try
            {
                var booking = _unitOfWork.Bookings.GetByBookingNumber(model.BookingNumber);

                if (booking == null)
                {
                    result.Errors.Add("Booking not found.");
                    return result;
                }

                if (booking.BookingStatus != "Pending")
                {
                    result.Errors.Add("Booking is not in a pending state.");
                    return result;
                }

                if (booking.UserId == 0)
                {
                    booking.UserId = userId;
                }
                else if (booking.UserId != userId)
                {
                    result.Errors.Add("This booking does not belong to you.");
                    return result;
                }

                // Update contact info
                booking.ContactName = model.ContactName?.Trim();
                booking.ContactPhone = model.ContactPhone?.Trim();
                booking.ContactEmail = model.ContactEmail?.Trim();
                booking.SpecialRequests = model.SpecialRequests?.Trim();

                // Apply coupon if provided
                if (!string.IsNullOrWhiteSpace(model.CouponCode))
                {
                    var couponService = new CouponService(_unitOfWork);
                    var validation = couponService.ValidateCoupon(model.CouponCode, booking.NetAmount, userId);

                    if (validation.IsValid)
                    {
                        var coupon = _unitOfWork.Coupons.GetByCode(model.CouponCode);
                        booking.CouponId = coupon?.Id;
                        booking.CouponDiscount = validation.DiscountAmount;
                        booking.DiscountAmount = validation.DiscountAmount;
                        booking.NetAmount = booking.NetAmount - validation.DiscountAmount;
                        if (booking.NetAmount < 0) booking.NetAmount = 0;
                    }
                }

                // Create passengers
                if (model.Passengers != null && model.Passengers.Any())
                {
                    var seatIndex = 0;
                    var heldSeats = _unitOfWork.Query<Seat>()
                        .Where(s => s.BookingId == booking.Id && s.ScheduleId == booking.ScheduleId
                                 && s.JourneyDate == booking.JourneyDate)
                        .OrderBy(s => s.Id)
                        .ToList();

                    foreach (var passenger in model.Passengers)
                    {
                        var seatLayoutId = passenger.SeatLayoutId;
                        var heldSeat = heldSeats.FirstOrDefault(s => s.SeatLayoutId == seatLayoutId);

                        var bookingPassenger = new BookingPassenger
                        {
                            BookingId = booking.Id,
                            SeatId = heldSeat?.Id,
                            PassengerName = passenger.Name.Trim(),
                            Age = passenger.Age,
                            Gender = passenger.Gender,
                            IsLadiesSeat = passenger.IsLadiesSeat,
                            IdCardType = passenger.IdCardType,
                            IdCardNumber = passenger.IdCardNumber,
                            Fare = heldSeat?.Fare ?? booking.TotalFare / booking.NumberOfSeats,
                            CreatedAt = DateTime.UtcNow
                        };

                        _unitOfWork.Query<BookingPassenger>().Add(bookingPassenger);
                        seatIndex++;
                    }
                }

                _unitOfWork.SaveChanges();

                // Process payment
                var paymentService = new PaymentService(_unitOfWork);
                var paymentRequest = new PaymentRequestViewModel
                {
                    BookingNumber = booking.BookingNumber,
                    Amount = booking.NetAmount,
                    PaymentMethod = model.PaymentMethod,
                    PaymentGateway = "Mock"
                };

                var paymentResult = paymentService.ProcessPayment(paymentRequest);

                if (paymentResult.Success)
                {
                    booking.BookingStatus = "Confirmed";
                    booking.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.SaveChanges();

                    // Update seat status to Booked
                    var seatsToUpdate = _unitOfWork.Query<Seat>()
                        .Where(s => s.BookingId == booking.Id && s.ScheduleId == booking.ScheduleId
                                 && s.JourneyDate == booking.JourneyDate)
                        .ToList();

                    foreach (var seat in seatsToUpdate)
                    {
                        seat.Status = "Booked";
                        seat.IsOnHold = false;
                        seat.HoldExpiry = null;
                    }

                    // Generate ticket
                    var ticketService = new TicketService(_unitOfWork);
                    var ticket = ticketService.GenerateTicket(booking.BookingNumber);

                    _unitOfWork.SaveChanges();

                    result.Success = true;
                    result.Message = "Booking confirmed successfully!";
                    result.BookingNumber = booking.BookingNumber;
                    result.PaymentReference = paymentResult.PaymentReference;
                    result.Amount = booking.NetAmount;
                    result.BookingId = booking.Id;
                }
                else
                {
                    result.Errors.Add("Payment failed: " + paymentResult.Message);
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(BookingService));
                log.Error("ConfirmBooking error", ex);
                result.Errors.Add("An error occurred while confirming the booking. Please try again.");
            }

            return result;
        }

        public BookingConfirmationViewModel GetBookingConfirmation(string bookingNumber)
        {
            try
            {
                var booking = _unitOfWork.Query<Booking>()
                    .Include(b => b.Schedule)
                    .Include(b => b.Schedule.Bus)
                    .Include(b => b.Schedule.Bus.Operator)
                    .Include(b => b.Schedule.Route.SourceCity)
                    .Include(b => b.Schedule.Route.DestinationCity)
                    .Include(b => b.BoardingPoint)
                    .Include(b => b.DroppingPoint)
                    .Include(b => b.Tickets)
                    .Include(b => b.User)
                    .Include(b => b.BookingPassengers)
                    .Include(b => b.Payments)
                    .FirstOrDefault(b => b.BookingNumber == bookingNumber);

                if (booking == null) return null;

                var schedule = booking.Schedule;
                var bus = schedule.Bus;

                var ticket = booking.Tickets?.FirstOrDefault();
                var payment = booking.Payments?.FirstOrDefault(p => p.PaymentStatus == "Success");

                // Determine if cancellable
                bool isCancellable = booking.BookingStatus == "Confirmed"
                    && (booking.JourneyDate - DateTime.UtcNow.Date).TotalDays >= 1;

                var cancellationDeadline = booking.JourneyDate.Add(schedule.DepartureTime).AddHours(-1);

                return new BookingConfirmationViewModel
                {
                    BookingNumber = booking.BookingNumber,
                    TicketNumber = ticket?.TicketNumber,
                    BookingStatus = booking.BookingStatus,
                    JourneyDate = booking.JourneyDate,
                    DepartureTime = schedule.DepartureTime.ToString(@"hh\:mm"),
                    ArrivalTime = schedule.ArrivalTime.ToString(@"hh\:mm"),
                    Duration = $"{schedule.DurationMinutes / 60}h {schedule.DurationMinutes % 60}m",
                    BusNumber = bus.BusNumber,
                    OperatorName = bus.Operator.CompanyName,
                    SourceCity = schedule.Route.SourceCity.Name,
                    DestinationCity = schedule.Route.DestinationCity.Name,
                    BoardingPoint = booking.BoardingPoint?.Name,
                    BoardingTime = booking.BoardingPoint?.PickupTime.ToString(@"hh\:mm"),
                    DroppingPoint = booking.DroppingPoint?.Name,
                    DroppingTime = booking.DroppingPoint?.DropTime.ToString(@"hh\:mm"),
                    ContactName = booking.ContactName,
                    ContactPhone = booking.ContactPhone,
                    ContactEmail = booking.ContactEmail,
                    NumberOfSeats = booking.NumberOfSeats,
                    SeatNumbers = booking.BookingPassengers
                        .Select(bp => bp.Seat?.SeatNumber ?? "N/A")
                        .ToList(),
                    TotalFare = booking.TotalFare,
                    ConvenienceFee = booking.ConvenienceFee,
                    TaxAmount = booking.TaxAmount,
                    DiscountAmount = booking.DiscountAmount,
                    NetAmount = booking.NetAmount,
                    PaymentStatus = payment?.PaymentStatus ?? "Pending",
                    PaymentMethod = payment?.PaymentMethod,
                    BookedAt = booking.CreatedAt,
                    QRCodeBase64 = ticket?.QRCodeData,
                    IsCancellable = isCancellable,
                    CancellationDeadline = cancellationDeadline
                };
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(BookingService));
                log.Error("GetBookingConfirmation error", ex);
                return null;
            }
        }

        public BookingHistoryViewModel GetUserBookings(int userId, int page = 1, int pageSize = 10)
        {
            var result = new BookingHistoryViewModel
            {
                Bookings = new List<BookingHistoryItemViewModel>()
            };

            try
            {
                var query = _unitOfWork.Bookings.GetUserBookings(userId)
                    .Include(b => b.Schedule)
                    .Include(b => b.Schedule.Bus)
                    .Include(b => b.Schedule.Bus.Operator)
                    .Include(b => b.Schedule.Route.SourceCity)
                    .Include(b => b.Schedule.Route.DestinationCity)
                    .Include(b => b.BookingPassengers)
                    .OrderByDescending(b => b.CreatedAt);

                result.TotalCount = query.Count();
                result.TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize);
                result.CurrentPage = page;

                var bookings = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                foreach (var booking in bookings)
                {
                    var schedule = booking.Schedule;
                    var bus = schedule.Bus;

                    bool isCancellable = booking.BookingStatus == "Confirmed"
                        && (booking.JourneyDate - DateTime.UtcNow.Date).TotalDays >= 1;

                    result.Bookings.Add(new BookingHistoryItemViewModel
                    {
                        BookingNumber = booking.BookingNumber,
                        BusNumber = bus.BusNumber,
                        OperatorName = bus.Operator.CompanyName,
                        SourceCity = schedule.Route.SourceCity.Name,
                        DestinationCity = schedule.Route.DestinationCity.Name,
                        JourneyDate = booking.JourneyDate,
                        DepartureTime = schedule.DepartureTime.ToString(@"hh\:mm"),
                        ArrivalTime = schedule.ArrivalTime.ToString(@"hh\:mm"),
                        BookingStatus = booking.BookingStatus,
                        NetAmount = booking.NetAmount,
                        NumberOfSeats = booking.NumberOfSeats,
                        SeatNumbers = booking.BookingPassengers
                            .Select(bp => bp.Seat?.SeatNumber ?? "N/A")
                            .ToList(),
                        BookedAt = booking.CreatedAt,
                        IsCancellable = isCancellable
                    });
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(BookingService));
                log.Error("GetUserBookings error", ex);
            }

            return result;
        }

        public CancelBookingResult CancelBooking(int userId, string bookingNumber, string reason)
        {
            var result = new CancelBookingResult { Success = false };

            try
            {
                var booking = _unitOfWork.Query<Booking>()
                    .Include(b => b.Schedule)
                    .Include(b => b.Payments)
                    .Include(b => b.Refunds)
                    .FirstOrDefault(b => b.BookingNumber == bookingNumber);

                if (booking == null)
                {
                    result.Message = "Booking not found.";
                    return result;
                }

                if (booking.UserId != userId)
                {
                    result.Message = "This booking does not belong to you.";
                    return result;
                }

                if (booking.BookingStatus != "Confirmed" && booking.BookingStatus != "Pending")
                {
                    result.Message = "Booking cannot be cancelled in its current state.";
                    return result;
                }

                // Check cancellation window
                var departureDateTime = booking.JourneyDate.Add(booking.Schedule.DepartureTime);
                var hoursUntilDeparture = (departureDateTime - DateTime.UtcNow).TotalHours;

                if (hoursUntilDeparture < 1)
                {
                    result.Message = "Cancellation is not allowed within 1 hour of departure.";
                    return result;
                }

                // Get applicable cancellation policy
                var policy = _unitOfWork.Query<CancellationPolicy>()
                    .Where(cp => cp.OperatorId == booking.Schedule.Bus.OperatorId
                              && cp.IsActive
                              && cp.HoursBeforeDeparture <= hoursUntilDeparture)
                    .OrderByDescending(cp => cp.HoursBeforeDeparture)
                    .FirstOrDefault();

                decimal refundPercentage = policy?.RefundPercentage ?? 0m;
                decimal refundAmount = Math.Round(booking.NetAmount * refundPercentage / 100m, 2);
                decimal cancellationCharge = booking.NetAmount - refundAmount;

                if (booking.BookingStatus == "Pending")
                {
                    refundAmount = booking.NetAmount;
                    cancellationCharge = 0;
                }

                _unitOfWork.BeginTransaction();

                try
                {
                    // Update booking
                    booking.BookingStatus = "Cancelled";
                    booking.IsCancelled = true;
                    booking.CancelledAt = DateTime.UtcNow;
                    booking.CancelledBy = userId;
                    booking.CancellationReason = reason;
                    booking.RefundAmount = refundAmount;
                    booking.CancellationCharge = cancellationCharge;
                    booking.UpdatedAt = DateTime.UtcNow;

                    // Release seats
                    var seats = _unitOfWork.Query<Seat>()
                        .Where(s => s.BookingId == booking.Id)
                        .ToList();

                    foreach (var seat in seats)
                    {
                        seat.Status = "Available";
                        seat.IsOnHold = false;
                        seat.HoldExpiry = null;
                        seat.BookingId = null;
                    }

                    _unitOfWork.SaveChanges();

                    // Process refund if amount > 0
                    if (refundAmount > 0)
                    {
                        var payment = booking.Payments?.FirstOrDefault(p => p.PaymentStatus == "Success");
                        if (payment != null)
                        {
                            var refundService = new PaymentService(_unitOfWork);
                            refundService.ProcessRefund(booking.BookingNumber, reason);
                        }
                    }

                    _unitOfWork.CommitTransaction();

                    result.Success = true;
                    result.Message = "Booking cancelled successfully.";
                    result.RefundAmount = refundAmount;
                    result.CancellationCharge = cancellationCharge;
                }
                catch
                {
                    _unitOfWork.RollbackTransaction();
                    throw;
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(BookingService));
                log.Error("CancelBooking error", ex);
                result.Message = "An error occurred while cancelling the booking.";
            }

            return result;
        }

        public BookingDetailViewModel GetBookingDetail(string bookingNumber)
        {
            try
            {
                var booking = _unitOfWork.Query<Booking>()
                    .Include(b => b.Schedule)
                    .Include(b => b.Schedule.Bus)
                    .Include(b => b.Schedule.Bus.Operator)
                    .Include(b => b.Schedule.Bus.BusType)
                    .Include(b => b.Schedule.Bus.BusAmenities.Select(ba => ba.Amenity))
                    .Include(b => b.Schedule.Route.SourceCity)
                    .Include(b => b.Schedule.Route.DestinationCity)
                    .Include(b => b.Schedule.ScheduleStops.Select(ss => ss.Stop))
                    .Include(b => b.BoardingPoint)
                    .Include(b => b.DroppingPoint)
                    .Include(b => b.Tickets)
                    .Include(b => b.BookingPassengers)
                    .Include(b => b.User)
                    .Include(b => b.Payments)
                    .FirstOrDefault(b => b.BookingNumber == bookingNumber);

                if (booking == null) return null;

                var schedule = booking.Schedule;
                var bus = schedule.Bus;

                var ticket = booking.Tickets?.FirstOrDefault();
                var payment = booking.Payments?.FirstOrDefault(p => p.PaymentStatus == "Success");

                bool isCancellable = booking.BookingStatus == "Confirmed"
                    && (booking.JourneyDate - DateTime.UtcNow.Date).TotalDays >= 1;

                var cancellationDeadline = booking.JourneyDate.Add(schedule.DepartureTime).AddHours(-1);

                return new BookingDetailViewModel
                {
                    BookingNumber = booking.BookingNumber,
                    TicketNumber = ticket?.TicketNumber,
                    BookingStatus = booking.BookingStatus,
                    JourneyDate = booking.JourneyDate,
                    DepartureTime = schedule.DepartureTime.ToString(@"hh\:mm"),
                    ArrivalTime = schedule.ArrivalTime.ToString(@"hh\:mm"),
                    BusNumber = bus.BusNumber,
                    OperatorName = bus.Operator.CompanyName,
                    OperatorLogo = bus.Operator.LogoUrl,
                    BusType = bus.BusType?.Name ?? "Standard",
                    IsAC = bus.IsAC,
                    SourceCity = schedule.Route.SourceCity.Name,
                    DestinationCity = schedule.Route.DestinationCity.Name,
                    BoardingPoint = booking.BoardingPoint?.Name,
                    BoardingTime = booking.BoardingPoint?.PickupTime.ToString(@"hh\:mm"),
                    DroppingPoint = booking.DroppingPoint?.Name,
                    DroppingTime = booking.DroppingPoint?.DropTime.ToString(@"hh\:mm"),
                    ContactName = booking.ContactName,
                    ContactPhone = booking.ContactPhone,
                    ContactEmail = booking.ContactEmail,
                    NumberOfSeats = booking.NumberOfSeats,
                    SeatNumbers = booking.BookingPassengers
                        .Select(bp => bp.Seat?.SeatNumber ?? "N/A")
                        .ToList(),
                    Passengers = booking.BookingPassengers.Select(bp => new PassengerDetailViewModel
                    {
                        Name = bp.PassengerName,
                        Age = bp.Age,
                        Gender = bp.Gender,
                        SeatNumber = bp.Seat?.SeatNumber,
                        IdCardType = bp.IdCardType,
                        IdCardNumber = bp.IdCardNumber,
                        IsLadiesSeat = bp.IsLadiesSeat
                    }).ToList(),
                    TotalFare = booking.TotalFare,
                    ConvenienceFee = booking.ConvenienceFee,
                    TaxAmount = booking.TaxAmount,
                    DiscountAmount = booking.DiscountAmount,
                    NetAmount = booking.NetAmount,
                    RefundAmount = booking.RefundAmount,
                    CancellationCharge = booking.CancellationCharge,
                    PaymentStatus = payment?.PaymentStatus ?? "Pending",
                    PaymentMethod = payment?.PaymentMethod,
                    CouponCode = booking.Coupon?.Code,
                    BookedAt = booking.CreatedAt,
                    IsCancelled = booking.IsCancelled,
                    CancelledAt = booking.CancelledAt,
                    CancellationReason = booking.CancellationReason,
                    IsCancellable = isCancellable,
                    CancellationDeadline = cancellationDeadline,
                    QRCodeBase64 = ticket?.QRCodeData,
                    RouteStops = schedule.ScheduleStops
                        .OrderBy(ss => ss.StopOrder)
                        .Select(ss => new StopViewModel
                        {
                            StopId = ss.Stop.Id,
                            StopName = ss.Stop.StopName,
                            StopOrder = ss.StopOrder,
                            ArrivalTime = ss.ArrivalTime?.ToString(@"hh\:mm"),
                            DepartureTime = ss.DepartureTime?.ToString(@"hh\:mm")
                        }).ToList(),
                    Amenities = bus.BusAmenities
                        .Where(ba => ba.IsActive)
                        .Select(ba => new AmenityViewModel
                        {
                            Name = ba.Amenity.Name,
                            IconClass = ba.Amenity.IconClass
                        }).ToList()
                };
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(BookingService));
                log.Error("GetBookingDetail error", ex);
                return null;
            }
        }

        public void ReleaseHeldSeats(int scheduleId, DateTime journeyDate)
        {
            try
            {
                var expiredHolds = _unitOfWork.Query<Seat>()
                    .Where(s => s.ScheduleId == scheduleId
                             && s.JourneyDate == journeyDate
                             && s.IsOnHold
                             && s.HoldExpiry <= DateTime.UtcNow)
                    .ToList();

                foreach (var seat in expiredHolds)
                {
                    seat.IsOnHold = false;
                    seat.HoldExpiry = null;
                    seat.BookingId = null;
                }

                _unitOfWork.SaveChanges();

                // Also release bookings that are still Pending with expired holds
                var bookingIds = expiredHolds
                    .Where(s => s.BookingId.HasValue)
                    .Select(s => s.BookingId.Value)
                    .Distinct()
                    .ToList();

                if (bookingIds.Any())
                {
                    var pendingBookings = _unitOfWork.Query<Booking>()
                        .Where(b => bookingIds.Contains(b.Id) && b.BookingStatus == "Pending")
                        .ToList();

                    foreach (var booking in pendingBookings)
                    {
                        booking.BookingStatus = "Cancelled";
                        booking.IsCancelled = true;
                        booking.CancelledAt = DateTime.UtcNow;
                        booking.CancellationReason = "Auto-cancelled: Seat hold expired";
                        booking.UpdatedAt = DateTime.UtcNow;
                    }

                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(BookingService));
                log.Error("ReleaseHeldSeats error", ex);
            }
        }

        private string GenerateBookingNumber()
        {
            var prefix = "BT";
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random();
            var suffix = random.Next(1000, 9999).ToString();
            return $"{prefix}{timestamp}{suffix}";
        }
    }
}
