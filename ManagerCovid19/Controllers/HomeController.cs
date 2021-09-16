using ManagerCovid19.Data;
using ManagerCovid19.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ManagerCovid19.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        ManagerCovid19Context _context;

        public HomeController(ILogger<HomeController> logger, ManagerCovid19Context context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("login")]
        public IActionResult Login(string ReturnURL)
        {
            ViewData["ReturnURL"] = ReturnURL;
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Validate(string RN, string password, string ReturnURL)
        {
            ViewData["ReturnURL"] = ReturnURL;
            var user = _context.Member.Find(RN);
            if (user != null)
            {
                if (user.Password == password)
                {
                    var claims = new List<Claim>();
                    claims.Add(new Claim("RN", RN));
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Name));
                    claims.Add(new Claim(ClaimTypes.Role, user.Admin? "Admin":""));
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    return Redirect(ReturnURL);
                }
            }
            TempData["error"] = "Error: Invalid register number or password";
            return View("Login");
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        [Route("denied")]
        public IActionResult Denied()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
