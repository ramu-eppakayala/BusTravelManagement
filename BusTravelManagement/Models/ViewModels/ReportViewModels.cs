using System.Collections.Generic;

namespace BusTravelManagement.Models.ViewModels
{
    public class DailyReportViewModel
    {
        public System.DateTime ReportDate { get; set; }
        public int TotalBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int PendingBookings { get; set; }
        public decimal GrossRevenue { get; set; }
        public decimal NetRevenue { get; set; }
        public decimal CancellationRevenue { get; set; }
        public decimal RefundAmount { get; set; }
        public int TotalPassengers { get; set; }
        public double OccupancyRate { get; set; }
        public List<ChartDataPoint> HourlyBookingChart { get; set; }
        public List<ChartDataPoint> OperatorRevenueChart { get; set; }
    }

    public class MonthlyReportViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public int TotalBookings { get; set; }
        public int TotalCancellations { get; set; }
        public decimal GrossRevenue { get; set; }
        public decimal NetRevenue { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal CancellationCharges { get; set; }
        public int TotalPassengers { get; set; }
        public int TotalTrips { get; set; }
        public double OccupancyRate { get; set; }
        public List<ChartDataPoint> DailyRevenueChart { get; set; }
        public List<ChartDataPoint> TopRoutes { get; set; }
        public List<ChartDataPoint> TopOperators { get; set; }
    }

    public class YearlyReportViewModel
    {
        public int Year { get; set; }
        public int TotalBookings { get; set; }
        public int TotalCancellations { get; set; }
        public decimal GrossRevenue { get; set; }
        public decimal NetRevenue { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal CancellationCharges { get; set; }
        public int TotalPassengers { get; set; }
        public int TotalTrips { get; set; }
        public double OccupancyRate { get; set; }
        public List<ChartDataPoint> MonthlyRevenueChart { get; set; }
        public List<ChartDataPoint> MonthlyBookingChart { get; set; }
        public List<ChartDataPoint> TopRoutes { get; set; }
        public List<ChartDataPoint> TopOperators { get; set; }
        public List<ChartDataPoint> BusTypeDistribution { get; set; }
    }

    public class BookingReportViewModel
    {
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
        public int TotalBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int PendingBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<BookingReportItemViewModel> Bookings { get; set; }
    }

    public class BookingReportItemViewModel
    {
        public string BookingNumber { get; set; }
        public string CustomerName { get; set; }
        public string OperatorName { get; set; }
        public string Route { get; set; }
        public System.DateTime JourneyDate { get; set; }
        public string BookingStatus { get; set; }
        public int Seats { get; set; }
        public decimal Amount { get; set; }
        public System.DateTime BookedAt { get; set; }
    }

    public class CancellationReportViewModel
    {
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
        public int TotalCancellations { get; set; }
        public decimal TotalRefundAmount { get; set; }
        public decimal TotalCancellationCharges { get; set; }
        public List<CancellationReportItemViewModel> Cancellations { get; set; }
    }

    public class CancellationReportItemViewModel
    {
        public string BookingNumber { get; set; }
        public string CustomerName { get; set; }
        public string Route { get; set; }
        public System.DateTime JourneyDate { get; set; }
        public decimal BookingAmount { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal CancellationCharge { get; set; }
        public string Reason { get; set; }
        public System.DateTime CancelledAt { get; set; }
    }

    public class OperatorReportViewModel
    {
        public int OperatorId { get; set; }
        public string OperatorName { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
        public int TotalTrips { get; set; }
        public int TotalBookings { get; set; }
        public int TotalCancellations { get; set; }
        public int TotalPassengers { get; set; }
        public decimal GrossRevenue { get; set; }
        public decimal NetRevenue { get; set; }
        public decimal CommissionAmount { get; set; }
        public double OccupancyRate { get; set; }
        public List<OperatorBusReportViewModel> BusReports { get; set; }
    }

    public class OperatorBusReportViewModel
    {
        public int BusId { get; set; }
        public string BusNumber { get; set; }
        public int TotalBookings { get; set; }
        public int TotalPassengers { get; set; }
        public decimal Revenue { get; set; }
        public double OccupancyRate { get; set; }
    }

    public class CustomerReportViewModel
    {
        public int UserId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
        public int TotalBookings { get; set; }
        public int CompletedTrips { get; set; }
        public int CancelledBookings { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal TotalRefunded { get; set; }
        public List<BookingHistoryItemViewModel> RecentBookings { get; set; }
    }

    public class OccupancyReportViewModel
    {
        public int BusId { get; set; }
        public string BusNumber { get; set; }
        public string OperatorName { get; set; }
        public int TotalSeats { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
        public int TotalTrips { get; set; }
        public int TotalSeatsSold { get; set; }
        public int TotalSeatsAvailable { get; set; }
        public double OverallOccupancyRate { get; set; }
        public List<DailyOccupancyViewModel> DailyOccupancy { get; set; }
    }

    public class DailyOccupancyViewModel
    {
        public System.DateTime Date { get; set; }
        public int TotalSeats { get; set; }
        public int BookedSeats { get; set; }
        public int AvailableSeats { get; set; }
        public double OccupancyPercent { get; set; }
        public decimal Revenue { get; set; }
    }
}
