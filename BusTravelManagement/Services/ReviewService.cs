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
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void SubmitReview(int userId, SubmitReviewViewModel model)
        {
            try
            {
                // Validate booking ownership
                var booking = _unitOfWork.Query<Booking>()
                    .FirstOrDefault(b => b.Id == model.BookingId
                                      && b.UserId == userId
                                      && b.BookingStatus == "Completed");

                if (booking == null)
                {
                    throw new InvalidOperationException("Booking not found or not eligible for review. Only completed trips can be reviewed.");
                }

                // Check for duplicate review
                var existingReview = _unitOfWork.Query<Review>()
                    .FirstOrDefault(r => r.BookingId == model.BookingId && r.UserId == userId);

                if (existingReview != null)
                {
                    throw new InvalidOperationException("You have already submitted a review for this booking.");
                }

                var review = new Review
                {
                    BookingId = model.BookingId,
                    UserId = userId,
                    BusId = model.BusId,
                    Rating = model.Rating,
                    ReviewText = model.ReviewText?.Trim(),
                    IsApproved = false,
                    IsAbuseReported = false,
                    CreatedAt = DateTime.UtcNow
                };

                _unitOfWork.Reviews.Add(review);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(ReviewService));
                log.Error("SubmitReview error", ex);
                throw;
            }
        }

        public IEnumerable<ReviewViewModel> GetBusReviews(int busId)
        {
            try
            {
                return _unitOfWork.Reviews.GetBusReviews(busId)
                    .Include(r => r.User)
                    .Where(r => r.IsApproved)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList()
                    .Select(r => new ReviewViewModel
                    {
                        Id = r.Id,
                        BookingId = r.BookingId,
                        BusId = r.BusId,
                        UserName = r.User?.FullName ?? "Anonymous",
                        UserInitials = r.User != null
                            ? $"{r.User.FirstName[0]}{r.User.LastName[0]}"
                            : "AN",
                        ProfileImageUrl = r.User?.ProfileImageUrl,
                        Rating = r.Rating,
                        ReviewText = r.ReviewText,
                        CreatedAt = r.CreatedAt,
                        TimeAgo = GetTimeAgo(r.CreatedAt),
                        IsApproved = r.IsApproved,
                        IsAbuseReported = r.IsAbuseReported
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(ReviewService));
                log.Error("GetBusReviews error", ex);
                return new List<ReviewViewModel>();
            }
        }

        public ReviewStatsViewModel GetBusReviewStats(int busId)
        {
            var stats = new ReviewStatsViewModel
            {
                RatingDistribution = new Dictionary<int, int>()
            };

            try
            {
                var reviews = _unitOfWork.Reviews.GetBusReviews(busId)
                    .Where(r => r.IsApproved)
                    .ToList();

                stats.TotalReviews = reviews.Count;
                stats.AverageRating = reviews.Any()
                    ? Math.Round(reviews.Average(r => (double)r.Rating), 1)
                    : 0.0;
                stats.FiveStarCount = reviews.Count(r => r.Rating == 5);
                stats.FourStarCount = reviews.Count(r => r.Rating == 4);
                stats.ThreeStarCount = reviews.Count(r => r.Rating == 3);
                stats.TwoStarCount = reviews.Count(r => r.Rating == 2);
                stats.OneStarCount = reviews.Count(r => r.Rating == 1);

                stats.RatingDistribution[1] = stats.OneStarCount;
                stats.RatingDistribution[2] = stats.TwoStarCount;
                stats.RatingDistribution[3] = stats.ThreeStarCount;
                stats.RatingDistribution[4] = stats.FourStarCount;
                stats.RatingDistribution[5] = stats.FiveStarCount;
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(ReviewService));
                log.Error("GetBusReviewStats error", ex);
            }

            return stats;
        }

        public void ApproveReview(int reviewId)
        {
            try
            {
                var review = _unitOfWork.Reviews.GetById(reviewId);
                if (review != null)
                {
                    review.IsApproved = true;
                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(ReviewService));
                log.Error("ApproveReview error", ex);
                throw;
            }
        }

        public void ReportAbuse(int reviewId, string reason)
        {
            try
            {
                var review = _unitOfWork.Reviews.GetById(reviewId);
                if (review != null)
                {
                    review.IsAbuseReported = true;
                    review.AbuseReportReason = reason;
                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(ReviewService));
                log.Error("ReportAbuse error", ex);
                throw;
            }
        }

        public void DeleteReview(int reviewId)
        {
            try
            {
                var review = _unitOfWork.Reviews.GetById(reviewId);
                if (review != null)
                {
                    _unitOfWork.Reviews.Delete(review);
                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(ReviewService));
                log.Error("DeleteReview error", ex);
                throw;
            }
        }

        private string GetTimeAgo(DateTime dateTime)
        {
            var span = DateTime.UtcNow - dateTime;
            if (span.TotalDays > 365) return $"{(int)(span.TotalDays / 365)}y ago";
            if (span.TotalDays > 30) return $"{(int)(span.TotalDays / 30)}mo ago";
            if (span.TotalDays > 7) return $"{(int)(span.TotalDays / 7)}w ago";
            if (span.TotalDays > 1) return $"{(int)span.TotalDays}d ago";
            if (span.TotalHours > 1) return $"{(int)span.TotalHours}h ago";
            if (span.TotalMinutes > 1) return $"{(int)span.TotalMinutes}m ago";
            return "just now";
        }
    }
}
