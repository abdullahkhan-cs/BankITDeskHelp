using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankITDeskHelp.Data;
using BankITDeskHelp.Models;
using Microsoft.AspNetCore.Identity;
using BankITDeskHelp.ViewModels;

namespace BankITDeskHelp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly BankITDeskHelp.Services.ITicketMetricsService _metricsService;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, BankITDeskHelp.Services.ITicketMetricsService metricsService)
        {
            _context = context;
            _userManager = userManager;
            _metricsService = metricsService;
        }

        // GET: /Admin/Index
        public async Task<IActionResult> Index()
        {
            ViewBag.NewCount = await _context.Complaints.CountAsync(c => c.Status == ComplaintStatus.New);
            ViewBag.AssignedCount = await _context.Complaints.CountAsync(c => c.Status == ComplaintStatus.Assigned);
            ViewBag.InProgressCount = await _context.Complaints.CountAsync(c => c.Status == ComplaintStatus.InProgress);
            ViewBag.ResolvedCount = await _context.Complaints.CountAsync(c => c.Status == ComplaintStatus.Resolved);
            ViewBag.ClosedCount = await _context.Complaints.CountAsync(c => c.Status == ComplaintStatus.Closed);

            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalDepartments = await _context.Departments.CountAsync();

            var recentComplaints = await _context.Complaints
                .Include(c => c.Department)
                .Include(c => c.Branch)
                .OrderByDescending(c => c.CreatedAt)
                .Take(10)
                .ToListAsync();

            return View(recentComplaints);
        }

        [HttpGet]
        public async Task<IActionResult> DashboardData()
        {
            var metrics = await _metricsService.GetMetricsAsync();
            return Json(metrics);
        }

        // GET: /Admin/Assign/5
        public async Task<IActionResult> Assign(int id)
        {
            var complaint = await _context.Complaints
                .Include(c => c.Department)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (complaint == null)
                return NotFound();

            var managers = await _userManager.GetUsersInRoleAsync("Manager");

            var vm = new AssignComplaintViewModel
            {
                ComplaintId = complaint.Id,
                TicketNumber = complaint.TicketNumber,
                EmployeeName = complaint.EmployeeName,
                DepartmentName = complaint.Department?.Name ?? "",
                Priority = complaint.Priority,
                Description = complaint.Description,
                Managers = managers.ToList()
            };

            return View(vm);
        }

        // POST: /Admin/Assign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(AssignComplaintViewModel vm)
        {
            var complaint = await _context.Complaints.FindAsync(vm.ComplaintId);
            if (complaint == null)
                return NotFound();

            if (string.IsNullOrEmpty(vm.SelectedManagerId))
            {
                ModelState.AddModelError("SelectedManagerId", "Please select a manager.");
                vm.Managers = (await _userManager.GetUsersInRoleAsync("Manager")).ToList();
                return View(vm);
            }

            complaint.AssignedManagerId = vm.SelectedManagerId;
            complaint.Status = ComplaintStatus.Assigned;
            complaint.AssignedAt = DateTime.Now;

            var manager = await _userManager.FindByIdAsync(vm.SelectedManagerId);

            _context.ComplaintHistories.Add(new ComplaintHistory
            {
                ComplaintId = complaint.Id,
                Action = "Assigned to Manager",
                Details = $"Assigned to {manager?.FullName}. Remarks: {vm.Remarks}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: /Admin/Verify/5
        public async Task<IActionResult> Verify(int id)
        {
            var complaint = await _context.Complaints
                .Include(c => c.Department)
                .Include(c => c.AssignedManager)
                .Include(c => c.Comments)
                .Include(c => c.Histories)
                .Include(c => c.Attachments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (complaint == null) return NotFound();

            var vm = new AdminVerifyViewModel
            {
                ComplaintId = complaint.Id,
                TicketNumber = complaint.TicketNumber,
                EmployeeName = complaint.EmployeeName,
                DepartmentName = complaint.Department?.Name ?? "",
                AssignedManagerName = complaint.AssignedManager?.FullName,
                Priority = complaint.Priority,
                Subject = complaint.Subject,
                Description = complaint.Description,
                ManagerRemarks = complaint.ManagerRemarks,
                Status = complaint.Status,
                Comments = complaint.Comments.OrderBy(c => c.CreatedAt).ToList(),
                Histories = complaint.Histories.OrderBy(h => h.Timestamp).ToList(),
                Attachments = complaint.Attachments.ToList()
            };

            return View(vm);
        }

        // POST: /Admin/CloseTicket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseTicket(int id)
        {
            var complaint = await _context.Complaints.FindAsync(id);
            if (complaint == null) return NotFound();

            complaint.Status = ComplaintStatus.Closed;
            complaint.ClosedAt = DateTime.Now;

            _context.ComplaintHistories.Add(new ComplaintHistory
            {
                ComplaintId = complaint.Id,
                Action = "Admin Closed",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: /Admin/Reassign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reassign(int id)
        {
            var complaint = await _context.Complaints.FindAsync(id);
            if (complaint == null) return NotFound();

            complaint.Status = ComplaintStatus.Assigned;

            _context.ComplaintHistories.Add(new ComplaintHistory
            {
                ComplaintId = complaint.Id,
                Action = "Reassigned by Admin",
                Details = "Sent back to Manager for further work",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: /Admin/EnsureManagers - creates demo managers if missing
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnsureManagers()
        {
            await BankITDeskHelp.Data.SeedData.SeedManagerUserAsync(_userManager);
            TempData["Message"] = "Ensured demo managers exist.";
            return RedirectToAction("Index");
        }
    }
}
