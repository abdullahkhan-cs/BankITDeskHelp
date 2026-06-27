using System.ComponentModel.DataAnnotations;

namespace BankITDeskHelp.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    }
}