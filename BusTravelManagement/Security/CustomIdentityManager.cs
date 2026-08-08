using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace BusTravelManagement.Security
{
    public class CustomIdentity : IIdentity
    {
        public string Name { get; }
        public string AuthenticationType => "Forms";
        public bool IsAuthenticated => !string.IsNullOrEmpty(Name);
        public string[] Roles { get; }
        public int UserId { get; }

        public CustomIdentity(string name, string[] roles, int userId)
        {
            Name = name;
            Roles = roles;
            UserId = userId;
        }

        public bool IsInRole(string role)
        {
            return Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
        }
    }

    public static class CustomIdentityManager
    {
        public static CustomIdentity GetCurrentIdentity()
        {
            var context = HttpContext.Current;
            if (context?.User?.Identity?.IsAuthenticated != true)
                return null;

            var authCookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null)
                return null;

            try
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                if (ticket == null)
                    return null;

                var roles = ticket.UserData.Split(',');
                var userId = ExtractUserId(ticket.Name);

                return new CustomIdentity(ticket.Name, roles, userId);
            }
            catch
            {
                return null;
            }
        }

        public static bool IsAuthenticated()
        {
            return HttpContext.Current?.User?.Identity?.IsAuthenticated ?? false;
        }

        public static bool IsInRole(string role)
        {
            var identity = GetCurrentIdentity();
            return identity?.IsInRole(role) ?? false;
        }

        public static bool IsAdmin()
        {
            return IsInRole("Admin") || IsInRole("SuperAdmin");
        }

        public static bool IsOperator()
        {
            return IsInRole("Operator");
        }

        public static bool IsCustomer()
        {
            return IsInRole("Customer");
        }

        private static int ExtractUserId(string email)
        {
            // This is a simplified approach. In production, cache user ID in the auth ticket.
            return 0;
        }
    }
}
