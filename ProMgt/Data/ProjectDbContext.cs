using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace ProMgt.Data
{
    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> opt) : base(opt)
        {

        }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTask> Tasks { get; set; }
        public DbSet<Contact> Contacts { get; set; }
    }
}
