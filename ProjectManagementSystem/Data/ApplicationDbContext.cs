using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Models;


namespace ProjectManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected ApplicationDbContext()
        {

        }

        public override DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            builder.Entity<Project>()
                .HasOne(p => p.ProjectManager)
                .WithMany(b => b.Projects)
                .HasForeignKey(p => p.ProjectManagerId);

            builder.Entity<Task>()
                .HasOne(p => p.Assignee)
                .WithMany(b => b.Tasks)
                .HasForeignKey(p => p.AssigneeId);

            builder.Entity<Task>()
                .HasOne(p => p.Project)
                .WithMany(b => b.Tasks)
                .HasForeignKey(p => p.ProjectCode);
        }

    }
}
