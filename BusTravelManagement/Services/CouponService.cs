using System;
using System.Linq;
using BusTravelManagement.Models.Entities;
using BusTravelManagement.Models.ViewModels;
using BusTravelManagement.Repositories.Interfaces;
using BusTravelManagement.Services.Interfaces;

namespace BusTravelManagement.Services
{
    public class CouponService : ICouponService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CouponService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public CouponValidationResult ValidateCoupon(string code, decimal bookingAmount, int userId)
        {
            var result = new CouponValidationResult
            {
                IsValid = false,
                Message = "Invalid coupon code."
            };

            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    result.Message = "Please enter a coupon code.";
                    return result;
                }

                var coupon = _unitOfWork.Coupons.GetByCode(code.Trim().ToUpper());

                if (coupon == null)
                {
                    result.Message = "Invalid coupon code.";
                    return result;
                }

                if (!coupon.IsActive)
                {
                    result.Message = "This coupon is no longer active.";
                    return result;
                }

                var now = DateTime.UtcNow;
                if (now < coupon.ValidFrom)
                {
                    result.Message = "This coupon is not yet valid.";
                    return result;
                }

                if (now > coupon.ValidTo)
                {
                    result.Message = "This coupon has expired.";
                    return result;
                }

                // Check overall usage limit
                if (coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit.Value)
                {
                    result.Message = "This coupon has reached its usage limit.";
                    return result;
                }

                // Check per-user usage limit
                if (coupon.PerUserLimit > 0 && userId > 0)
                {
                    var userUsageCount = _unitOfWork.Query<Booking>()
                        .Count(b => b.UserId == userId && b.CouponId == coupon.Id
                                 && b.BookingStatus != "Cancelled");

                    if (userUsageCount >= coupon.PerUserLimit)
                    {
                        result.Message = "You have already used this coupon the maximum number of times.";
                        return result;
                    }
                }

                // Check minimum booking amount
                if (coupon.MinBookingAmount.HasValue && bookingAmount < coupon.MinBookingAmount.Value)
                {
                    result.Message = $"Minimum booking amount of {coupon.MinBookingAmount.Value:C} is required for this coupon.";
                    return result;
                }

                // Calculate discount
                decimal discountAmount = 0;

                if (coupon.DiscountType == "Percentage")
                {
                    discountAmount = Math.Round(bookingAmount * coupon.DiscountValue / 100m, 2);

                    // Apply max discount cap
                    if (coupon.MaxDiscountAmount.HasValue && discountAmount > coupon.MaxDiscountAmount.Value)
                    {
                        discountAmount = coupon.MaxDiscountAmount.Value;
                    }
                }
                else if (coupon.DiscountType == "Flat")
                {
                    discountAmount = coupon.DiscountValue;

                    // Discount should not exceed booking amount
                    if (discountAmount > bookingAmount)
                    {
                        discountAmount = bookingAmount;
                    }
                }

                result.IsValid = true;
                result.Message = "Coupon applied successfully.";
                result.DiscountType = coupon.DiscountType;
                result.DiscountValue = coupon.DiscountValue;
                result.MaxDiscountAmount = coupon.MaxDiscountAmount ?? 0;
                result.DiscountAmount = discountAmount;
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(CouponService));
                log.Error("ValidateCoupon error", ex);
                result.Message = "An error occurred while validating the coupon.";
            }

            return result;
        }
    }
}
