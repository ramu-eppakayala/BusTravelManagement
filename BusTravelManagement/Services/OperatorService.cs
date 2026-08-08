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
    public class OperatorService : IOperatorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OperatorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public OperatorDashboardViewModel GetDashboard(int operatorId)
        {
            var dashboard = new OperatorDashboardViewModel
            {
                WeeklyRevenueChart = new List<ChartDataPoint>(),
                OccupancyChart = new List<ChartDataPoint>(),
                RecentBookings = new List<RecentBookingViewModel>(),
                UpcomingTrips = new List<UpcomingTripViewModel>()
            };

            try
            {
                var oper = _unitOfWork.Operators.GetById(operatorId);
                if (oper == null) return dashboard;

                var today = DateTime.UtcNow.Date;
                var weekStart = today.AddDays(-(int)today.DayOfWeek);
                var monthStart = new DateTime(today.Year, today.Month, 1);

                // Get operator's bus IDs
                var busIds = _unitOfWork.Query<Bus>()
                    .Where(b => b.OperatorId == operatorId && !b.IsDeleted)
                    .Select(b => b.Id)
                    .ToList();

                // Get schedule IDs for these buses
                var scheduleIds = _unitOfWork.Query<Schedule>()
                    .Where(s => busIds.Contains(s.BusId) && s.IsActive)
                    .Select(s => s.Id)
                    .ToList();

                // Total buses
                dashboard.TotalBuses = busIds.Count;

                // Active schedules
                dashboard.ActiveSchedules = scheduleIds.Count;

                // Today's bookings
                dashboard.TodayBookings = _unitOfWork.Query<Booking>()
                    .Count(b => scheduleIds.Contains(b.ScheduleId)
                             && b.JourneyDate == today
                             && b.BookingStatus == "Confirmed");

                // Upcoming trips
                dashboard.UpcomingTripsCount = _unitOfWork.Query<Booking>()
                    .Count(b => scheduleIds.Contains(b.ScheduleId)
                             && b.JourneyDate >= today
                             && b.BookingStatus == "Confirmed");

                // Today's revenue
                var todayEnd = today.AddDays(1);
                dashboard.TodayRevenue = _unitOfWork.Query<Payment>()
                    .Where(p => scheduleIds.Contains(p.Booking.ScheduleId)
                             && p.PaymentStatus == "Success"
                             && p.PaidAt >= today && p.PaidAt < todayEnd)
                    .Sum(p => (decimal?)p.Amount) ?? 0;

                // Weekly revenue
                dashboard.WeeklyRevenue = _unitOfWork.Query<Payment>()
                    .Where(p => scheduleIds.Contains(p.Booking.ScheduleId)
                             && p.PaymentStatus == "Success"
                             && p.PaidAt >= weekStart)
                    .Sum(p => (decimal?)p.Amount) ?? 0;

                // Monthly revenue
                dashboard.MonthlyRevenue = _unitOfWork.Query<Payment>()
                    .Where(p => scheduleIds.Contains(p.Booking.ScheduleId)
                             && p.PaymentStatus == "Success"
                             && p.PaidAt >= monthStart)
                    .Sum(p => (decimal?)p.Amount) ?? 0;

                // Total seats sold and occupancy
                var totalBookings = _unitOfWork.Query<Booking>()
                    .Where(b => scheduleIds.Contains(b.ScheduleId) && b.BookingStatus == "Confirmed")
                    .ToList();

                dashboard.TotalSeatsSold = totalBookings.Sum(b => b.NumberOfSeats);

                var totalSeatsCapacity = _unitOfWork.Query<Bus>()
                    .Where(b => busIds.Contains(b.Id))
                    .Sum(b => b.TotalSeats);

                var totalTrips = _unitOfWork.Query<Booking>()
                    .Where(b => scheduleIds.Contains(b.ScheduleId) && b.BookingStatus == "Confirmed")
                    .Select(b => b.JourneyDate)
                    .Distinct()
                    .Count();

                var totalCapacity = totalSeatsCapacity * Math.Max(totalTrips, 1);
                dashboard.OccupancyRate = totalCapacity > 0
                    ? Math.Round((double)dashboard.TotalSeatsSold / totalCapacity * 100, 1)
                    : 0;

                // Weekly revenue chart (last 7 days)
                for (int i = 6; i >= 0; i--)
                {
                    var day = today.AddDays(-i);
                    var dayEnd = day.AddDays(1);
                    var dayRevenue = _unitOfWork.Query<Payment>()
                        .Where(p => scheduleIds.Contains(p.Booking.ScheduleId)
                                 && p.PaymentStatus == "Success"
                                 && p.PaidAt >= day && p.PaidAt < dayEnd)
                        .Sum(p => (decimal?)p.Amount) ?? 0;

                    dashboard.WeeklyRevenueChart.Add(new ChartDataPoint
                    {
                        Label = day.ToString("ddd"),
                        Value = dayRevenue
                    });
                }

                // Occupancy chart (last 7 days)
                for (int i = 6; i >= 0; i--)
                {
                    var day = today.AddDays(-i);
                    var dayBookings = _unitOfWork.Query<Booking>()
                        .Where(b => scheduleIds.Contains(b.ScheduleId)
                                 && b.JourneyDate == day
                                 && b.BookingStatus == "Confirmed")
                        .ToList();

                    var seatsSold = dayBookings.Sum(b => b.NumberOfSeats);
                    var dayOccupancy = totalSeatsCapacity > 0
                        ? Math.Round((double)seatsSold / totalSeatsCapacity * 100, 1)
                        : 0;

                    dashboard.OccupancyChart.Add(new ChartDataPoint
                    {
                        Label = day.ToString("ddd"),
                        Value = (decimal)dayOccupancy
                    });
                }

                // Recent bookings
                var recentBookings = _unitOfWork.Query<Booking>()
                    .Include(b => b.Schedule.Bus)
                    .Include(b => b.Schedule.Route.SourceCity)
                    .Include(b => b.Schedule.Route.DestinationCity)
                    .Include(b => b.User)
                    .Where(b => scheduleIds.Contains(b.ScheduleId))
                    .OrderByDescending(b => b.CreatedAt)
                    .Take(10)
                    .ToList();

                dashboard.RecentBookings = recentBookings.Select(b => new RecentBookingViewModel
                {
                    BookingNumber = b.BookingNumber,
                    CustomerName = b.User?.FullName ?? "N/A",
                    Route = $"{b.Schedule.Route.SourceCity.Name} - {b.Schedule.Route.DestinationCity.Name}",
                    JourneyDate = b.JourneyDate,
                    Amount = b.NetAmount,
                    Status = b.BookingStatus,
                    BookedAt = b.CreatedAt
                }).ToList();

                // Upcoming trips
                var upcoming = _unitOfWork.Query<Booking>()
                    .Include(b => b.Schedule.Bus)
                    .Include(b => b.Schedule.Route.SourceCity)
                    .Include(b => b.Schedule.Route.DestinationCity)
                    .Where(b => scheduleIds.Contains(b.ScheduleId)
                             && b.JourneyDate >= today
                             && b.BookingStatus == "Confirmed")
                    .GroupBy(b => new { b.ScheduleId, b.JourneyDate })
                    .Select(g => new
                    {
                        ScheduleId = g.Key.ScheduleId,
                        JourneyDate = g.Key.JourneyDate,
                        BookedSeats = g.Sum(b => b.NumberOfSeats),
                        BusNumber = g.FirstOrDefault().Schedule.Bus.BusNumber,
                        RouteName = g.FirstOrDefault().Schedule.Route.SourceCity.Name + " - " + g.FirstOrDefault().Schedule.Route.DestinationCity.Name,
                        DepartureTime = g.FirstOrDefault().Schedule.DepartureTime,
                        TotalSeats = g.FirstOrDefault().Schedule.Bus.TotalSeats
                    })
                    .OrderBy(x => x.JourneyDate)
                    .ThenBy(x => x.DepartureTime)
                    .Take(10)
                    .ToList();

                dashboard.UpcomingTrips = upcoming.Select(u => new UpcomingTripViewModel
                {
                    ScheduleId = u.ScheduleId,
                    BusNumber = u.BusNumber,
                    Route = u.RouteName,
                    JourneyDate = u.JourneyDate,
                    DepartureTime = u.DepartureTime.ToString(@"hh\:mm"),
                    BookedSeats = u.BookedSeats,
                    TotalSeats = u.TotalSeats,
                    OccupancyPercent = u.TotalSeats > 0
                        ? Math.Round((double)u.BookedSeats / u.TotalSeats * 100, 1)
                        : 0
                }).ToList();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(OperatorService));
                log.Error("GetDashboard error", ex);
            }

            return dashboard;
        }

        public OperatorProfileViewModel GetProfile(int operatorId)
        {
            try
            {
                var oper = _unitOfWork.Operators.GetById(operatorId);
                if (oper == null) return null;

                return new OperatorProfileViewModel
                {
                    Id = oper.Id,
                    CompanyName = oper.CompanyName,
                    CompanyRegistrationNumber = oper.CompanyRegistrationNumber,
                    ContactPerson = oper.ContactPerson,
                    ContactEmail = oper.ContactEmail,
                    ContactPhone = oper.ContactPhone,
                    Address = oper.Address,
                    City = oper.City,
                    State = oper.State,
                    Country = oper.Country,
                    PostalCode = oper.PostalCode,
                    WebsiteUrl = oper.WebsiteUrl,
                    LogoUrl = oper.LogoUrl,
                    IsVerified = oper.IsVerified
                };
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(OperatorService));
                log.Error("GetProfile error", ex);
                return null;
            }
        }

        public void UpdateProfile(int operatorId, OperatorProfileViewModel model)
        {
            try
            {
                var oper = _unitOfWork.Operators.GetById(operatorId);
                if (oper == null) throw new InvalidOperationException("Operator not found.");

                oper.CompanyName = model.CompanyName.Trim();
                oper.CompanyRegistrationNumber = model.CompanyRegistrationNumber?.Trim();
                oper.ContactPerson = model.ContactPerson.Trim();
                oper.ContactEmail = model.ContactEmail.Trim();
                oper.ContactPhone = model.ContactPhone.Trim();
                oper.Address = model.Address?.Trim();
                oper.City = model.City?.Trim();
                oper.State = model.State?.Trim();
                oper.Country = model.Country?.Trim();
                oper.PostalCode = model.PostalCode?.Trim();
                oper.WebsiteUrl = model.WebsiteUrl?.Trim();
                oper.LogoUrl = model.LogoUrl?.Trim();
                oper.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(OperatorService));
                log.Error("UpdateProfile error", ex);
                throw;
            }
        }

        public IEnumerable<ScheduleViewModel> GetSchedules(int operatorId)
        {
            try
            {
                var busIds = _unitOfWork.Query<Bus>()
                    .Where(b => b.OperatorId == operatorId && !b.IsDeleted)
                    .Select(b => b.Id)
                    .ToList();

                return _unitOfWork.Query<Schedule>()
                    .Include(s => s.Bus)
                    .Include(s => s.Route.SourceCity)
                    .Include(s => s.Route.DestinationCity)
                    .Where(s => busIds.Contains(s.BusId))
                    .OrderBy(s => s.DepartureTime)
                    .Select(s => new ScheduleViewModel
                    {
                        Id = s.Id,
                        BusNumber = s.Bus.BusNumber,
                        RouteName = s.Route.SourceCity.Name + " - " + s.Route.DestinationCity.Name,
                        DepartureTime = s.DepartureTime.ToString(@"hh\:mm"),
                        ArrivalTime = s.ArrivalTime.ToString(@"hh\:mm"),
                        DurationMinutes = s.DurationMinutes,
                        Frequency = s.Frequency,
                        BaseFare = s.BaseFare,
                        IsActive = s.IsActive
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(OperatorService));
                log.Error("GetSchedules error", ex);
                return new List<ScheduleViewModel>();
            }
        }

        public void CreateSchedule(int operatorId, CreateScheduleViewModel model)
        {
            try
            {
                // Verify bus belongs to this operator
                var bus = _unitOfWork.Query<Bus>()
                    .FirstOrDefault(b => b.Id == model.BusId && b.OperatorId == operatorId);

                if (bus == null)
                    throw new InvalidOperationException("Bus not found or does not belong to this operator.");

                var schedule = new Schedule
                {
                    BusId = model.BusId,
                    RouteId = model.RouteId,
                    DepartureTime = model.DepartureTime,
                    ArrivalTime = model.ArrivalTime,
                    DurationMinutes = model.DurationMinutes,
                    Frequency = model.Frequency,
                    DaysOfWeek = model.DaysOfWeek,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    BaseFare = model.BaseFare,
                    PerKmRate = model.PerKmRate,
                    IsActive = true,
                    IsRecurring = model.Frequency != "Custom",
                    CreatedAt = DateTime.UtcNow
                };

                _unitOfWork.Query<Schedule>().Add(schedule);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(OperatorService));
                log.Error("CreateSchedule error", ex);
                throw;
            }
        }

        public void UpdateSchedule(EditScheduleViewModel model)
        {
            try
            {
                var schedule = _unitOfWork.Query<Schedule>()
                    .FirstOrDefault(s => s.Id == model.Id);

                if (schedule == null)
                    throw new InvalidOperationException("Schedule not found.");

                schedule.DepartureTime = model.DepartureTime;
                schedule.ArrivalTime = model.ArrivalTime;
                schedule.Frequency = model.Frequency;
                schedule.BaseFare = model.BaseFare;
                schedule.PerKmRate = model.PerKmRate;
                schedule.IsActive = model.IsActive;
                schedule.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(OperatorService));
                log.Error("UpdateSchedule error", ex);
                throw;
            }
        }

        public void DeleteSchedule(int scheduleId)
        {
            try
            {
                var schedule = _unitOfWork.Query<Schedule>()
                    .FirstOrDefault(s => s.Id == scheduleId);

                if (schedule != null)
                {
                    schedule.IsActive = false;
                    schedule.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(OperatorService));
                log.Error("DeleteSchedule error", ex);
                throw;
            }
        }

        public IEnumerable<PricingRuleViewModel> GetPricingRules(int operatorId)
        {
            try
            {
                return _unitOfWork.Query<CancellationPolicy>()
                    .Where(cp => cp.OperatorId == operatorId && cp.IsActive)
                    .OrderBy(cp => cp.HoursBeforeDeparture)
                    .Select(cp => new PricingRuleViewModel
                    {
                        Id = cp.Id,
                        Description = $"Cancel {cp.HoursBeforeDeparture}+ hours before departure",
                        HoursBeforeDeparture = cp.HoursBeforeDeparture,
                        RefundPercentage = cp.RefundPercentage,
                        IsActive = cp.IsActive
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(OperatorService));
                log.Error("GetPricingRules error", ex);
                return new List<PricingRuleViewModel>();
            }
        }

        public void UpdatePricing(int operatorId, PricingViewModel model)
        {
            try
            {
                // Remove existing policies
                var existingPolicies = _unitOfWork.Query<CancellationPolicy>()
                    .Where(cp => cp.OperatorId == operatorId)
                    .ToList();

                foreach (var policy in existingPolicies)
                {
                    _unitOfWork.Query<CancellationPolicy>().Remove(policy);
                }

                _unitOfWork.SaveChanges();

                // Add new policies
                if (model.PricingRules != null)
                {
                    foreach (var rule in model.PricingRules)
                    {
                        _unitOfWork.Query<CancellationPolicy>().Add(new CancellationPolicy
                        {
                            OperatorId = operatorId,
                            HoursBeforeDeparture = rule.HoursBeforeDeparture,
                            RefundPercentage = rule.RefundPercentage,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        });
                    }

                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(OperatorService));
                log.Error("UpdatePricing error", ex);
                throw;
            }
        }
    }
}
