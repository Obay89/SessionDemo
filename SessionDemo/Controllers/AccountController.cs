using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SessionDemo.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (username == "admin" && password == "password")
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid username or password";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult ExtendSession()
        {
            if (User.Identity is ClaimsIdentity identity)
            {
                var claims = identity.Claims.ToList(); // ✅ Extract claims properly

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, // ✅ Keeps session active
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(2) // ✅ Extend session for 2 more minutes
                };

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                return Ok(new { message = "Session extended" });
            }

            return Unauthorized(); // If no user is logged in, return unauthorized response
        }
    }
}
