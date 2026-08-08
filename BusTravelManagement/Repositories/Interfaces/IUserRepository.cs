using BusTravelManagement.Models.Entities;

namespace BusTravelManagement.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        User GetByEmail(string email);
        User GetByResetToken(string token);
        bool IsEmailRegistered(string email);
    }

    public interface IBusRepository : IRepository<Bus>
    {
        System.Linq.IQueryable<Bus> GetBusesWithOperator();
    }

    public interface IBookingRepository : IRepository<Booking>
    {
        Booking GetByBookingNumber(string bookingNumber);
        System.Linq.IQueryable<Booking> GetUserBookings(int userId);
    }

    public interface IRouteRepository : IRepository<Route>
    {
        System.Linq.IQueryable<Route> GetActiveRoutes();
    }

    public interface ICityRepository : IRepository<City>
    {
        System.Collections.Generic.IEnumerable<City> SearchCities(string searchTerm);
        System.Collections.Generic.IEnumerable<City> GetPopularCities();
    }

    public interface ICouponRepository : IRepository<Coupon>
    {
        Coupon GetByCode(string code);
    }

    public interface IReviewRepository : IRepository<Review>
    {
        System.Linq.IQueryable<Review> GetBusReviews(int busId);
    }

    public interface INotificationRepository : IRepository<Notification>
    {
        System.Collections.Generic.IEnumerable<Notification> GetUserNotifications(int userId);
        int GetUnreadCount(int userId);
    }

    public interface IOperatorRepository : IRepository<Operator>
    {
        Operator GetByUserId(int userId);
    }

    public interface IPaymentRepository : IRepository<Payment>
    {
        Payment GetByPaymentReference(string reference);
        System.Collections.Generic.IEnumerable<Payment> GetBookingPayments(int bookingId);
    }

    public interface ITicketRepository : IRepository<Ticket>
    {
        Ticket GetByTicketNumber(string ticketNumber);
        Ticket GetByBookingId(int bookingId);
    }

    public interface IAuditLogRepository : IRepository<AuditLog>
    {
        void LogAction(string action, string entityType, int? entityId, int? userId,
                       string oldValues, string newValues, string ipAddress, string userAgent);
    }
}
