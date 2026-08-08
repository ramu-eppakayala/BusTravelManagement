using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using BusTravelManagement.Models;
using BusTravelManagement.Repositories.Interfaces;

namespace BusTravelManagement.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BusTravelDbContext _context;
        private DbContextTransaction _transaction;
        private bool _disposed = false;

        private IUserRepository _userRepository;
        private IBusRepository _busRepository;
        private IBookingRepository _bookingRepository;
        private IRouteRepository _routeRepository;
        private ICityRepository _cityRepository;
        private ICouponRepository _couponRepository;
        private IReviewRepository _reviewRepository;
        private INotificationRepository _notificationRepository;
        private IOperatorRepository _operatorRepository;
        private IPaymentRepository _paymentRepository;
        private ITicketRepository _ticketRepository;
        private IAuditLogRepository _auditLogRepository;

        public UnitOfWork(BusTravelDbContext context)
        {
            _context = context;
        }

        public IUserRepository Users =>
            _userRepository ?? (_userRepository = new UserRepository(_context));

        public IBusRepository Buses =>
            _busRepository ?? (_busRepository = new BusRepository(_context));

        public IBookingRepository Bookings =>
            _bookingRepository ?? (_bookingRepository = new BookingRepository(_context));

        public IRouteRepository Routes =>
            _routeRepository ?? (_routeRepository = new RouteRepository(_context));

        public ICityRepository Cities =>
            _cityRepository ?? (_cityRepository = new CityRepository(_context));

        public ICouponRepository Coupons =>
            _couponRepository ?? (_couponRepository = new CouponRepository(_context));

        public IReviewRepository Reviews =>
            _reviewRepository ?? (_reviewRepository = new ReviewRepository(_context));

        public INotificationRepository Notifications =>
            _notificationRepository ?? (_notificationRepository = new NotificationRepository(_context));

        public IOperatorRepository Operators =>
            _operatorRepository ?? (_operatorRepository = new OperatorRepository(_context));

        public IPaymentRepository Payments =>
            _paymentRepository ?? (_paymentRepository = new PaymentRepository(_context));

        public ITicketRepository Tickets =>
            _ticketRepository ?? (_ticketRepository = new TicketRepository(_context));

        public IAuditLogRepository AuditLogs =>
            _auditLogRepository ?? (_auditLogRepository = new AuditLogRepository(_context));

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _transaction?.Commit();
        }

        public void RollbackTransaction()
        {
            _transaction?.Rollback();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IDbSet<T> Query<T>() where T : class
        {
            return _context.Set<T>();
        }
    }

    public static class DbSetExtensions
    {
        public static T Delete<T>(this IDbSet<T> dbSet, T entity) where T : class
        {
            return dbSet.Remove(entity);
        }
    }
}
