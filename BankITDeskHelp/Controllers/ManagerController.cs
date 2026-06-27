using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankITDeskHelp.Data;
using BankITDeskHelp.Models;
using BankITDeskHelp.ViewModels;

namespace BankITDeskHelp.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ManagerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Manager/Index
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            var myTickets = await _context.Complaints
                .Include(c => c.Department)
                .Include(c => c.Branch)
                .Where(c => c.AssignedManagerId == currentUser.Id)
                .OrderByDescending(c => c.AssignedAt)
                .ToListAsync();

            ViewBag.AssignedCount = myTickets.Count(c => c.Status == ComplaintStatus.Assigned);
            ViewBag.WorkingCount = myTickets.Count(c => c.Status == ComplaintStatus.InProgress);
            ViewBag.ResolvedTodayCount = myTickets.Count(c => c.Status == ComplaintStatus.Resolved && c.ResolvedAt.HasValue && c.ResolvedAt.Value.Date == DateTime.Today);
            ViewBag.PendingCount = myTickets.Count(c => c.Status != ComplaintStatus.Closed);

            return View(myTickets);
        }

        // GET: /Manager/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var complaint = await _context.Complaints
                .Include(c => c.Department)
                .Include(c => c.Branch)
                .Include(c => c.Attachments)
                .Include(c => c.Comments)
                .Include(c => c.Histories)
                .FirstOrDefaultAsync(c => c.Id == id && c.AssignedManagerId == currentUser!.Id);

            if (complaint == null) return NotFound();

            var vm = new ManagerTicketDetailsViewModel
            {
                ComplaintId = complaint.Id,
                TicketNumber = complaint.TicketNumber,
                EmployeeName = complaint.EmployeeName,
                DepartmentName = complaint.Department?.Name ?? "",
                BranchName = complaint.Branch?.Name ?? "",
                Priority = complaint.Priority,
                Subject = complaint.Subject,
                Description = complaint.Description,
                Status = complaint.Status,
                Attachments = complaint.Attachments.ToList(),
                Comments = complaint.Comments.OrderBy(c => c.CreatedAt).ToList(),
                Histories = complaint.Histories.OrderBy(h => h.Timestamp).ToList()
            };

            return View(vm);
        }

        // POST: /Manager/StartWork/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartWork(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var complaint = await _context.Complaints
                .FirstOrDefaultAsync(c => c.Id == id && c.AssignedManagerId == currentUser!.Id);

            if (complaint == null) return NotFound();

            complaint.Status = ComplaintStatus.InProgress;
            complaint.StartedAt = DateTime.Now;

            _context.ComplaintHistories.Add(new ComplaintHistory
            {
                ComplaintId = complaint.Id,
                Action = "Manager Started Work",
                Timestamp = DateTime.Now,
                PerformedByUserId = currentUser!.Id
            });

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id });
        }

        // POST: /Manager/AddComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int complaintId, string newComment)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (!string.IsNullOrWhiteSpace(newComment))
            {
                _context.Comments.Add(new Comment
                {
                    ComplaintId = complaintId,
                    Text = newComment,
                    AuthorUserId = currentUser!.Id,
                    CreatedAt = DateTime.Now
                });

                _context.ComplaintHistories.Add(new ComplaintHistory
                {
                    ComplaintId = complaintId,
                    Action = "Manager Added Comment",
                    Timestamp = DateTime.Now,
                    PerformedByUserId = currentUser!.Id
                });

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = complaintId });
        }

        // POST: /Manager/Resolve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resolve(int complaintId, string? managerRemarks)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var complaint = await _context.Complaints
                .FirstOrDefaultAsync(c => c.Id == complaintId && c.AssignedManagerId == currentUser!.Id);

            if (complaint == null) return NotFound();

            complaint.Status = ComplaintStatus.Resolved;
            complaint.ResolvedAt = DateTime.Now;
            complaint.ManagerRemarks = managerRemarks;

            _context.ComplaintHistories.Add(new ComplaintHistory
            {
                ComplaintId = complaint.Id,
                Action = "Manager Resolved",
                Details = managerRemarks,
                Timestamp = DateTime.Now,
                PerformedByUserId = currentUser!.Id
            });

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}