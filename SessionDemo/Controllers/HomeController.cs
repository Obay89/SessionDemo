using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Authorization;

namespace SessionDemo.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Store session ID for debugging
            ViewBag.Username = User.Identity.Name; // Get logged-in user
            /*
            if (HttpContext.Session.GetString("SessionID") == null)
            {
                HttpContext.Session.SetString("SessionID", Guid.NewGuid().ToString());
            }

            ViewBag.SessionID = HttpContext.Session.GetString("SessionID");*/
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear session manually
            return RedirectToAction("Index");
        }
    }
}
