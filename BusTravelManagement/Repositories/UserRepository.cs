using System.Linq;
using BusTravelManagement.Models;
using BusTravelManagement.Models.Entities;
using BusTravelManagement.Repositories.Interfaces;

namespace BusTravelManagement.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(BusTravelDbContext context) : base(context) { }

        public User GetByEmail(string email)
        {
            return DbSet.FirstOrDefault(u => u.Email == email && u.IsActive);
        }

        public User GetByResetToken(string token)
        {
            return DbSet.FirstOrDefault(u => u.ResetPasswordToken == token
                && u.ResetPasswordTokenExpiry > System.DateTime.UtcNow);
        }

        public bool IsEmailRegistered(string email)
        {
            return DbSet.Any(u => u.Email == email);
        }
    }

    public class OperatorRepository : BaseRepository<Operator>, IOperatorRepository
    {
        public OperatorRepository(BusTravelDbContext context) : base(context) { }

        public Operator GetByUserId(int userId)
        {
            return DbSet.FirstOrDefault(o => o.UserId == userId);
        }
    }

    public class BusRepository : BaseRepository<Bus>, IBusRepository
    {
        public BusRepository(BusTravelDbContext context) : base(context) { }

        public IQueryable<Bus> GetBusesWithOperator()
        {
            return DbSet.Include("Operator").Include("BusType");
        }
    }

    public class CityRepository : BaseRepository<City>, ICityRepository
    {
        public CityRepository(BusTravelDbContext context) : base(context) { }

        public System.Collections.Generic.IEnumerable<City> SearchCities(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new System.Collections.Generic.List<City>();

            return DbSet.Where(c => c.IsActive &&
                (c.Name.Contains(searchTerm) || c.State.Contains(searchTerm)))
                .OrderBy(c => c.Name)
                .Take(10)
                .ToList();
        }

        public System.Collections.Generic.IEnumerable<City> GetPopularCities()
        {
            return DbSet.Where(c => c.IsActive && c.IsPopular)
                .OrderBy(c => c.Name)
                .ToList();
        }
    }

    public class RouteRepository : BaseRepository<Route>, IRouteRepository
    {
        public RouteRepository(BusTravelDbContext context) : base(context) { }

        public IQueryable<Route> GetActiveRoutes()
        {
            return DbSet.Include("SourceCity").Include("DestinationCity")
                .Where(r => r.IsActive);
        }
    }

    public class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {
        public BookingRepository(BusTravelDbContext context) : base(context) { }

        public Booking GetByBookingNumber(string bookingNumber)
        {
            return DbSet.Include("Schedule.Bus")
                .Include("BookingPassengers")
                .Include("Payments")
                .Include("Tickets")
                .Include("SourceStop")
                .Include("DestinationStop")
                .Include("BoardingPoint")
                .Include("DroppingPoint")
                .FirstOrDefault(b => b.BookingNumber == bookingNumber);
        }

        public IQueryable<Booking> GetUserBookings(int userId)
        {
            return DbSet.Include("Schedule.Bus")
                .Include("BookingPassengers")
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt);
        }
    }

    public class CouponRepository : BaseRepository<Coupon>, ICouponRepository
    {
        public CouponRepository(BusTravelDbContext context) : base(context) { }

        public Coupon GetByCode(string code)
        {
            return DbSet.FirstOrDefault(c => c.Code == code && c.IsActive
                && c.ValidFrom <= System.DateTime.UtcNow
                && c.ValidTo >= System.DateTime.UtcNow);
        }
    }

    public class ReviewRepository : BaseRepository<Review>, IReviewRepository
    {
        public ReviewRepository(BusTravelDbContext context) : base(context) { }

        public IQueryable<Review> GetBusReviews(int busId)
        {
            return DbSet.Include("User")
                .Where(r => r.BusId == busId && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt);
        }
    }

    public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(BusTravelDbContext context) : base(context) { }

        public System.Collections.Generic.IEnumerable<Notification> GetUserNotifications(int userId)
        {
            return DbSet.Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(50)
                .ToList();
        }

        public int GetUnreadCount(int userId)
        {
            return DbSet.Count(n => n.UserId == userId && !n.IsRead);
        }
    }

    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(BusTravelDbContext context) : base(context) { }

        public Payment GetByPaymentReference(string reference)
        {
            return DbSet.FirstOrDefault(p => p.PaymentReference == reference);
        }

        public System.Collections.Generic.IEnumerable<Payment> GetBookingPayments(int bookingId)
        {
            return DbSet.Where(p => p.BookingId == bookingId)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
        }
    }

    public class TicketRepository : BaseRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(BusTravelDbContext context) : base(context) { }

        public Ticket GetByTicketNumber(string ticketNumber)
        {
            return DbSet.FirstOrDefault(t => t.TicketNumber == ticketNumber);
        }

        public Ticket GetByBookingId(int bookingId)
        {
            return DbSet.FirstOrDefault(t => t.BookingId == bookingId);
        }
    }

    public class AuditLogRepository : BaseRepository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(BusTravelDbContext context) : base(context) { }

        public void LogAction(string action, string entityType, int? entityId, int? userId,
                             string oldValues, string newValues, string ipAddress, string userAgent)
        {
            var log = new AuditLog
            {
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                UserId = userId,
                OldValues = oldValues,
                NewValues = newValues,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                CreatedAt = System.DateTime.UtcNow
            };
            DbSet.Add(log);
        }
    }
}
