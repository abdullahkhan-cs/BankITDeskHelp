using BankITDeskHelp.Models;

namespace BankITDeskHelp.ViewModels
{
    public class AssignComplaintViewModel
    {
        public int ComplaintId { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public PriorityLevel Priority { get; set; }
        public string Description { get; set; } = string.Empty;

        public string? SelectedManagerId { get; set; }
        public string? Remarks { get; set; }

        public List<ApplicationUser>? Managers { get; set; }
    }
}