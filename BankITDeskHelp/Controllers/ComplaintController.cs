using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BankITDeskHelp.Data;
using BankITDeskHelp.Models;
using BankITDeskHelp.ViewModels;

namespace BankITDeskHelp.Controllers
{
    public class ComplaintController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public ComplaintController(ApplicationDbContext context, IWebHostEnvironment env, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        // GET: /Complaint/Create
        public async Task<IActionResult> Create()
        {
            var vm = new ComplaintCreateViewModel
            {
                Branches = await _context.Branches.OrderBy(b => b.Name).ToListAsync(),
                Departments = await _context.Departments.OrderBy(d => d.Name).ToListAsync(),
                Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync()
            };
            return View(vm);
        }

        // POST: /Complaint/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComplaintCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Branches = await _context.Branches.OrderBy(b => b.Name).ToListAsync();
                vm.Departments = await _context.Departments.OrderBy(d => d.Name).ToListAsync();
                vm.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
                return View(vm);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var complaint = new Complaint
                {
                    TicketNumber = await GenerateTicketNumberAsync(),
                    EmployeeName = vm.EmployeeName,
                    EmployeeId = vm.EmployeeId,
                    Email = vm.Email,
                    Phone = vm.Phone,
                    BranchId = vm.BranchId,
                    DepartmentId = vm.DepartmentId,
                    CategoryId = vm.CategoryId,
                    Priority = vm.Priority,
                    Subject = vm.Subject,
                    Description = vm.Description,
                    Status = ComplaintStatus.New,
                    CreatedAt = DateTime.Now
                };

                _context.Complaints.Add(complaint);
                await _context.SaveChangesAsync();

                // Automatic assignment: pick a manager with the fewest open tickets in the same department
                var managers = await _userManager.GetUsersInRoleAsync("Manager");
                ApplicationUser? selectedManager = null;
                if (managers != null && managers.Count > 0)
                {
                    // Filter managers by department
                    var departmentManagers = managers.Where(m => m.DepartmentId == complaint.DepartmentId).ToList();

                    // If no department-specific managers, fall back to all managers
                    var managerPool = departmentManagers.Any() ? departmentManagers : managers.ToList();

                    // Use single query to get workloads (fix N+1 problem)
                    var managerWorkloads = await _context.Complaints
                        .Where(c => c.Status != ComplaintStatus.Closed && managerPool.Select(m => m.Id).Contains(c.AssignedManagerId))
                        .GroupBy(c => c.AssignedManagerId)
                        .Select(g => new { ManagerId = g.Key, Count = g.Count() })
                        .ToListAsync();

                    selectedManager = managerPool
                        .OrderBy(m => managerWorkloads.FirstOrDefault(w => w.ManagerId == m.Id)?.Count ?? 0)
                        .FirstOrDefault();
                }

                if (selectedManager != null)
                {
                    complaint.AssignedManagerId = selectedManager.Id;
                    complaint.Status = ComplaintStatus.Assigned;
                    complaint.AssignedAt = DateTime.Now;

                    _context.ComplaintHistories.Add(new ComplaintHistory
                    {
                        ComplaintId = complaint.Id,
                        Action = "Assigned to Manager",
                        Details = $"Automatically assigned to {selectedManager.FullName}",
                        Timestamp = DateTime.Now
                    });
                }

                // Handle file attachment with security validation
                if (vm.Attachment != null && vm.Attachment.Length > 0)
                {
                    // Validate file size (5MB limit)
                    if (vm.Attachment.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("Attachment", "File size exceeds 5MB limit.");
                        vm.Branches = await _context.Branches.OrderBy(b => b.Name).ToListAsync();
                        vm.Departments = await _context.Departments.OrderBy(d => d.Name).ToListAsync();
                        vm.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
                        return View(vm);
                    }

                    // Validate file type
                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".png", ".jpg", ".jpeg" };
                    var extension = Path.GetExtension(vm.Attachment.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("Attachment", "Invalid file type. Allowed types: PDF, DOC, DOCX, XLS, XLSX, PNG, JPG, JPEG.");
                        vm.Branches = await _context.Branches.OrderBy(b => b.Name).ToListAsync();
                        vm.Departments = await _context.Departments.OrderBy(d => d.Name).ToListAsync();
                        vm.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
                        return View(vm);
                    }

                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{vm.Attachment.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await vm.Attachment.CopyToAsync(stream);
                    }

                    _context.Attachments.Add(new Attachment
                    {
                        ComplaintId = complaint.Id,
                        FileName = vm.Attachment.FileName,
                        FilePath = $"/uploads/{uniqueFileName}"
                    });
                }

                // Record creation history if not already recorded by assignment
                if (!(_context.ComplaintHistories.Any(h => h.ComplaintId == complaint.Id && h.Action == "Complaint Created")))
                {
                    _context.ComplaintHistories.Add(new ComplaintHistory
                    {
                        ComplaintId = complaint.Id,
                        Action = "Complaint Created",
                        Details = $"Submitted by {complaint.EmployeeName}",
                        Timestamp = DateTime.Now
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction("Confirmation", new { ticketNumber = complaint.TicketNumber });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // GET: /Complaint/Confirmation?ticketNumber=IT-2026-00001
        public IActionResult Confirmation(string ticketNumber)
        {
            ViewBag.TicketNumber = ticketNumber;
            return View();
        }

        private async Task<string> GenerateTicketNumberAsync()
        {
            int year = DateTime.Now.Year;
            string prefix = $"IT-{year}-";

            var lastTicket = await _context.Complaints
                .Where(c => c.TicketNumber.StartsWith(prefix))
                .OrderByDescending(c => c.Id)
                .Select(c => c.TicketNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastTicket != null)
            {
                var numberPart = lastTicket.Substring(prefix.Length);
                if (int.TryParse(numberPart, out int parsed))
                {
                    nextNumber = parsed + 1;
                }
            }

            return $"{prefix}{nextNumber:D5}"; // IT-2026-00001
        }
    }
}