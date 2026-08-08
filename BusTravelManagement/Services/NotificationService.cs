using System;
using System.Collections.Generic;
using System.Linq;
using BusTravelManagement.Models.Entities;
using BusTravelManagement.Models.ViewModels;
using BusTravelManagement.Repositories.Interfaces;
using BusTravelManagement.Services.Interfaces;

namespace BusTravelManagement.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void SendEmail(string to, string subject, string body)
        {
            try
            {
                // Log the email instead of actually sending
                var log = log4net.LogManager.GetLogger(typeof(NotificationService));
                log.Info($"EMAIL TO: {to}");
                log.Info($"SUBJECT: {subject}");
                log.Info($"BODY: {body}");

                // Create a notification record for system tracking
                _unitOfWork.Notifications.Add(new Notification
                {
                    UserId = 0, // System-level notification
                    Title = subject,
                    Message = body,
                    NotificationType = "Email",
                    ReferenceType = "Email",
                    IsRead = false,
                    SentAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                });

                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(NotificationService));
                log.Error("SendEmail error", ex);
            }
        }

        public void SendSms(string phone, string message)
        {
            try
            {
                // Log the SMS instead of actually sending
                var log = log4net.LogManager.GetLogger(typeof(NotificationService));
                log.Info($"SMS TO: {phone}");
                log.Info($"MESSAGE: {message}");

                // Create a notification record for system tracking
                _unitOfWork.Notifications.Add(new Notification
                {
                    UserId = 0,
                    Title = "SMS Notification",
                    Message = message,
                    NotificationType = "SMS",
                    ReferenceType = "SMS",
                    IsRead = false,
                    SentAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                });

                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(NotificationService));
                log.Error("SendSms error", ex);
            }
        }

        public void SendSystemNotification(int userId, string title, string message,
                                            string referenceType = null, int? referenceId = null)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Title = title,
                    Message = message,
                    NotificationType = "System",
                    ReferenceType = referenceType,
                    ReferenceId = referenceId,
                    IsRead = false,
                    SentAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                _unitOfWork.Notifications.Add(notification);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(NotificationService));
                log.Error("SendSystemNotification error", ex);
            }
        }

        public IEnumerable<NotificationViewModel> GetUserNotifications(int userId)
        {
            try
            {
                return _unitOfWork.Notifications.GetUserNotifications(userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .Select(n => new NotificationViewModel
                    {
                        Id = n.Id,
                        Title = n.Title,
                        Message = n.Message,
                        NotificationType = n.NotificationType,
                        ReferenceType = n.ReferenceType,
                        ReferenceId = n.ReferenceId,
                        IsRead = n.IsRead,
                        CreatedAt = n.CreatedAt,
                        TimeAgo = GetTimeAgo(n.CreatedAt)
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(NotificationService));
                log.Error("GetUserNotifications error", ex);
                return new List<NotificationViewModel>();
            }
        }

        public int GetUnreadCount(int userId)
        {
            try
            {
                return _unitOfWork.Notifications.GetUnreadCount(userId);
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(NotificationService));
                log.Error("GetUnreadCount error", ex);
                return 0;
            }
        }

        public void MarkAsRead(int notificationId)
        {
            try
            {
                var notification = _unitOfWork.Notifications.GetById(notificationId);
                if (notification != null && !notification.IsRead)
                {
                    notification.IsRead = true;
                    notification.ReadAt = DateTime.UtcNow;
                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(NotificationService));
                log.Error("MarkAsRead error", ex);
            }
        }

        public void MarkAllAsRead(int userId)
        {
            try
            {
                var unreadNotifications = _unitOfWork.Notifications.GetUserNotifications(userId)
                    .Where(n => !n.IsRead)
                    .ToList();

                foreach (var notification in unreadNotifications)
                {
                    notification.IsRead = true;
                    notification.ReadAt = DateTime.UtcNow;
                }

                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(NotificationService));
                log.Error("MarkAllAsRead error", ex);
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
