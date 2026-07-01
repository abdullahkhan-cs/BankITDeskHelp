using BankITDeskHelp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BankITDeskHelp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // If the user is authenticated, send them to their role dashboard.
            if (User?.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Index", "Admin");
                if (User.IsInRole("Manager"))
                    return RedirectToAction("Index", "Manager");

                // Authenticated non-admin users can go to the Complaint creation page
                return RedirectToAction("Create", "Complaint");
            }

            // Anonymous users should see the public landing page (Home/Index)
            return View();
        }

        public IActionResult Privacy()
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
