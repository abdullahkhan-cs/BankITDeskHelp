using BankITDeskHelp.Models;
using System.ComponentModel.DataAnnotations;

namespace BankITDeskHelp.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public int ComplaintId { get; set; }
        public Complaint? Complaint { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        public bool IsInternalNote { get; set; } = false; // internal notes vs visible comments

        public string? AuthorUserId { get; set; }
        public ApplicationUser? AuthorUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}