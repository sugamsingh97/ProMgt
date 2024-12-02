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

        public DbSet<ProjectAssignment> ProjectAssignments { get; set; }
        public DbSet<TaskAssignment> TasksAssignments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ////Seed ProjectMgtColors
            modelBuilder.Entity<ProjectMgtColor>().HasData(
                new ProjectMgtColor { Id = 1, Name = "Light Gray", HexCode = "#C7C4C4" },
                new ProjectMgtColor { Id = 2, Name = "Soft Red", HexCode = "#EF6A6A" },
                new ProjectMgtColor { Id = 3, Name = "Peach", HexCode = "#EC8D71" },
                new ProjectMgtColor { Id = 4, Name = "Light Orange", HexCode = "#F1BD6C" },
                new ProjectMgtColor { Id = 5, Name = "Pale Yellow", HexCode = "#F2DA6F" },
                new ProjectMgtColor { Id = 6, Name = "Light Green", HexCode = "#B3DF97" },
                new ProjectMgtColor { Id = 7, Name = "Mint Green", HexCode = "#83C9A8" },
                new ProjectMgtColor { Id = 8, Name = "Aqua", HexCode = "#4ECBC4" },
                new ProjectMgtColor { Id = 9, Name = "Light Cyan", HexCode = "#9EE7E3" },
                new ProjectMgtColor { Id = 10, Name = "Blue", HexCode = "#4573D2" },
                new ProjectMgtColor { Id = 11, Name = "Lavender", HexCode = "#A79FF3" },
                new ProjectMgtColor { Id = 12, Name = "Light Purple", HexCode = "#C48FDF" },
                new ProjectMgtColor { Id = 13, Name = "Pink", HexCode = "#F9AAEF" },
                new ProjectMgtColor { Id = 14, Name = "Hot Pink", HexCode = "#F26FB2" },
                new ProjectMgtColor { Id = 15, Name = "Salmon", HexCode = "#FC979A" },
                new ProjectMgtColor { Id = 16, Name = "Charcoal Gray", HexCode = "#6D6E6F" }
            );

            ////Seed ProjectStatuses
            modelBuilder.Entity<ProjectStatus>().HasData(
                new ProjectStatus { Id = 1, Name = "On track", HexCode = "#4573D2" },
                new ProjectStatus { Id = 2, Name = "At Risk", HexCode = "#EF6A6A" },
                new ProjectStatus { Id = 3, Name = "Off track", HexCode = "#F26FB2" },
                new ProjectStatus { Id = 4, Name = "On hold", HexCode = "#F2DA6F" },
                new ProjectStatus { Id = 5, Name = "Completed", HexCode = "#83C9A8" }
            );

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

            modelBuilder.Entity<Project>()
                .HasMany(p => p.ProjectAssignments)
                .WithOne(pu => pu.Project)
                .HasForeignKey(pu => pu.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectTask>()
                .HasMany(p => p.TaskAssignments)
                .WithOne(ta => ta.ProjectTask)
                .HasForeignKey(ta => ta.TaskId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }

}
