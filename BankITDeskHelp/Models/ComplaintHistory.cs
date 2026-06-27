using BankITDeskHelp.Models;
using System.ComponentModel.DataAnnotations;

namespace BankITDeskHelp.Models
{
    public class ComplaintHistory
    {
        public int Id { get; set; }

        public int ComplaintId { get; set; }
        public Complaint? Complaint { get; set; }

        [Required, MaxLength(200)]
        public string Action { get; set; } = string.Empty; // e.g. "Complaint Created", "Assigned to ATM Manager"

        [MaxLength(500)]
        public string? Details { get; set; }

        public string? PerformedByUserId { get; set; }
        public ApplicationUser? PerformedByUser { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}