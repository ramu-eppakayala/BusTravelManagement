using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BusTravelManagement.Models.ViewModels
{
    public class SearchViewModel
    {
        [Required(ErrorMessage = "Source city is required")]
        [Display(Name = "From")]
        public int SourceCityId { get; set; }

        [Required(ErrorMessage = "Destination city is required")]
        [Display(Name = "To")]
        public int DestinationCityId { get; set; }

        [Required(ErrorMessage = "Journey date is required")]
        [Display(Name = "Journey Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime JourneyDate { get; set; } = System.DateTime.Today.AddDays(1);

        // Filter options
        [Display(Name = "Bus Type")]
        public int? BusTypeId { get; set; }

        [Display(Name = "Operator")]
        public int? OperatorId { get; set; }

        [Display(Name = "Departure After")]
        public System.TimeSpan? DepartureAfter { get; set; }

        [Display(Name = "Arrival Before")]
        public System.TimeSpan? ArrivalBefore { get; set; }

        [Display(Name = "AC Buses Only")]
        public bool IsAC { get; set; }

        [Display(Name = "Non-AC Buses Only")]
        public bool IsNonAC { get; set; }

        [Display(Name = "Sleeper")]
        public bool IsSleeper { get; set; }

        [Display(Name = "Seater")]
        public bool IsSeater { get; set; }

        [Display(Name = "Max Price")]
        [Range(0, 100000)]
        public decimal? MaxPrice { get; set; }

        [Display(Name = "Min Price")]
        [Range(0, 100000)]
        public decimal? MinPrice { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string SortBy { get; set; } = "Departure";
    }

    public class SearchResultViewModel
    {
        public List<BusResultViewModel> Buses { get; set; }
        public SearchViewModel SearchCriteria { get; set; }
        public int TotalResults { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SourceCityName { get; set; }
        public string DestinationCityName { get; set; }
    }

    public class BusResultViewModel
    {
        public int ScheduleId { get; set; }
        public int BusId { get; set; }
        public int OperatorId { get; set; }
        public string BusNumber { get; set; }
        public string OperatorName { get; set; }
        public string OperatorLogo { get; set; }
        public string BusType { get; set; }
        public bool IsAC { get; set; }
        public bool IsSleeper { get; set; }
        public string SeatLayoutType { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public int DurationMinutes { get; set; }
        public string Duration => $"{DurationMinutes / 60}h {DurationMinutes % 60}m";
        public decimal StartingFare { get; set; }
        public decimal BaseFare { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public List<string> Amenities { get; set; }
        public int BoardingPointsCount { get; set; }
        public int DroppingPointsCount { get; set; }
    }

    public class BusDetailViewModel
    {
        public int ScheduleId { get; set; }
        public int BusId { get; set; }
        public string BusNumber { get; set; }
        public string RegistrationNumber { get; set; }
        public string OperatorName { get; set; }
        public string OperatorLogo { get; set; }
        public string BusType { get; set; }
        public string SeatLayoutType { get; set; }
        public bool IsAC { get; set; }
        public bool IsSleeper { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public int DurationMinutes { get; set; }
        public string Duration => $"{DurationMinutes / 60}h {DurationMinutes % 60}m";
        public decimal BaseFare { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public System.DateTime JourneyDate { get; set; }
        public string SourceCity { get; set; }
        public string DestinationCity { get; set; }
        public List<AmenityViewModel> Amenities { get; set; }
        public List<BoardingPointViewModel> BoardingPoints { get; set; }
        public List<DroppingPointViewModel> DroppingPoints { get; set; }
        public List<StopViewModel> Stops { get; set; }
        public List<ReviewViewModel> Reviews { get; set; }
        public List<string> GalleryImages { get; set; }
    }

    public class AmenityViewModel
    {
        public string Name { get; set; }
        public string IconClass { get; set; }
    }

    public class BoardingPointViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Landmark { get; set; }
        public string PickupTime { get; set; }
        public int DayOffset { get; set; }
    }

    public class DroppingPointViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Landmark { get; set; }
        public string DropTime { get; set; }
        public int DayOffset { get; set; }
    }

    public class StopViewModel
    {
        public int StopId { get; set; }
        public string StopName { get; set; }
        public int StopOrder { get; set; }
        public string ArrivalTime { get; set; }
        public string DepartureTime { get; set; }
    }

    public class CitySearchResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public string DisplayName => $"{Name}, {State}";
    }

    public class SeatViewModel
    {
        public int SeatLayoutId { get; set; }
        public int? SeatId { get; set; }
        public string SeatNumber { get; set; }
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
        public string SeatPosition { get; set; } // Window, Aisle, Middle
        public string Deck { get; set; }
        public string Status { get; set; } // Available, Booked, Reserved, Ladies, Selected
        public decimal Fare { get; set; }
        public bool IsLadiesSeat { get; set; }
    }

    public class SeatLayoutViewModel
    {
        public int BusId { get; set; }
        public string BusNumber { get; set; }
        public string SeatLayoutType { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableCount { get; set; }
        public int BookedCount { get; set; }
        public int LadiesSeatCount { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<SeatViewModel> Seats { get; set; }
        public System.DateTime JourneyDate { get; set; }
    }

    public class FareBreakdownViewModel
    {
        public decimal BaseFare { get; set; }
        public decimal DistanceFare { get; set; }
        public decimal TotalFare { get; set; }
        public decimal ConvenienceFee { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal CouponDiscount { get; set; }
        public decimal NetAmount { get; set; }
        public int NumberOfSeats { get; set; }
        public decimal PerSeatFare { get; set; }
        public string CouponCode { get; set; }
        public bool CouponApplied { get; set; }
    }
}
