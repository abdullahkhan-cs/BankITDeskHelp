using System.ComponentModel.DataAnnotations;

namespace BankITDeskHelp.Models
{
    public class Branch
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Location { get; set; }

        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    }
}