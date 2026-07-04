using BankITDeskHelp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;

namespace BankITDeskHelp.Models
{
    public class Complaint
    {
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string TicketNumber { get; set; } = string.Empty; // e.g. IT-2026-00001

        // Employee info (captured at submission time, not necessarily a registered user)
        [Required, MaxLength(100)]
        public string EmployeeName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string EmployeeId { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        public int BranchId { get; set; }
        public Branch? Branch { get; set; }

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

        [Required, MaxLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required, MaxLength(5000)]
        public string Description { get; set; } = string.Empty;

        public ComplaintStatus Status { get; set; } = ComplaintStatus.New;

        // Assigned Manager (registered ApplicationUser with Manager role)
        public string? AssignedManagerId { get; set; }
        public ApplicationUser? AssignedManager { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? AssignedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        [MaxLength(1000)]
        public string? ManagerRemarks { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public ICollection<ComplaintHistory> Histories { get; set; } = new List<ComplaintHistory>();
    }
}