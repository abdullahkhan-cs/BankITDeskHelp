using BankITDeskHelp.Models;

namespace BankITDeskHelp.ViewModels
{
    public class AdminVerifyViewModel
    {
        public int ComplaintId { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string? AssignedManagerName { get; set; }
        public PriorityLevel Priority { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ManagerRemarks { get; set; }
        public ComplaintStatus Status { get; set; }
        public List<Comment> Comments { get; set; } = new();
        public List<ComplaintHistory> Histories { get; set; } = new();
        public List<Attachment> Attachments { get; set; } = new();
    }
}