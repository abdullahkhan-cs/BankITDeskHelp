using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BankITDeskHelp.Data;
using BankITDeskHelp.ViewModels;

namespace BankITDeskHelp.Services
{
    public class TicketMetricsService : ITicketMetricsService
    {
        private readonly ApplicationDbContext _db;

        public TicketMetricsService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<TicketMetricsDto> GetMetricsAsync()
        {
            var dto = new TicketMetricsDto();

            dto.NewCount = await _db.Complaints.CountAsync(c => c.Status == Models.ComplaintStatus.New);
            dto.AssignedCount = await _db.Complaints.CountAsync(c => c.Status == Models.ComplaintStatus.Assigned);
            dto.InProgressCount = await _db.Complaints.CountAsync(c => c.Status == Models.ComplaintStatus.InProgress);
            dto.ResolvedCount = await _db.Complaints.CountAsync(c => c.Status == Models.ComplaintStatus.Resolved);
            dto.ClosedCount = await _db.Complaints.CountAsync(c => c.Status == Models.ComplaintStatus.Closed);

            dto.TotalUsers = await _db.Users.CountAsync();

            // Avg resolution time for resolved tickets
            var resolvedTimes = await _db.Complaints
                .Where(c => c.Status == Models.ComplaintStatus.Resolved && c.ResolvedAt != null)
                .Select(c => EF.Functions.DateDiffMinute(c.CreatedAt, c.ResolvedAt!.Value))
                .ToListAsync();

            if (resolvedTimes.Any())
            {
                dto.AvgResolutionMinutes = resolvedTimes.Average();
            }

            // Department breakdown
            var dept = await _db.Complaints
                .Include(c => c.Department)
                .Where(c => c.Department != null)
                .GroupBy(c => c.Department!.Name)
                .Select(g => new DepartmentMetric
                {
                    DepartmentName = g.Key ?? "-",
                    Count = g.Count()
                })
                .OrderByDescending(d => d.Count)
                .ToListAsync();

            dto.DepartmentMetrics = dept;

            dto.StatusCounts = new[] { dto.NewCount, dto.AssignedCount, dto.InProgressCount, dto.ResolvedCount, dto.ClosedCount };

            return dto;
        }
    }
}
