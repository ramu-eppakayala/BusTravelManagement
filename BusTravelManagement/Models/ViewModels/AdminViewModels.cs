using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusTravelManagement.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalOperators { get; set; }
        public int TotalBuses { get; set; }
        public int TotalBookings { get; set; }
        public int TotalBookingsToday { get; set; }
        public int PendingReviews { get; set; }
        public int PendingOperators { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal YearlyRevenue { get; set; }
        public List<ChartDataPoint> RevenueChartData { get; set; }
        public List<ChartDataPoint> BookingChartData { get; set; }
        public List<RecentBookingViewModel> RecentBookings { get; set; }
    }

    public class ChartDataPoint
    {
        public string Label { get; set; }
        public decimal Value { get; set; }
    }

    public class RecentBookingViewModel
    {
        public string BookingNumber { get; set; }
        public string CustomerName { get; set; }
        public string Route { get; set; }
        public System.DateTime JourneyDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public System.DateTime BookedAt { get; set; }
    }

    public class UserListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Roles { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public bool IsEmailVerified { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public System.DateTime? LastLoginAt { get; set; }
    }

    public class OperatorListViewModel
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string City { get; set; }
        public int BusCount { get; set; }
        public bool IsActive { get; set; }
        public bool IsVerified { get; set; }
        public decimal Commission { get; set; }
        public System.DateTime CreatedAt { get; set; }
    }

    public class CouponViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MinBookingAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime ValidFrom { get; set; }
        public System.DateTime ValidTo { get; set; }
    }

    public class CreateCouponViewModel
    {
        [Required(ErrorMessage = "Coupon code is required")]
        [StringLength(50)]
        [Display(Name = "Coupon Code")]
        public string Code { get; set; }

        [StringLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Discount type is required")]
        [Display(Name = "Discount Type")]
        public string DiscountType { get; set; }

        [Required(ErrorMessage = "Discount value is required")]
        [Range(0, 100000)]
        [Display(Name = "Discount Value")]
        public decimal DiscountValue { get; set; }

        [Display(Name = "Minimum Booking Amount")]
        [Range(0, 100000)]
        public decimal? MinBookingAmount { get; set; }

        [Display(Name = "Maximum Discount Amount")]
        [Range(0, 100000)]
        public decimal? MaxDiscountAmount { get; set; }

        [Display(Name = "Usage Limit")]
        [Range(1, 100000)]
        public int? UsageLimit { get; set; }

        [Display(Name = "Per User Limit")]
        [Range(1, 100)]
        public int PerUserLimit { get; set; } = 1;

        [Required(ErrorMessage = "Valid from date is required")]
        [Display(Name = "Valid From")]
        [DataType(DataType.Date)]
        public System.DateTime ValidFrom { get; set; }

        [Required(ErrorMessage = "Valid to date is required")]
        [Display(Name = "Valid To")]
        [DataType(DataType.Date)]
        public System.DateTime ValidTo { get; set; }
    }

    public class AuditLogViewModel
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public string EntityType { get; set; }
        public int? EntityId { get; set; }
        public string UserName { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public System.DateTime CreatedAt { get; set; }
    }
}
