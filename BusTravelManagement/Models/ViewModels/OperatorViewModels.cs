using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusTravelManagement.Models.ViewModels
{
    public class OperatorDashboardViewModel
    {
        public int TotalBuses { get; set; }
        public int ActiveSchedules { get; set; }
        public int TodayBookings { get; set; }
        public int UpcomingTripsCount { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal WeeklyRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int TotalSeatsSold { get; set; }
        public double OccupancyRate { get; set; }
        public List<ChartDataPoint> WeeklyRevenueChart { get; set; }
        public List<ChartDataPoint> OccupancyChart { get; set; }
        public List<RecentBookingViewModel> RecentBookings { get; set; }
        public List<UpcomingTripViewModel> UpcomingTrips { get; set; }
    }

    public class UpcomingTripViewModel
    {
        public int ScheduleId { get; set; }
        public string BusNumber { get; set; }
        public string Route { get; set; }
        public System.DateTime JourneyDate { get; set; }
        public string DepartureTime { get; set; }
        public int BookedSeats { get; set; }
        public int TotalSeats { get; set; }
        public double OccupancyPercent { get; set; }
    }

    public class OperatorProfileViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Company name is required")]
        [StringLength(200)]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [StringLength(100)]
        [Display(Name = "Registration Number")]
        public string CompanyRegistrationNumber { get; set; }

        [Required(ErrorMessage = "Contact person is required")]
        [StringLength(100)]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }

        [Required(ErrorMessage = "Contact email is required")]
        [EmailAddress]
        [StringLength(100)]
        [Display(Name = "Contact Email")]
        public string ContactEmail { get; set; }

        [Required(ErrorMessage = "Contact phone is required")]
        [StringLength(20)]
        [Display(Name = "Contact Phone")]
        public string ContactPhone { get; set; }

        [StringLength(500)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [StringLength(100)]
        [Display(Name = "City")]
        public string City { get; set; }

        [StringLength(100)]
        [Display(Name = "State")]
        public string State { get; set; }

        [StringLength(100)]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [StringLength(20)]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [StringLength(200)]
        [Display(Name = "Website")]
        public string WebsiteUrl { get; set; }

        [Display(Name = "Logo")]
        public string LogoUrl { get; set; }

        public bool IsVerified { get; set; }
    }

    public class BusListViewModel
    {
        public int Id { get; set; }
        public string BusNumber { get; set; }
        public string RegistrationNumber { get; set; }
        public string BusType { get; set; }
        public int TotalSeats { get; set; }
        public string SeatLayoutType { get; set; }
        public bool IsAC { get; set; }
        public bool IsSleeper { get; set; }
        public bool IsActive { get; set; }
        public int ScheduleCount { get; set; }
    }

    public class CreateBusViewModel
    {
        [Required(ErrorMessage = "Bus number is required")]
        [StringLength(50)]
        [Display(Name = "Bus Number")]
        public string BusNumber { get; set; }

        [Required(ErrorMessage = "Registration number is required")]
        [StringLength(50)]
        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; }

        [Display(Name = "Bus Type")]
        public int? BusTypeId { get; set; }

        [Required(ErrorMessage = "Total seats is required")]
        [Range(10, 80)]
        [Display(Name = "Total Seats")]
        public int TotalSeats { get; set; }

        [Required(ErrorMessage = "Seat layout type is required")]
        [Display(Name = "Seat Layout")]
        public string SeatLayoutType { get; set; }

        [Display(Name = "Air Conditioned")]
        public bool IsAC { get; set; }

        [Display(Name = "Sleeper")]
        public bool IsSleeper { get; set; }

        [Display(Name = "Single Axle")]
        public bool IsSingleAxle { get; set; } = true;

        [Display(Name = "Amenities")]
        public List<int> AmenityIds { get; set; }
    }

    public class EditBusViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Bus number is required")]
        [StringLength(50)]
        [Display(Name = "Bus Number")]
        public string BusNumber { get; set; }

        [Required(ErrorMessage = "Registration number is required")]
        [StringLength(50)]
        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; }

        [Display(Name = "Bus Type")]
        public int? BusTypeId { get; set; }

        [Display(Name = "Air Conditioned")]
        public bool IsAC { get; set; }

        [Display(Name = "Sleeper")]
        public bool IsSleeper { get; set; }

        [Display(Name = "Single Axle")]
        public bool IsSingleAxle { get; set; }

        public bool IsActive { get; set; }

        [Display(Name = "Amenities")]
        public List<int> AmenityIds { get; set; }
    }

    public class ScheduleViewModel
    {
        public int Id { get; set; }
        public string BusNumber { get; set; }
        public string RouteName { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public int DurationMinutes { get; set; }
        public string Frequency { get; set; }
        public decimal BaseFare { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateScheduleViewModel
    {
        [Required(ErrorMessage = "Bus is required")]
        [Display(Name = "Bus")]
        public int BusId { get; set; }

        [Required(ErrorMessage = "Route is required")]
        [Display(Name = "Route")]
        public int RouteId { get; set; }

        [Required(ErrorMessage = "Departure time is required")]
        [Display(Name = "Departure Time")]
        [DataType(DataType.Time)]
        public System.TimeSpan DepartureTime { get; set; }

        [Required(ErrorMessage = "Arrival time is required")]
        [Display(Name = "Arrival Time")]
        [DataType(DataType.Time)]
        public System.TimeSpan ArrivalTime { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 1440)]
        [Display(Name = "Duration (minutes)")]
        public int DurationMinutes { get; set; }

        [Required]
        [Display(Name = "Frequency")]
        public string Frequency { get; set; }

        [Display(Name = "Days of Week")]
        public string DaysOfWeek { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public System.DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public System.DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Base fare is required")]
        [Range(1, 100000)]
        [Display(Name = "Base Fare")]
        public decimal BaseFare { get; set; }

        [Display(Name = "Per Km Rate")]
        [Range(0, 1000)]
        public decimal PerKmRate { get; set; }
    }

    public class EditScheduleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Departure time is required")]
        [Display(Name = "Departure Time")]
        [DataType(DataType.Time)]
        public System.TimeSpan DepartureTime { get; set; }

        [Required(ErrorMessage = "Arrival time is required")]
        [Display(Name = "Arrival Time")]
        [DataType(DataType.Time)]
        public System.TimeSpan ArrivalTime { get; set; }

        [Display(Name = "Frequency")]
        public string Frequency { get; set; }

        [Display(Name = "Base Fare")]
        [Range(1, 100000)]
        public decimal BaseFare { get; set; }

        [Display(Name = "Per Km Rate")]
        [Range(0, 1000)]
        public decimal PerKmRate { get; set; }

        public bool IsActive { get; set; }
    }

    public class PricingViewModel
    {
        public int OperatorId { get; set; }
        public List<PricingRuleViewModel> PricingRules { get; set; }
    }

    public class PricingRuleViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int HoursBeforeDeparture { get; set; }
        public decimal RefundPercentage { get; set; }
        public bool IsActive { get; set; }
    }
}
