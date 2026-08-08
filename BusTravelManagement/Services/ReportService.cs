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
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public DailyReportViewModel GetDailyReport(DateTime date)
        {
            var report = new DailyReportViewModel
            {
                ReportDate = date,
                HourlyBookingChart = new List<ChartDataPoint>(),
                OperatorRevenueChart = new List<ChartDataPoint>()
            };

            try
            {
                var dayStart = date.Date;
                var dayEnd = dayStart.AddDays(1).AddSeconds(-1);

                var bookings = _unitOfWork.Query<Booking>()
                    .Where(b => b.CreatedAt >= dayStart && b.CreatedAt <= dayEnd)
                    .ToList();

                report.TotalBookings = bookings.Count;
                report.ConfirmedBookings = bookings.Count(b => b.BookingStatus == "Confirmed");
                report.CancelledBookings = bookings.Count(b => b.BookingStatus == "Cancelled" || b.BookingStatus == "Refunded");
                report.PendingBookings = bookings.Count(b => b.BookingStatus == "Pending");

                report.GrossRevenue = bookings.Where(b => b.BookingStatus == "Confirmed")
                    .Sum(b => (decimal?)b.TotalFare) ?? 0;

                report.NetRevenue = bookings.Where(b => b.BookingStatus == "Confirmed")
                    .Sum(b => (decimal?)b.NetAmount) ?? 0;

                report.CancellationRevenue = bookings
                    .Sum(b => (decimal?)b.CancellationCharge) ?? 0;

                report.RefundAmount = bookings
                    .Sum(b => (decimal?)b.RefundAmount) ?? 0;

                report.TotalPassengers = bookings.Sum(b => b.NumberOfSeats);

                // Occupancy rate
                var totalBuses = _unitOfWork.Query<Bus>().Count(b => b.IsActive && !b.IsDeleted);
                var totalCapacity = totalBuses > 0
                    ? _unitOfWork.Query<Bus>().Where(b => b.IsActive && !b.IsDeleted).Sum(b => b.TotalSeats)
                    : 1;
                report.OccupancyRate = Math.Round((double)report.TotalPassengers / totalCapacity * 100, 1);

                // Hourly booking chart
                for (int hour = 0; hour < 24; hour++)
                {
                    var hourStart = dayStart.AddHours(hour);
                    var hourEnd = hourStart.AddHours(1).AddSeconds(-1);

                    var hourCount = bookings.Count(b => b.CreatedAt >= hourStart && b.CreatedAt <= hourEnd);

                    report.HourlyBookingChart.Add(new ChartDataPoint
                    {
                        Label = $"{hour:D2}:00",
                        Value = hourCount
                    });
                }

                // Operator revenue chart
                var operatorRevenue = _unitOfWork.Query<Booking>()
                    .Where(b => b.CreatedAt >= dayStart && b.CreatedAt <= dayEnd
                             && b.BookingStatus == "Confirmed")
                    .GroupBy(b => b.Schedule.Bus.Operator.CompanyName)
                    .Select(g => new { Operator = g.Key, Revenue = g.Sum(b => b.NetAmount) })
                    .ToList();

                foreach (var item in operatorRevenue)
                {
                    report.OperatorRevenueChart.Add(new ChartDataPoint
                    {
                        Label = item.Operator,
                        Value = item.Revenue
                    });
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(ReportService));
                log.Error("GetDailyReport error", ex);
            }

            return report;
        }

        public MonthlyReportViewModel GetMonthlyReport(int year, int month)
        {
            var report = new MonthlyReportViewModel
            {
                Year = year,
                Month = month,
                MonthName = new DateTime(year, month, 1).ToString("MMMM"),
                DailyRevenueChart = new List<ChartDataPoint>(),
                TopRoutes = new List<ChartDataPoint>(),
                TopOperators = new List<ChartDataPoint>()
            };

            try
            {
                var monthStart = new DateTime(year, month, 1);
                var monthEnd = monthStart.AddMonths(1).AddSeconds(-1);

                var bookings = _unitOfWork.Query<Booking>()
                    .Include(b => b.Schedule.Route.SourceCity)
                    .Include(b => b.Schedule.Route.DestinationCity)
                    .Include(b => b.Schedule.Bus.Operator)
                    .Where(b => b.CreatedAt >= monthStart && b.CreatedAt <= monthEnd)
                    .ToList();

                report.TotalBookings = bookings.Count;
                report.TotalCancellations = bookings.Count(b => b.BookingStatus == "Cancelled" || b.BookingStatus == "Refunded");
                report.GrossRevenue = bookings.Where(b => b.BookingStatus == "Confirmed")
                    .Sum(b => (decimal?)b.TotalFare) ?? 0;
                report.NetRevenue = bookings.Where(b => b.BookingStatus == "Confirmed")
                    .Sum(b => (decimal?)b.NetAmount) ?? 0;
                report.RefundAmount = bookings.Sum(b => (decimal?)b.RefundAmount) ?? 0;
                report.CancellationCharges = bookings.Sum(b => (decimal?)b.CancellationCharge) ?? 0;
                report.TotalPassengers = bookings.Sum(b => b.NumberOfSeats);
                report.TotalTrips = bookings.Select(b => new { b.ScheduleId, b.JourneyDate }).Distinct().Count();

                var totalCapacity = _unitOfWork.Query<Bus>()
                    .Where(b => b.IsActive && !b.IsDeleted)
                    .Sum(b => b.TotalSeats) * report.TotalTrips;

                report.OccupancyRate = totalCapacity > 0
                    ? Math.Round((double)report.TotalPassengers / totalCapacity * 100, 1)
                    : 0;

                // Daily revenue chart
                var daysInMonth = DateTime.DaysInMonth(year, month);
                for (int day = 1; day <= daysInMonth; day++)
                {
                    var dayDate = new DateTime(year, month, day);
                    var dayRevenue = bookings
                        .Where(b => b.CreatedAt.Date == dayDate && b.BookingStatus == "Confirmed")
                        .Sum(b => b.NetAmount);

                    report.DailyRevenueChart.Add(new ChartDataPoint
                    {
                        Label = day.ToString(),
                        Value = dayRevenue
                    });
                }

                // Top routes
                var routeRevenue = bookings
                    .Where(b => b.BookingStatus == "Confirmed")
                    .GroupBy(b => $"{b.Schedule.Route.SourceCity.Name} - {b.Schedule.Route.DestinationCity.Name}")
                    .Select(g => new { Route = g.Key, Revenue = g.Sum(b => b.NetAmount) })
                    .OrderByDescending(x => x.Revenue)
                    .Take(10)
                    .ToList();

                foreach (var item in routeRevenue)
                {
                    report.TopRoutes.Add(new ChartDataPoint
                    {
                        Label = item.Route,
                        Value = item.Revenue
                    });
                }

                // Top operators
                var operatorRevenue = bookings
                    .Where(b => b.BookingStatus == "Confirmed")
                    .GroupBy(b => b.Schedule.Bus.Operator.CompanyName)
                    .Select(g => new { Operator = g.Key, Revenue = g.Sum(b => b.NetAmount) })
                    .OrderByDescending(x => x.Revenue)
                    .Take(10)
                    .ToList();

                foreach (var item in operatorRevenue)
                {
                    report.TopOperators.Add(new ChartDataPoint
                    {
                        Label = item.Operator,
                        Value = item.Revenue
                    });
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(ReportService));
                log.Error("GetMonthlyReport error", ex);
            }

            return report;
        }

        public YearlyReportViewModel GetYearlyReport(int year)
        {
            var report = new YearlyReportViewModel
            {
                Year = year,
                MonthlyRevenueChart = new List<ChartDataPoint>(),
                MonthlyBookingChart = new List<ChartDataPoint>(),
                TopRoutes = new List<ChartDataPoint>(),
                TopOperators = new List<ChartDataPoint>(),
                BusTypeDistribution = new List<ChartDataPoint>()
            };

            try
            {
                var yearStart = new DateTime(year, 1, 1);
                var yearEnd = yearStart.AddYears(1).AddSeconds(-1);

                var bookings = _unitOfWork.Query<Booking>()
                    .Include(b => b.Schedule.Route.SourceCity)
                    .Include(b => b.Schedule.Route.DestinationCity)
                    .Include(b => b.Schedule.Bus.Operator)
                    .Include(b => b.Schedule.Bus.BusType)
                    .Where(b => b.CreatedAt >= yearStart && b.CreatedAt <= yearEnd)
                    .ToList();

                report.TotalBookings = bookings.Count;
                report.TotalCancellations = bookings.Count(b => b.BookingStatus == "Cancelled" || b.BookingStatus == "Refunded");
                report.GrossRevenue = bookings.Where(b => b.BookingStatus == "Confirmed")
                    .Sum(b => (decimal?)b.TotalFare) ?? 0;
                report.NetRevenue = bookings.Where(b => b.BookingStatus == "Confirmed")
                    .Sum(b => (decimal?)b.NetAmount) ?? 0;
                report.RefundAmount = bookings.Sum(b => (decimal?)b.RefundAmount) ?? 0;
                report.CancellationCharges = bookings.Sum(b => (decimal?)b.CancellationCharge) ?? 0;
                report.TotalPassengers = bookings.Sum(b => b.NumberOfSeats);
                report.TotalTrips = bookings.Select(b => new { b.ScheduleId, b.JourneyDate }).Distinct().Count();

                var totalCapacity = _unitOfWork.Query<Bus>()
                    .Where(b => b.IsActive && !b.IsDeleted)
                    .Sum(b => b.TotalSeats) * report.TotalTrips;

                report.OccupancyRate = totalCapacity > 0
                    ? Math.Round((double)report.TotalPassengers / totalCapacity * 100, 1)
                    : 0;

                // Monthly revenue and booking charts
                for (int month = 1; month <= 12; month++)
                {
                    var monthName = new DateTime(year, month, 1).ToString("MMM");
                    var monthRevenue = bookings
                        .Where(b => b.CreatedAt.Month == month && b.BookingStatus == "Confirmed")
                        .Sum(b => b.NetAmount);
                    var monthBookings = bookings.Count(b => b.CreatedAt.Month == month);

                    report.MonthlyRevenueChart.Add(new ChartDataPoint
                    {
                        Label = monthName,
                        Value = monthRevenue
                    });

                    report.MonthlyBookingChart.Add(new ChartDataPoint
                    {
                        Label = monthName,
                        Value = monthBookings
                    });
                }

                // Top routes
                var routeRevenue = bookings
                    .Where(b => b.BookingStatus == "Confirmed")
                    .GroupBy(b => $"{b.Schedule.Route.SourceCity.Name} - {b.Schedule.Route.DestinationCity.Name}")
                    .Select(g => new { Route = g.Key, Revenue = g.Sum(b => b.NetAmount) })
                    .OrderByDescending(x => x.Revenue)
                    .Take(10)
                    .ToList();

                foreach (var item in routeRevenue)
                {
                    report.TopRoutes.Add(new ChartDataPoint
                    {
                        Label = item.Route,
                        Value = item.Revenue
                    });
                }

                // Top operators
                var operatorRevenue = bookings
                    .Where(b => b.BookingStatus == "Confirmed")
                    .GroupBy(b => b.Schedule.Bus.Operator.CompanyName)
                    .Select(g => new { Operator = g.Key, Revenue = g.Sum(b => b.NetAmount) })
                    .OrderByDescending(x => x.Revenue)
                    .Take(10)
                    .ToList();

                foreach (var item in operatorRevenue)
                {
                    report.TopOperators.Add(new ChartDataPoint
                    {
                        Label = item.Operator,
                        Value = item.Revenue
                    });
                }

                // Bus type distribution
                var busTypeCounts = bookings
                    .Where(b => b.BookingStatus == "Confirmed")
                    .GroupBy(b => b.Schedule.Bus.BusType != null
                        ? b.Schedule.Bus.BusType.Name
                        : "Standard")
                    .Select(g => new { Type = g.Key, Count = g.Count() })
                    .ToList();

                foreach (var item in busTypeCounts)
                {
                    report.BusTypeDistribution.Add(new ChartDataPoint
                    {
                        Label = item.Type,
                        Value = item.Count
                    });
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(ReportService));
                log.Error("GetYearlyReport error", ex);
            }

            return report;
        }

        public BookingReportViewModel GetBookingReport(DateTime fromDate, DateTime toDate, string status = null)
        {
            var report = new BookingReportViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                Bookings = new List<BookingReportItemViewModel>()
            };

            try
            {
                var query = _unitOfWork.Query<Booking>()
                    .Include(b => b.User)
                    .Include(b => b.Schedule.Bus.Operator)
                    .Include(b => b.Schedule.Route.SourceCity)
                    .Include(b => b.Schedule.Route.DestinationCity)
                    .Where(b => b.CreatedAt >= fromDate && b.CreatedAt <= toDate.AddDays(1));

                if (!string.IsNullOrWhiteSpace(status))
                {
                    query = query.Where(b => b.BookingStatus == status);
                }

                var bookings = query.OrderByDescending(b => b.CreatedAt).ToList();

                report.TotalBookings = bookings.Count;
                report.ConfirmedBookings = bookings.Count(b => b.BookingStatus == "Confirmed");
                report.CancelledBookings = bookings.Count(b => b.BookingStatus == "Cancelled" || b.BookingStatus == "Refunded");
                report.CompletedBookings = bookings.Count(b => b.BookingStatus == "Completed");
                report.PendingBookings = bookings.Count(b => b.BookingStatus == "Pending");
                report.TotalRevenue = bookings.Where(b => b.BookingStatus == "Confirmed")
                    .Sum(b => (decimal?)b.NetAmount) ?? 0;

                report.Bookings = bookings.Select(b => new BookingReportItemViewModel
                {
                    BookingNumber = b.BookingNumber,
                    CustomerName = b.User?.FullName ?? "N/A",
                    OperatorName = b.Schedule.Bus.Operator.CompanyName,
                    Route = $"{b.Schedule.Route.SourceCity.Name} - {b.Schedule.Route.DestinationCity.Name}",
                    JourneyDate = b.JourneyDate,
                    BookingStatus = b.BookingStatus,
                    Seats = b.NumberOfSeats,
                    Amount = b.NetAmount,
                    BookedAt = b.CreatedAt
                }).ToList();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(ReportService));
                log.Error("GetBookingReport error", ex);
            }

            return report;
        }

        public CancellationReportViewModel GetCancellationReport(DateTime fromDate, DateTime toDate)
        {
            var report = new CancellationReportViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                Cancellations = new List<CancellationReportItemViewModel>()
            };

            try
            {
                var cancellations = _unitOfWork.Query<Booking>()
                    .Include(b => b.User)
                    .Include(b => b.Schedule.Route.SourceCity)
                    .Include(b => b.Schedule.Route.DestinationCity)
                    .Where(b => b.IsCancelled
                             && b.CancelledAt >= fromDate
                             && b.CancelledAt <= toDate.AddDays(1))
                    .OrderByDescending(b => b.CancelledAt)
                    .ToList();

                report.TotalCancellations = cancellations.Count;
                report.TotalRefundAmount = cancellations.Sum(b => b.RefundAmount);
                report.TotalCancellationCharges = cancellations.Sum(b => b.CancellationCharge);

                report.Cancellations = cancellations.Select(b => new CancellationReportItemViewModel
                {
                    BookingNumber = b.BookingNumber,
                    CustomerName = b.User?.FullName ?? "N/A",
                    Route = $"{b.Schedule.Route.SourceCity.Name} - {b.Schedule.Route.DestinationCity.Name}",
                    JourneyDate = b.JourneyDate,
                    BookingAmount = b.NetAmount,
                    RefundAmount = b.RefundAmount,
                    CancellationCharge = b.CancellationCharge,
                    Reason = b.CancellationReason,
                    CancelledAt = b.CancelledAt ?? b.CreatedAt
                }).ToList();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(ReportService));
                log.Error("GetCancellationReport error", ex);
            }

            return report;
        }

        public OperatorReportViewModel GetOperatorReport(int operatorId, DateTime fromDate, DateTime toDate)
        {
            var report = new OperatorReportViewModel
            {
                OperatorId = operatorId,
                FromDate = fromDate,
                ToDate = toDate,
                BusReports = new List<OperatorBusReportViewModel>()
            };

            try
            {
                var oper = _unitOfWork.Operators.GetById(operatorId);
                if (oper == null) return report;

                report.OperatorName = oper.CompanyName;

                var busIds = _unitOfWork.Query<Bus>()
                    .Where(b => b.OperatorId == operatorId && !b.IsDeleted)
                    .Select(b => b.Id)
                    .ToList();

                var bookings = _unitOfWork.Query<Booking>()
                    .Include(b => b.Schedule.Bus)
                    .Where(b => busIds.Contains(b.Schedule.BusId)
                             && b.CreatedAt >= fromDate
                             && b.CreatedAt <= toDate.AddDays(1))
                    .ToList();

                report.TotalTrips = bookings.Select(b => new { b.ScheduleId, b.JourneyDate }).Distinct().Count();
                report.TotalBookings = bookings.Count;
                report.TotalCancellations = bookings.Count(b => b.BookingStatus == "Cancelled" || b.BookingStatus == "Refunded");
                report.TotalPassengers = bookings.Sum(b => b.NumberOfSeats);
                report.GrossRevenue = bookings.Where(b => b.BookingStatus == "Confirmed")
                    .Sum(b => (decimal?)b.TotalFare) ?? 0;
                report.NetRevenue = bookings.Where(b => b.BookingStatus == "Confirmed")
                    .Sum(b => (decimal?)b.NetAmount) ?? 0;
                report.CommissionAmount = Math.Round(report.NetRevenue * oper.CommissionPercentage / 100m, 2);

                var totalSeats = _unitOfWork.Query<Bus>()
                    .Where(b => busIds.Contains(b.Id))
                    .Sum(b => b.TotalSeats);

                var totalTripsCount = bookings.Select(b => new { b.ScheduleId, b.JourneyDate }).Distinct().Count();
                var totalCapacity = totalSeats * Math.Max(totalTripsCount, 1);
                report.OccupancyRate = totalCapacity > 0
                    ? Math.Round((double)report.TotalPassengers / totalCapacity * 100, 1)
                    : 0;

                // Per-bus reports
                foreach (var busId in busIds)
                {
                    var busBookings = bookings.Where(b => b.Schedule.BusId == busId).ToList();
                    var bus = _unitOfWork.Buses.GetById(busId);

                    if (bus != null)
                    {
                        var busCapacity = bus.TotalSeats * Math.Max(
                            busBookings.Select(b => new { b.ScheduleId, b.JourneyDate }).Distinct().Count(), 1);

                        report.BusReports.Add(new OperatorBusReportViewModel
                        {
                            BusId = busId,
                            BusNumber = bus.BusNumber,
                            TotalBookings = busBookings.Count,
                            TotalPassengers = busBookings.Sum(b => b.NumberOfSeats),
                            Revenue = busBookings.Where(b => b.BookingStatus == "Confirmed")
                                .Sum(b => (decimal?)b.NetAmount) ?? 0,
                            OccupancyRate = busCapacity > 0
                                ? Math.Round((double)busBookings.Sum(b => b.NumberOfSeats) / busCapacity * 100, 1)
                                : 0
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(ReportService));
                log.Error("GetOperatorReport error", ex);
            }

            return report;
        }

        public CustomerReportViewModel GetCustomerReport(int userId, DateTime fromDate, DateTime toDate)
        {
            var report = new CustomerReportViewModel
            {
                UserId = userId,
                FromDate = fromDate,
                ToDate = toDate,
                RecentBookings = new List<BookingHistoryItemViewModel>()
            };

            try
            {
                var user = _unitOfWork.Users.GetById(userId);
                if (user == null) return report;

                report.CustomerName = user.FullName;
                report.Email = user.Email;
                report.Phone = user.PhoneNumber;

                var bookings = _unitOfWork.Bookings.GetUserBookings(userId)
                    .Include(b => b.Schedule.Bus.Operator)
                    .Include(b => b.Schedule.Route.SourceCity)
                    .Include(b => b.Schedule.Route.DestinationCity)
                    .Include(b => b.BookingPassengers)
                    .Where(b => b.CreatedAt >= fromDate && b.CreatedAt <= toDate.AddDays(1))
                    .OrderByDescending(b => b.CreatedAt)
                    .ToList();

                report.TotalBookings = bookings.Count;
                report.CompletedTrips = bookings.Count(b => b.BookingStatus == "Completed" || b.BookingStatus == "Confirmed");
                report.CancelledBookings = bookings.Count(b => b.BookingStatus == "Cancelled" || b.BookingStatus == "Refunded");
                report.TotalSpent = bookings.Where(b => b.BookingStatus == "Confirmed" || b.BookingStatus == "Completed")
                    .Sum(b => (decimal?)b.NetAmount) ?? 0;
                report.TotalRefunded = bookings.Sum(b => (decimal?)b.RefundAmount) ?? 0;

                report.RecentBookings = bookings.Select(b => new BookingHistoryItemViewModel
                {
                    BookingNumber = b.BookingNumber,
                    BusNumber = b.Schedule.Bus.BusNumber,
                    OperatorName = b.Schedule.Bus.Operator.CompanyName,
                    SourceCity = b.Schedule.Route.SourceCity.Name,
                    DestinationCity = b.Schedule.Route.DestinationCity.Name,
                    JourneyDate = b.JourneyDate,
                    DepartureTime = b.Schedule.DepartureTime.ToString(@"hh\:mm"),
                    ArrivalTime = b.Schedule.ArrivalTime.ToString(@"hh\:mm"),
                    BookingStatus = b.BookingStatus,
                    NetAmount = b.NetAmount,
                    NumberOfSeats = b.NumberOfSeats,
                    SeatNumbers = b.BookingPassengers
                        .Select(bp => bp.Seat?.SeatNumber ?? "N/A")
                        .ToList(),
                    BookedAt = b.CreatedAt,
                    IsCancellable = b.BookingStatus == "Confirmed"
                        && (b.JourneyDate - DateTime.UtcNow.Date).TotalDays >= 1
                }).ToList();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(ReportService));
                log.Error("GetCustomerReport error", ex);
            }

            return report;
        }

        public OccupancyReportViewModel GetOccupancyReport(int busId, DateTime fromDate, DateTime toDate)
        {
            var report = new OccupancyReportViewModel
            {
                BusId = busId,
                FromDate = fromDate,
                ToDate = toDate,
                DailyOccupancy = new List<DailyOccupancyViewModel>()
            };

            try
            {
                var bus = _unitOfWork.Query<Bus>()
                    .Include(b => b.Operator)
                    .FirstOrDefault(b => b.Id == busId);

                if (bus == null) return report;

                report.BusNumber = bus.BusNumber;
                report.OperatorName = bus.Operator.CompanyName;
                report.TotalSeats = bus.TotalSeats;

                var scheduleIds = _unitOfWork.Query<Schedule>()
                    .Where(s => s.BusId == busId && s.IsActive)
                    .Select(s => s.Id)
                    .ToList();

                var totalDays = (toDate - fromDate).Days + 1;
                report.TotalTrips = scheduleIds.Count * totalDays;

                // Get all bookings for this bus in the date range
                var bookings = _unitOfWork.Query<Booking>()
                    .Where(b => scheduleIds.Contains(b.ScheduleId)
                             && b.JourneyDate >= fromDate
                             && b.JourneyDate <= toDate
                             && (b.BookingStatus == "Confirmed" || b.BookingStatus == "Completed"))
                    .ToList();

                report.TotalSeatsSold = bookings.Sum(b => b.NumberOfSeats);
                report.TotalSeatsAvailable = report.TotalSeats * report.TotalTrips - report.TotalSeatsSold;
                report.OverallOccupancyRate = (report.TotalSeats * report.TotalTrips) > 0
                    ? Math.Round((double)report.TotalSeatsSold / (report.TotalSeats * report.TotalTrips) * 100, 1)
                    : 0;

                // Daily occupancy
                for (var date = fromDate; date <= toDate; date = date.AddDays(1))
                {
                    var dayBookings = bookings.Where(b => b.JourneyDate == date).ToList();
                    var bookedSeats = dayBookings.Sum(b => b.NumberOfSeats);
                    var dayRevenue = dayBookings.Sum(b => b.NetAmount);

                    report.DailyOccupancy.Add(new DailyOccupancyViewModel
                    {
                        Date = date,
                        TotalSeats = report.TotalSeats,
                        BookedSeats = bookedSeats,
                        AvailableSeats = report.TotalSeats - bookedSeats,
                        OccupancyPercent = report.TotalSeats > 0
                            ? Math.Round((double)bookedSeats / report.TotalSeats * 100, 1)
                            : 0,
                        Revenue = dayRevenue
                    });
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(ReportService));
                log.Error("GetOccupancyReport error", ex);
            }

            return report;
        }
    }
}
