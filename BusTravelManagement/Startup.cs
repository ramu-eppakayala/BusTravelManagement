using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(BusTravelManagement.Startup))]

namespace BusTravelManagement
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Configure Auth
            app.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions
            {
                AuthenticationType = Microsoft.Owin.Security.Cookies.CookieAuthenticationDefaults.AuthenticationType,
                // Custom name invalidates cookies issued by earlier auth builds whose
                // identities lacked ClaimTypes.NameIdentifier (the cause of the
                // anti-forgery InvalidOperationException). Forces one clean re-login.
                CookieName = ".BusTravel.Auth",
                LoginPath = new PathString("/Account/Login"),
                LogoutPath = new PathString("/Account/Logout"),
                ExpireTimeSpan = System.TimeSpan.FromMinutes(30),
                SlidingExpiration = true,
                CookieHttpOnly = true,
                // Use Lax for localhost/dev so POST navigation works. SameSite=None requires Secure+HTTPS.
                CookieSameSite = Microsoft.Owin.SameSiteMode.Lax,
                // Keep cookie Secure behavior same as request (works for HTTP localhost)
                CookieSecure = Microsoft.Owin.Security.Cookies.CookieSecureOption.SameAsRequest
            });
        }
    }
}
