using System.Collections.Generic;

namespace BankITDeskHelp.ViewModels
{
    public class DepartmentMetric
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class TicketMetricsDto
    {
        public int NewCount { get; set; }
        public int AssignedCount { get; set; }
        public int InProgressCount { get; set; }
        public int ResolvedCount { get; set; }
        public int ClosedCount { get; set; }

        public int TotalUsers { get; set; }

        // Avg resolution time in minutes for resolved tickets
        public double AvgResolutionMinutes { get; set; }

        // Department breakdown
        public List<DepartmentMetric> DepartmentMetrics { get; set; } = new List<DepartmentMetric>();

        // For chart convenience
        public string[] StatusLabels { get; set; } = new[] { "New", "Assigned", "In Progress", "Resolved", "Closed" };
        public int[] StatusCounts { get; set; } = new int[5];
    }
}
