using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace BusTravelManagement.Repositories.Interfaces
{
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

        IDbSet<T> Query<T>() where T : class;

        int SaveChanges();
        Task<int> SaveChangesAsync();
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
    }
}
