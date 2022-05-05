using GymAdmin.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace GymAdmin.Data
{

    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        
        public DbSet<Director> Directors { get; set; }
        public DbSet<Event> Events{ get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceAccess> ServiceAccesses { get; set; }
        public DbSet<Professional> Professionals { get; set; }
        public DbSet<Schedule> Schedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasIndex("Document", "DocumentType").IsUnique();
            modelBuilder.Entity<Service>().HasIndex(s => s.Name).IsUnique();
            modelBuilder.Entity<Schedule>().HasIndex("Day", "StartHour", "FinishHour").IsUnique();
            modelBuilder.Entity<Professional>().HasIndex("UserId", "ProfessionalType").IsUnique();
        }
    }
}