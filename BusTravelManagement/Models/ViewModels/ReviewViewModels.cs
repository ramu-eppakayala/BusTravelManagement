using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusTravelManagement.Models.ViewModels
{
    public class SubmitReviewViewModel
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        public int BusId { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [StringLength(2000, ErrorMessage = "Review cannot exceed 2000 characters")]
        [Display(Name = "Your Review")]
        public string ReviewText { get; set; }
    }

    public class ReviewViewModel
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int BusId { get; set; }
        public string UserName { get; set; }
        public string UserInitials { get; set; }
        public string ProfileImageUrl { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public string TimeAgo { get; set; }
        public bool IsApproved { get; set; }
        public bool IsAbuseReported { get; set; }
    }

    public class ReviewStatsViewModel
    {
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
        public int FiveStarCount { get; set; }
        public int FourStarCount { get; set; }
        public int ThreeStarCount { get; set; }
        public int TwoStarCount { get; set; }
        public int OneStarCount { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; }
        public int FiveStarPercent => TotalReviews > 0 ? (FiveStarCount * 100 / TotalReviews) : 0;
        public int FourStarPercent => TotalReviews > 0 ? (FourStarCount * 100 / TotalReviews) : 0;
        public int ThreeStarPercent => TotalReviews > 0 ? (ThreeStarCount * 100 / TotalReviews) : 0;
        public int TwoStarPercent => TotalReviews > 0 ? (TwoStarCount * 100 / TotalReviews) : 0;
        public int OneStarPercent => TotalReviews > 0 ? (OneStarCount * 100 / TotalReviews) : 0;
    }
}
