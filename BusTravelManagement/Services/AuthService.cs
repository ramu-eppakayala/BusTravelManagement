using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Security.Claims;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using BusTravelManagement.Models.Entities;
using BusTravelManagement.Models.ViewModels;
using BusTravelManagement.Repositories.Interfaces;
using BusTravelManagement.Security;
using BusTravelManagement.Services.Interfaces;

namespace BusTravelManagement.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public LoginResult Login(LoginViewModel model)
        {
            var result = new LoginResult { Success = false };

            try
            {
                var user = _unitOfWork.Users.GetByEmail(model.Email.Trim().ToLower());

                if (user == null)
                {
                    result.Message = "Invalid email or password.";
                    return result;
                }

                if (!user.IsActive)
                {
                    result.Message = "Your account has been deactivated. Please contact support.";
                    return result;
                }

                if (user.IsLocked)
                {
                    result.Message = "Your account has been locked due to multiple failed login attempts. Please try again later.";
                    return result;
                }

                if (!PasswordHelper.VerifyPassword(model.Password, user.PasswordHash))
                {
                    user.FailedLoginAttempts++;
                    if (user.FailedLoginAttempts >= 5)
                    {
                        user.IsLocked = true;
                    }
                    try { _unitOfWork.SaveChanges(); } catch { }
                    result.Message = "Invalid email or password.";
                    return result;
                }

                // Check email verification
                if (!user.IsEmailVerified)
                {
                    result.EmailNotVerified = true;
                    result.Message = "Please verify your email address before logging in.";
                    return result;
                }

                // Successful login — update audit fields; failure here must not block auth
                user.FailedLoginAttempts = 0;
                user.LastLoginAt = DateTime.UtcNow;
                try { _unitOfWork.SaveChanges(); } catch { }

                // Set forms auth cookie
                var roles = _unitOfWork.Users.Query()
                    .Where(u => u.Id == user.Id)
                    .SelectMany(u => u.UserRoles)
                    .Select(ur => ur.Role.Name)
                    .ToList();


                // Create OWIN claims identity and sign in via OWIN middleware
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };
                claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationType);

                var authManager = HttpContext.Current.GetOwinContext().Authentication;
                var authProps = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(14) : DateTimeOffset.UtcNow.AddMinutes(30)
                };

                authManager.SignIn(authProps, identity);

                result.Success = true;
                result.UserId = user.Id;
                result.UserName = user.FullName;
                result.Email = user.Email;
                result.Roles = roles;
                result.Message = "Login successful.";
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(AuthService));
                log.Error("Login error", ex);
                result.Message = "An error occurred during login. Please try again.";
            }

            return result;
        }

        public RegisterResult Register(RegisterViewModel model)
        {
            var result = new RegisterResult { Success = false, Errors = new List<string>() };

            try
            {
                // Validate
                if (_unitOfWork.Users.IsEmailRegistered(model.Email.Trim().ToLower()))
                {
                    result.Errors.Add("Email is already registered.");
                    return result;
                }

                var user = new User
                {
                    FirstName = model.FirstName.Trim(),
                    LastName = model.LastName.Trim(),
                    Email = model.Email.Trim().ToLower(),
                    PasswordHash = PasswordHelper.HashPassword(model.Password),
                    PhoneNumber = model.PhoneNumber?.Trim(),
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    IsEmailVerified = false,
                    EmailVerificationToken = TokenHelper.GenerateEmailVerificationToken(),
                    IsActive = true,
                    IsLocked = false,
                    FailedLoginAttempts = 0,
                    CreatedAt = DateTime.UtcNow
                };

                _unitOfWork.Users.Add(user);
                _unitOfWork.SaveChanges();

                // Assign default "Customer" role
                var customerRole = _unitOfWork.Query<Role>().FirstOrDefault(r => r.Name == "Customer");
                if (customerRole != null)
                {
                    _unitOfWork.Query<UserRole>().Add(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = customerRole.Id
                    });
                    _unitOfWork.SaveChanges();
                }

                // Send verification email
                try
                {
                    var verificationLink = string.Format(
                        "{0}/Account/VerifyEmail?token={1}",
                        HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority),
                        HttpUtility.UrlEncode(user.EmailVerificationToken)
                    );

                    var emailBody = $"<h2>Welcome to BusTravel!</h2><p>Please verify your email by clicking <a href='{verificationLink}'>here</a>.</p>";
                    var emailService = new NotificationService(_unitOfWork);
                    emailService.SendEmail(user.Email, "Verify your BusTravel account", emailBody);
                }
                catch (Exception ex)
                {
                    var log = log4net.LogManager.GetLogger(typeof(AuthService));
                    log.Warn("Failed to send verification email", ex);
                }

                result.Success = true;
                result.UserId = user.Id;
                result.Message = "Registration successful! Please check your email to verify your account.";
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(AuthService));
                log.Error("Registration error", ex);
                result.Errors.Add("An error occurred during registration. Please try again.");
            }

            return result;
        }

        public LogoutResult Logout()
        {
            var authManager = HttpContext.Current.GetOwinContext().Authentication;
            authManager.SignOut(CookieAuthenticationDefaults.AuthenticationType);

            // Clear session
            try { HttpContext.Current.Session.Clear(); HttpContext.Current.Session.Abandon(); } catch { }

            return new LogoutResult { Success = true };
        }

        public ForgotPasswordResult ForgotPassword(ForgotPasswordViewModel model)
        {
            var result = new ForgotPasswordResult { Success = false };

            try
            {
                var user = _unitOfWork.Users.GetByEmail(model.Email.Trim().ToLower());
                if (user == null)
                {
                    // Don't reveal if email exists
                    result.Success = true;
                    result.Message = "If the email is registered, you will receive a password reset link.";
                    return result;
                }

                user.ResetPasswordToken = TokenHelper.GenerateResetPasswordToken();
                user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(24);
                _unitOfWork.SaveChanges();

                var resetLink = string.Format(
                    "{0}/Account/ResetPassword?token={1}",
                    HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority),
                    HttpUtility.UrlEncode(user.ResetPasswordToken)
                );

                var emailBody = $"<h2>Reset Your Password</h2><p>Click <a href='{resetLink}'>here</a> to reset your password. This link expires in 24 hours.</p>";
                var emailService = new NotificationService(_unitOfWork);
                emailService.SendEmail(user.Email, "BusTravel Password Reset", emailBody);

                result.Success = true;
                result.Message = "If the email is registered, you will receive a password reset link.";
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(AuthService));
                log.Error("ForgotPassword error", ex);
                result.Message = "An error occurred. Please try again.";
            }

            return result;
        }

        public ResetPasswordResult ResetPassword(ResetPasswordViewModel model)
        {
            var result = new ResetPasswordResult { Success = false };

            try
            {
                var user = _unitOfWork.Users.GetByResetToken(model.Token);
                if (user == null)
                {
                    result.Message = "Invalid or expired reset token.";
                    return result;
                }

                user.PasswordHash = PasswordHelper.HashPassword(model.Password);
                user.ResetPasswordToken = null;
                user.ResetPasswordTokenExpiry = null;
                user.FailedLoginAttempts = 0;
                user.IsLocked = false;
                _unitOfWork.SaveChanges();

                result.Success = true;
                result.Message = "Password reset successful. Please login with your new password.";
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(AuthService));
                log.Error("ResetPassword error", ex);
                result.Message = "An error occurred. Please try again.";
            }

            return result;
        }

        public VerifyEmailResult VerifyEmail(string token)
        {
            var result = new VerifyEmailResult { Success = false };

            try
            {
                var user = _unitOfWork.Query<User>()
                    .FirstOrDefault(u => u.EmailVerificationToken == token && !u.IsEmailVerified);

                if (user == null)
                {
                    result.Message = "Invalid or expired verification token.";
                    return result;
                }

                user.IsEmailVerified = true;
                user.EmailVerificationToken = null;
                _unitOfWork.SaveChanges();

                result.Success = true;
                result.Message = "Email verified successfully! You can now login.";
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(AuthService));
                log.Error("VerifyEmail error", ex);
                result.Message = "An error occurred. Please try again.";
            }

            return result;
        }

        public UserProfileViewModel GetProfile(int userId)
        {
            var user = _unitOfWork.Users.GetById(userId);
            if (user == null) return null;

            return new UserProfileViewModel
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                ProfileImageUrl = user.ProfileImageUrl,
                IsEmailVerified = user.IsEmailVerified,
                LastLoginAt = user.LastLoginAt,
                MemberSince = user.CreatedAt
            };
        }

        public UpdateProfileResult UpdateProfile(int userId, UserProfileViewModel model)
        {
            var result = new UpdateProfileResult { Success = false };

            try
            {
                var user = _unitOfWork.Users.GetById(userId);
                if (user == null)
                {
                    result.Message = "User not found.";
                    return result;
                }

                user.FirstName = model.FirstName.Trim();
                user.LastName = model.LastName.Trim();
                user.PhoneNumber = model.PhoneNumber?.Trim();
                user.DateOfBirth = model.DateOfBirth;
                user.Gender = model.Gender;
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.SaveChanges();
                result.Success = true;
                result.Message = "Profile updated successfully.";
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(AuthService));
                log.Error("UpdateProfile error", ex);
                result.Message = "An error occurred while updating profile.";
            }

            return result;
        }

        public ChangePasswordResult ChangePassword(int userId, ChangePasswordViewModel model)
        {
            var result = new ChangePasswordResult { Success = false };

            try
            {
                var user = _unitOfWork.Users.GetById(userId);
                if (user == null)
                {
                    result.Message = "User not found.";
                    return result;
                }

                if (!PasswordHelper.VerifyPassword(model.CurrentPassword, user.PasswordHash))
                {
                    result.Message = "Current password is incorrect.";
                    return result;
                }

                user.PasswordHash = PasswordHelper.HashPassword(model.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.SaveChanges();

                result.Success = true;
                result.Message = "Password changed successfully.";
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(AuthService));
                log.Error("ChangePassword error", ex);
                result.Message = "An error occurred while changing password.";
            }

            return result;
        }

        public int? GetCurrentUserId()
        {
            if (HttpContext.Current?.User?.Identity?.IsAuthenticated == true)
            {
                var user = _unitOfWork.Users.GetByEmail(HttpContext.Current.User.Identity.Name);
                return user?.Id;
            }
            return null;
        }

        public string GetCurrentUserEmail()
        {
            return HttpContext.Current?.User?.Identity?.Name;
        }

        public bool IsUserInRole(int userId, string roleName)
        {
            return _unitOfWork.Query<UserRole>()
                .Any(ur => ur.UserId == userId && ur.Role.Name == roleName);
        }

        public bool IsEmailVerified(int userId)
        {
            var user = _unitOfWork.Users.GetById(userId);
            return user?.IsEmailVerified ?? false;
        }
    }
}
