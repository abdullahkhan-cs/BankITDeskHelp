using System.ComponentModel.DataAnnotations;
using BankITDeskHelp.Models;

namespace BankITDeskHelp.ViewModels
{
    public class ComplaintCreateViewModel
    {
        [Required(ErrorMessage = "Employee name is required")]
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Employee ID is required")]
        [Display(Name = "Employee ID")]
        public string EmployeeId { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? Phone { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required]
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

        [Required, MaxLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public IFormFile? Attachment { get; set; }

        // For populating dropdowns in the view
        public List<Branch>? Branches { get; set; }
        public List<Department>? Departments { get; set; }
        public List<Category>? Categories { get; set; }
    }
}