using EdsanBooking.Data;
using EdsanBooking.Models;
using EdsanBooking.Configuration;
using Microsoft.Extensions.Configuration;
using EdsanBooking.Services;
using EdsanBooking.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using EdsanBooking.Interface;

namespace EdsanBooking.Controllers
{
    public class AccountController : Controller
    {
        private readonly BookingContext _context;
        private readonly IGuestService _guestService;

        public AccountController(BookingContext context, IGuestService guestService)
        {
            _context = context;
            _guestService = guestService;
        }

        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Signup(SignupDto signupDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _guestService.SignupAsync(signupDto);
                    //return RedirectToAction("Login", "Account");

                    HttpContext.Session.SetString("FirstName", signupDto.FirstName);
                    HttpContext.Session.SetString("LastName", signupDto.LastName);

                    // Replace with your authentication logic
                    bool isAuthenticated = true;

                    if (isAuthenticated)
                    {
                        // Set session variable
                        HttpContext.Session.SetString("UserLoggedIn", "true");
                        HttpContext.Session.SetString("UserRole", "Guest");
                        HttpContext.Session.SetString("Location", signupDto.Location);
                    }

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(signupDto);
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users
                    .FirstOrDefault(u => u.UserName == model.Username && u.Password == model.Password && u.Location == model.Location);

                if (user != null && !user.IsLocked && user.Status == "Active")
                {
                    // Reset failed login attempts
                    user.FailedLoginAttempts = 0;
                    _context.SaveChanges();

                    // Store the username and location in session
                    HttpContext.Session.SetString("Username", user.UserName);
                    HttpContext.Session.SetString("Location", user.Location);
                    HttpContext.Session.SetString("UserRole", user.UserRole);

                    // Replace with your authentication logic
                    bool isAuthenticated = true;

                    if(isAuthenticated)
                    {
                        // Set session variable
                        HttpContext.Session.SetString("UserLoggedIn", "true");
                    }

                    // Calculate the first day of the current month and today's date
                    DateTime fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    DateTime toDate = DateTime.Now;

                    if(user.UserRole == "Admin" || user.UserRole == "User")
                    {
                        // Redirect to the Summary action with the required parameters
                        return RedirectToAction("Summary", "Dashboard", new { fromDate = fromDate, toDate = toDate, location = user.Location });
                        //return RedirectToAction("Index", "Home");
                    }
                    else if (user.UserRole == "Guest")
                    {
                        // Redirect to the Index action of the Home controller
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        // Handle other user roles or errors here if necessary
                        return RedirectToAction("Login", "Account"); // Example: Redirect back to login if user role is not recognized
                    }

                }
                else if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid username, password, or location.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Your account is locked or inactive.");
                }
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult Logout()
        {
            // Clear the user session or cookie
            HttpContext.Session.Clear(); // If using sessions

            // Alternatively, sign out the user if using authentication cookies
            // await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirect to login page
            //return RedirectToAction("Login", "Account");
            return RedirectToAction("Index", "Home");
        }
    }

}
