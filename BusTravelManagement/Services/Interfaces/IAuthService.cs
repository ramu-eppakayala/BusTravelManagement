using BusTravelManagement.Models.ViewModels;

namespace BusTravelManagement.Services.Interfaces
{
    public interface IAuthService
    {
        LoginResult Login(LoginViewModel model);
        RegisterResult Register(RegisterViewModel model);
        LogoutResult Logout();
        ForgotPasswordResult ForgotPassword(ForgotPasswordViewModel model);
        ResetPasswordResult ResetPassword(ResetPasswordViewModel model);
        VerifyEmailResult VerifyEmail(string token);
        UserProfileViewModel GetProfile(int userId);
        UpdateProfileResult UpdateProfile(int userId, UserProfileViewModel model);
        ChangePasswordResult ChangePassword(int userId, ChangePasswordViewModel model);
        int? GetCurrentUserId();
        string GetCurrentUserEmail();
        bool IsUserInRole(int userId, string roleName);
        bool IsEmailVerified(int userId);
    }

    public class LoginResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public System.Collections.Generic.List<string> Roles { get; set; }
        public bool IsLocked { get; set; }
        public bool EmailNotVerified { get; set; }
    }

    public class RegisterResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? UserId { get; set; }
        public System.Collections.Generic.List<string> Errors { get; set; }
    }

    public class LogoutResult
    {
        public bool Success { get; set; }
    }

    public class ForgotPasswordResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class ResetPasswordResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class VerifyEmailResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class UpdateProfileResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class ChangePasswordResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
