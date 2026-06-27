using Microsoft.AspNetCore.Identity;

namespace BankITDeskHelp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? BranchName { get; set; }
        public string? DepartmentName { get; set; }
    }
}