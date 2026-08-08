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
    public class AuditService : IAuditService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuditService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Log(string action, string entityType, int? entityId, int? userId,
                        string oldValues = null, string newValues = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    Action = action,
                    EntityType = entityType,
                    EntityId = entityId,
                    UserId = userId,
                    OldValues = oldValues,
                    NewValues = newValues,
                    IpAddress = GetClientIpAddress(),
                    UserAgent = GetUserAgent(),
                    CreatedAt = DateTime.UtcNow
                };

                _unitOfWork.AuditLogs.Add(auditLog);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(AuditService));
                log.Error("Log audit error", ex);
            }
        }

        public IEnumerable<AuditLogViewModel> GetLogs(int page = 1, int pageSize = 50)
        {
            try
            {
                return _unitOfWork.Query<AuditLog>()
                    .OrderByDescending(a => a.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()
                    .Select(a => new AuditLogViewModel
                    {
                        Id = a.Id,
                        Action = a.Action,
                        EntityType = a.EntityType,
                        EntityId = a.EntityId,
                        UserName = GetUserName(a.UserId),
                        IpAddress = a.IpAddress,
                        UserAgent = a.UserAgent,
                        CreatedAt = a.CreatedAt
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(AuditService));
                log.Error("GetLogs error", ex);
                return new List<AuditLogViewModel>();
            }
        }

        public IEnumerable<AuditLogViewModel> GetLogsByUser(int userId, int page = 1, int pageSize = 50)
        {
            try
            {
                return _unitOfWork.Query<AuditLog>()
                    .Where(a => a.UserId == userId)
                    .OrderByDescending(a => a.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()
                    .Select(a => new AuditLogViewModel
                    {
                        Id = a.Id,
                        Action = a.Action,
                        EntityType = a.EntityType,
                        EntityId = a.EntityId,
                        UserName = GetUserName(a.UserId),
                        IpAddress = a.IpAddress,
                        UserAgent = a.UserAgent,
                        CreatedAt = a.CreatedAt
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(AuditService));
                log.Error("GetLogsByUser error", ex);
                return new List<AuditLogViewModel>();
            }
        }

        public IEnumerable<AuditLogViewModel> GetLogsByEntity(string entityType, int entityId)
        {
            try
            {
                return _unitOfWork.Query<AuditLog>()
                    .Where(a => a.EntityType == entityType && a.EntityId == entityId)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToList()
                    .Select(a => new AuditLogViewModel
                    {
                        Id = a.Id,
                        Action = a.Action,
                        EntityType = a.EntityType,
                        EntityId = a.EntityId,
                        UserName = GetUserName(a.UserId),
                        IpAddress = a.IpAddress,
                        UserAgent = a.UserAgent,
                        CreatedAt = a.CreatedAt
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(AuditService));
                log.Error("GetLogsByEntity error", ex);
                return new List<AuditLogViewModel>();
            }
        }

        private string GetUserName(int? userId)
        {
            if (!userId.HasValue || userId.Value <= 0) return "System";

            try
            {
                var user = _unitOfWork.Users.GetById(userId.Value);
                return user?.FullName ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }

        private string GetClientIpAddress()
        {
            try
            {
                if (System.Web.HttpContext.Current?.Request != null)
                {
                    var request = System.Web.HttpContext.Current.Request;
                    var ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (!string.IsNullOrEmpty(ip))
                    {
                        var ips = ip.Split(',');
                        return ips[0].Trim();
                    }
                    return request.ServerVariables["REMOTE_ADDR"] ?? request.UserHostAddress;
                }
            }
            catch
            {
                // Silently fail on IP retrieval
            }

            return null;
        }

        private string GetUserAgent()
        {
            try
            {
                return System.Web.HttpContext.Current?.Request?.UserAgent;
            }
            catch
            {
                return null;
            }
        }
    }
}
