using BankITDeskHelp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BankITDeskHelp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<ComplaintHistory> ComplaintHistories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Attachment> Attachments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Prevent cascade delete cycles (multiple FKs to ApplicationUser/Complaint)
            builder.Entity<Complaint>()
                .HasOne(c => c.AssignedManager)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ComplaintHistory>()
                .HasOne(h => h.PerformedByUser)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Comment>()
                .HasOne(c => c.AuthorUser)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}