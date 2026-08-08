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
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public AdminDashboardViewModel GetDashboard()
        {
            var dashboard = new AdminDashboardViewModel
            {
                RevenueChartData = new List<ChartDataPoint>(),
                BookingChartData = new List<ChartDataPoint>(),
                RecentBookings = new List<RecentBookingViewModel>()
            };

            try
            {
                var today = DateTime.UtcNow.Date;
                var monthStart = new DateTime(today.Year, today.Month, 1);
                var yearStart = new DateTime(today.Year, 1, 1);

                dashboard.TotalUsers = _unitOfWork.Query<User>().Count(u => u.IsActive);
                dashboard.TotalOperators = _unitOfWork.Query<Operator>().Count();
                dashboard.TotalBuses = _unitOfWork.Query<Bus>().Count(b => !b.IsDeleted);
                dashboard.TotalBookings = _unitOfWork.Query<Booking>().Count();
                var todayEnd = today.AddDays(1);
                dashboard.TotalBookingsToday = _unitOfWork.Query<Booking>()
                    .Count(b => b.CreatedAt >= today && b.CreatedAt < todayEnd);
                dashboard.PendingReviews = _unitOfWork.Query<Review>()
                    .Count(r => !r.IsApproved && !r.IsAbuseReported);
                dashboard.PendingOperators = _unitOfWork.Query<Operator>()
                    .Count(o => !o.IsVerified);

                dashboard.TodayRevenue = _unitOfWork.Query<Payment>()
                    .Where(p => p.PaymentStatus == "Success"
                             && p.PaidAt >= today && p.PaidAt < todayEnd)
                    .Sum(p => (decimal?)p.Amount) ?? 0;

                dashboard.MonthlyRevenue = _unitOfWork.Query<Payment>()
                    .Where(p => p.PaymentStatus == "Success"
                             && p.PaidAt >= monthStart)
                    .Sum(p => (decimal?)p.Amount) ?? 0;

                dashboard.YearlyRevenue = _unitOfWork.Query<Payment>()
                    .Where(p => p.PaymentStatus == "Success"
                             && p.PaidAt >= yearStart)
                    .Sum(p => (decimal?)p.Amount) ?? 0;

                // Revenue chart - last 12 months
                for (int i = 11; i >= 0; i--)
                {
                    var monthDate = today.AddMonths(-i);
                    var monthStartDate = new DateTime(monthDate.Year, monthDate.Month, 1);
                    var monthEndDate = monthStartDate.AddMonths(1).AddDays(-1);

                    var revenue = _unitOfWork.Query<Payment>()
                        .Where(p => p.PaymentStatus == "Success"
                                 && p.PaidAt >= monthStartDate
                                 && p.PaidAt <= monthEndDate)
                        .Sum(p => (decimal?)p.Amount) ?? 0;

                    dashboard.RevenueChartData.Add(new ChartDataPoint
                    {
                        Label = monthStartDate.ToString("MMM yyyy"),
                        Value = revenue
                    });
                }

                // Booking chart - last 12 months
                for (int i = 11; i >= 0; i--)
                {
                    var monthDate = today.AddMonths(-i);
                    var monthStartDate = new DateTime(monthDate.Year, monthDate.Month, 1);
                    var monthEndDate = monthStartDate.AddMonths(1).AddDays(-1);

                    var count = _unitOfWork.Query<Booking>()
                        .Count(b => b.CreatedAt >= monthStartDate && b.CreatedAt <= monthEndDate);

                    dashboard.BookingChartData.Add(new ChartDataPoint
                    {
                        Label = monthStartDate.ToString("MMM yyyy"),
                        Value = count
                    });
                }

                // Recent bookings
                var recentBookings = _unitOfWork.Query<Booking>()
                    .Include(b => b.User)
                    .Include(b => b.Schedule.Route.SourceCity)
                    .Include(b => b.Schedule.Route.DestinationCity)
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
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(AdminService));
                log.Error("GetDashboard error", ex);
            }

            return dashboard;
        }

        public IEnumerable<UserListViewModel> GetUsers(int page = 1, int pageSize = 20)
        {
            try
            {
                return _unitOfWork.Query<User>()
                    .Include(u => u.UserRoles.Select(ur => ur.Role))
                    .OrderByDescending(u => u.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()
                    .Select(u => new UserListViewModel
                    {
                        Id = u.Id,
                        Name = u.FullName,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber,
                        Roles = string.Join(", ", u.UserRoles.Select(ur => ur.Role.Name)),
                        IsActive = u.IsActive,
                        IsLocked = u.IsLocked,
                        IsEmailVerified = u.IsEmailVerified,
                        CreatedAt = u.CreatedAt,
                        LastLoginAt = u.LastLoginAt
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(AdminService));
                log.Error("GetUsers error", ex);
                return new List<UserListViewModel>();
            }
        }

        public IEnumerable<OperatorListViewModel> GetOperators(int page = 1, int pageSize = 20)
        {
            try
            {
                return _unitOfWork.Query<Operator>()
                    .Include(o => o.Buses)
                    .OrderByDescending(o => o.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()
                    .Select(o => new OperatorListViewModel
                    {
                        Id = o.Id,
                        CompanyName = o.CompanyName,
                        ContactPerson = o.ContactPerson,
                        ContactEmail = o.ContactEmail,
                        ContactPhone = o.ContactPhone,
                        City = o.City,
                        BusCount = o.Buses.Count(b => !b.IsDeleted),
                        IsActive = o.IsActive,
                        IsVerified = o.IsVerified,
                        Commission = o.CommissionPercentage,
                        CreatedAt = o.CreatedAt
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(AdminService));
                log.Error("GetOperators error", ex);
                return new List<OperatorListViewModel>();
            }
        }

        public IEnumerable<BusListViewModel> GetAllBuses(int page = 1, int pageSize = 20)
        {
            try
            {
                return _unitOfWork.Query<Bus>()
                    .Include(b => b.BusType)
                    .Include(b => b.Operator)
                    .Include(b => b.Schedules)
                    .Where(b => !b.IsDeleted)
                    .OrderByDescending(b => b.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()
                    .Select(b => new BusListViewModel
                    {
                        Id = b.Id,
                        BusNumber = b.BusNumber,
                        RegistrationNumber = b.RegistrationNumber,
                        BusType = b.BusType?.Name ?? "Standard",
                        TotalSeats = b.TotalSeats,
                        SeatLayoutType = b.SeatLayoutType,
                        IsAC = b.IsAC,
                        IsSleeper = b.IsSleeper,
                        IsActive = b.IsActive,
                        ScheduleCount = b.Schedules.Count(s => s.IsActive)
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(AdminService));
                log.Error("GetAllBuses error", ex);
                return new List<BusListViewModel>();
            }
        }

        public void ApproveOperator(int operatorId)
        {
            try
            {
                var oper = _unitOfWork.Operators.GetById(operatorId);
                if (oper != null)
                {
                    oper.IsVerified = true;
                    oper.IsActive = true;
                    oper.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(AdminService));
                log.Error("ApproveOperator error", ex);
                throw;
            }
        }

        public void SuspendUser(int userId)
        {
            try
            {
                var user = _unitOfWork.Users.GetById(userId);
                if (user != null)
                {
                    user.IsActive = false;
                    user.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(AdminService));
                log.Error("SuspendUser error", ex);
                throw;
            }
        }

        public void ActivateUser(int userId)
        {
            try
            {
                var user = _unitOfWork.Users.GetById(userId);
                if (user != null)
                {
                    user.IsActive = true;
                    user.IsLocked = false;
                    user.FailedLoginAttempts = 0;
                    user.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(AdminService));
                log.Error("ActivateUser error", ex);
                throw;
            }
        }

        public void AssignRole(int userId, string roleName)
        {
            try
            {
                var user = _unitOfWork.Users.GetById(userId);
                if (user == null) throw new InvalidOperationException("User not found.");

                var role = _unitOfWork.Query<Role>()
                    .FirstOrDefault(r => r.Name == roleName && r.IsActive);

                if (role == null) throw new InvalidOperationException("Role not found.");

                var existing = _unitOfWork.Query<UserRole>()
                    .FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == role.Id);

                if (existing == null)
                {
                    _unitOfWork.Query<UserRole>().Add(new UserRole
                    {
                        UserId = userId,
                        RoleId = role.Id,
                        CreatedAt = DateTime.UtcNow
                    });
                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(AdminService));
                log.Error("AssignRole error", ex);
                throw;
            }
        }

        public void RemoveRole(int userId, string roleName)
        {
            try
            {
                var role = _unitOfWork.Query<Role>()
                    .FirstOrDefault(r => r.Name == roleName);

                if (role == null) return;

                var userRole = _unitOfWork.Query<UserRole>()
                    .FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == role.Id);

                if (userRole != null)
                {
                    _unitOfWork.Query<UserRole>().Remove(userRole);
                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(AdminService));
                log.Error("RemoveRole error", ex);
                throw;
            }
        }

        public IEnumerable<CouponViewModel> GetCoupons()
        {
            try
            {
                return _unitOfWork.Query<Coupon>()
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new CouponViewModel
                    {
                        Id = c.Id,
                        Code = c.Code,
                        Description = c.Description,
                        DiscountType = c.DiscountType,
                        DiscountValue = c.DiscountValue,
                        MinBookingAmount = c.MinBookingAmount,
                        MaxDiscountAmount = c.MaxDiscountAmount,
                        UsageLimit = c.UsageLimit,
                        UsedCount = c.UsedCount,
                        IsActive = c.IsActive,
                        ValidFrom = c.ValidFrom,
                        ValidTo = c.ValidTo
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(AdminService));
                log.Error("GetCoupons error", ex);
                return new List<CouponViewModel>();
            }
        }

        public void CreateCoupon(CreateCouponViewModel model)
        {
            try
            {
                var coupon = new Coupon
                {
                    Code = model.Code.Trim().ToUpper(),
                    Description = model.Description?.Trim(),
                    DiscountType = model.DiscountType,
                    DiscountValue = model.DiscountValue,
                    MinBookingAmount = model.MinBookingAmount,
                    MaxDiscountAmount = model.MaxDiscountAmount,
                    UsageLimit = model.UsageLimit,
                    PerUserLimit = model.PerUserLimit,
                    IsActive = true,
                    ValidFrom = model.ValidFrom,
                    ValidTo = model.ValidTo,
                    UsedCount = 0,
                    CreatedAt = DateTime.UtcNow
                };

                _unitOfWork.Coupons.Add(coupon);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(AdminService));
                log.Error("CreateCoupon error", ex);
                throw;
            }
        }

        public void ToggleCouponStatus(int couponId)
        {
            try
            {
                var coupon = _unitOfWork.Coupons.GetById(couponId);
                if (coupon != null)
                {
                    coupon.IsActive = !coupon.IsActive;
                    coupon.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(AdminService));
                log.Error("ToggleCouponStatus error", ex);
                throw;
            }
        }

        public IEnumerable<ReviewViewModel> GetPendingReviews()
        {
            try
            {
                return _unitOfWork.Query<Review>()
                    .Include(r => r.User)
                    .Include(r => r.Bus)
                    .Where(r => !r.IsApproved && !r.IsAbuseReported)
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
                var log = log4net.LogManager.GetLogger(typeof(AdminService));
                log.Error("GetPendingReviews error", ex);
                return new List<ReviewViewModel>();
            }
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
                var log = log4net.LogManager.GetLogger(typeof(AdminService));
                log.Error("ApproveReview error", ex);
                throw;
            }
        }

        public void RejectReview(int reviewId)
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
                var log = log4net.LogManager.GetLogger(typeof(AdminService));
                log.Error("RejectReview error", ex);
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
