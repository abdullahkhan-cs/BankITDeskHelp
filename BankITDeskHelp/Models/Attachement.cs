using System.ComponentModel.DataAnnotations;

namespace BankITDeskHelp.Models
{
    public class Attachment
    {
        public int Id { get; set; }

        public int ComplaintId { get; set; }
        public Complaint? Complaint { get; set; }

        [Required, MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.Now;
    }
}