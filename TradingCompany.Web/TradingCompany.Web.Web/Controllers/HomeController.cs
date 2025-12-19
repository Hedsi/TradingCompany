using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TradingCompany.Web.Models;

namespace TradingCompany.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // GET: /
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                ViewBag.UserName = User.Identity.Name;
                ViewBag.IsManager = User.IsInRole("Менеджер");
            }

            return View();
        }

        // GET: /Home/Privacy
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        // GET: /Home/About
        [AllowAnonymous]
        public IActionResult About()
        {
            return View();
        }

        // GET: /Home/Contact
        [AllowAnonymous]
        public IActionResult Contact()
        {
            return View();
        }

        // GET: /Home/Error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}