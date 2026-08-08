using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BusTravelManagement.Models.Entities;

namespace BusTravelManagement.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        T GetById(int id);
        Task<T> GetByIdAsync(int id);
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsync();
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        bool Any(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        int Count(Expression<Func<T, bool>> predicate = null);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);
        IQueryable<T> Query();
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Update(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        void Attach(T entity);
    }

    public interface IUserRepository : IRepository<User>
    {
        User GetByEmail(string email);
        User GetByResetToken(string token);
        bool IsEmailRegistered(string email);
    }

    public interface IOperatorRepository : IRepository<Operator>
    {
        Operator GetByUserId(int userId);
    }

    public interface IBusRepository : IRepository<Bus>
    {
        IQueryable<Bus> GetBusesWithOperator();
    }

    public interface ICityRepository : IRepository<City>
    {
        IEnumerable<City> SearchCities(string searchTerm);
        IEnumerable<City> GetPopularCities();
    }

    public interface IRouteRepository : IRepository<Route>
    {
        IQueryable<Route> GetActiveRoutes();
    }

    public interface IBookingRepository : IRepository<Booking>
    {
        Booking GetByBookingNumber(string bookingNumber);
        IQueryable<Booking> GetUserBookings(int userId);
    }

    public interface ICouponRepository : IRepository<Coupon>
    {
        Coupon GetByCode(string code);
    }

    public interface IReviewRepository : IRepository<Review>
    {
        IQueryable<Review> GetBusReviews(int busId);
    }

    public interface INotificationRepository : IRepository<Notification>
    {
        IEnumerable<Notification> GetUserNotifications(int userId);
        int GetUnreadCount(int userId);
    }

    public interface IPaymentRepository : IRepository<Payment>
    {
        Payment GetByPaymentReference(string reference);
        IEnumerable<Payment> GetBookingPayments(int bookingId);
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

    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IBusRepository Buses { get; }
        IBookingRepository Bookings { get; }
        IRouteRepository Routes { get; }
        ICityRepository Cities { get; }
        ICouponRepository Coupons { get; }
        IReviewRepository Reviews { get; }
        INotificationRepository Notifications { get; }
        IOperatorRepository Operators { get; }
        IPaymentRepository Payments { get; }
        ITicketRepository Tickets { get; }
        IAuditLogRepository AuditLogs { get; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();

        // Generic query access for entities without specific repositories
        IDbSet<T> Query<T>() where T : class;
    }
}
