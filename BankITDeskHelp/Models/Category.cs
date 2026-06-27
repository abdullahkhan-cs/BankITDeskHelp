using System.ComponentModel.DataAnnotations;

namespace BankITDeskHelp.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty; // e.g. ATM Support, Core Banking, Internet Banking

        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    }
}