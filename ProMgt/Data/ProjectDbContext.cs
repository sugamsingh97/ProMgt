using Microsoft.EntityFrameworkCore;
using ProMgt.Data.Model;
using System.Security.Cryptography.X509Certificates;
using TaskStatus = ProMgt.Data.Model.TaskStatus;

namespace ProMgt.Data
{
    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> opt) : base(opt)
        {

        }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<ProjectStatus> ProjectStatuses { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<TaskStatus> TaskStatuses { get; set; }
        public DbSet<ProjectMgtColor> ProjectMgtColors { get; set; }
        public DbSet<Section> Sections { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Project>()
            .HasMany(p => p.Tasks)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Priorities)
                .WithOne(pr => pr.Project)
                .HasForeignKey(pr => pr.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.TaskStatuses)
                .WithOne(ts => ts.Project)
                .HasForeignKey(ts => ts.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // project with section
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Sections)
                .WithOne(sc => sc.Project)
                .HasForeignKey(sc => sc.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectTask>()
                .HasOne(t => t.Priority)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.PriorityId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ProjectTask>()
                .HasOne(t => t.TaskStatus)
                .WithMany(ts => ts.Tasks)
                .HasForeignKey(t => t.TaskStatusId)
                .OnDelete(DeleteBehavior.SetNull);

            // task with section
            modelBuilder.Entity<ProjectTask>()
                .HasOne(t => t.Section)
                .WithMany(sc => sc.Tasks)
                .HasForeignKey(t=> t.SectionId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Priority>()
                .HasOne(p => p.Color)
                .WithMany(c => c.Priorities)
                .HasForeignKey(p => p.ColorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskStatus>()
                .HasOne(ts => ts.Color)
                .WithMany(c => c.TaskStatuses)
                .HasForeignKey(ts => ts.ColorId)
                .OnDelete(DeleteBehavior.Restrict);
            
        }
    }

}
