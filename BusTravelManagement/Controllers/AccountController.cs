using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using BusTravelManagement.Filters;
using BusTravelManagement.Models.ViewModels;
using BusTravelManagement.Services.Interfaces;

namespace BusTravelManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(AccountController));

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        [RedirectIfAuthenticated]
        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RedirectIfAuthenticated]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = _authService.Login(model);
            try
            {
                _logger.Info($"Login attempt: email={model.Email} success={result.Success} message={result.Message}");
            }
            catch { }
            if (result.Success)
            {
                return RedirectToAction("Index", "Home");
            }

            if (result.EmailNotVerified)
            {
                ViewBag.EmailNotVerified = true;
            }

            ViewBag.ErrorMessage = result.Message;
            return View(model);
        }

        // Diagnostic endpoint to inspect server-side cookie and identity state
        [HttpGet]
        [AllowAnonymous]
        public ActionResult DebugAuth()
        {
            var cookies = new List<object>();
            var keys = Request.Cookies?.AllKeys ?? new string[0];

            foreach (var k in keys)
            {
                var c = Request.Cookies[k];
                cookies.Add(new
                {
                    Name = k,
                    Value = c?.Value,
                    Domain = c?.Domain,
                    Path = c?.Path,
                    HttpOnly = c?.HttpOnly,
                    Secure = c?.Secure,
                    Expires = c?.Expires.ToString("o")
                });
            }

            var result = new
            {
                RequestUrl = Request.Url?.ToString(),
                UserIdentity = new
                {
                    IsAuthenticated = HttpContext.User?.Identity?.IsAuthenticated ?? false,
                    Name = HttpContext.User?.Identity?.Name
                },
                Cookies = cookies
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [RedirectIfAuthenticated]
        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RedirectIfAuthenticated]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = _authService.Register(model);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Login");
            }

            ViewBag.ErrorMessage = result.Message;
            ViewBag.Errors = result.Errors;
            return View(model);
        }

        [HttpPost]
        public JsonResult CheckEmailAvailability(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Json(true);

            var unitOfWork = DependencyResolver.Current.GetService<Repositories.Interfaces.IUnitOfWork>();
            var exists = unitOfWork.Users.IsEmailRegistered(email.Trim().ToLower());
            return Json(!exists);
        }

        [HttpGet]
        public ActionResult VerifyEmail(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login");

            var result = _authService.VerifyEmail(token);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        [RedirectIfAuthenticated]
        public ActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RedirectIfAuthenticated]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = _authService.ForgotPassword(model);
            TempData["SuccessMessage"] = result.Message;

            return RedirectToAction("Login");
        }

        [HttpGet]
        [RedirectIfAuthenticated]
        public ActionResult ResetPassword(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login");

            return View(new ResetPasswordViewModel { Token = token });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RedirectIfAuthenticated]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = _authService.ResetPassword(model);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Login");
            }

            ViewBag.ErrorMessage = result.Message;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            _authService.Logout();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public new ActionResult Profile()
        {
            var userId = _authService.GetCurrentUserId();
            if (!userId.HasValue)
                return RedirectToAction("Login");

            var profile = _authService.GetProfile(userId.Value);
            if (profile == null)
                return RedirectToAction("Login");

            return View(profile);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public new ActionResult Profile(UserProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = _authService.GetCurrentUserId();
            if (!userId.HasValue)
                return RedirectToAction("Login");

            var result = _authService.UpdateProfile(userId.Value, model);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Profile");
            }

            ViewBag.ErrorMessage = result.Message;
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = _authService.GetCurrentUserId();
            if (!userId.HasValue)
                return RedirectToAction("Login");

            var result = _authService.ChangePassword(userId.Value, model);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Profile");
            }

            ViewBag.ErrorMessage = result.Message;
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public ActionResult MyBookings(int page = 1, int pageSize = 10)
        {
            var userId = _authService.GetCurrentUserId();
            if (!userId.HasValue)
                return RedirectToAction("Login");

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            ViewBag.PageSize = pageSize;

            var bookingService = DependencyResolver.Current.GetService<IBookingService>();
            var bookings = bookingService.GetUserBookings(userId.Value, page, pageSize);
            return View(bookings);
        }

        [HttpGet]
        [Authorize]
        public ActionResult SavedPassengers()
        {
            var userId = _authService.GetCurrentUserId();
            if (!userId.HasValue)
                return RedirectToAction("Login");

            var unitOfWork = DependencyResolver.Current.GetService<Repositories.Interfaces.IUnitOfWork>();
            var passengers = unitOfWork.Query<Models.Entities.SavedPassenger>()
                .Where(p => p.UserId == userId.Value && p.IsActive)
                .ToList();

            return View(passengers);
        }

        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}
