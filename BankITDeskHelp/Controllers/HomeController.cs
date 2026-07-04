using BankITDeskHelp.Models;
using BankITDeskHelp.Data;
using BankITDeskHelp.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BankITDeskHelp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // If the user is authenticated, send them to their role dashboard.
            if (User?.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole(Roles.Admin))
                    return RedirectToAction("Index", "Admin");
                if (User.IsInRole(Roles.Manager))
                    return RedirectToAction("Index", "Manager");

                // Authenticated non-admin users can go to the Complaint creation page
                return RedirectToAction("Create", "Complaint");
            }

            // Anonymous users should see the public landing page (Home/Index)
            // Load departments, branches, and categories for the form
            ViewBag.Departments = await _context.Departments.OrderBy(d => d.Name).ToListAsync();
            ViewBag.Branches = await _context.Branches.OrderBy(b => b.Name).ToListAsync();
            ViewBag.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();

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
