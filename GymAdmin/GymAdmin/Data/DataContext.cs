using GymAdmin.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace GymAdmin.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        //Entities
        public DbSet<Service> Services { get; set; }
        public DbSet<Professional> Professionals { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Director> Directors { get; set; }

        //Relationships
        public DbSet<ServiceAccess> ServiceAccesses { get; set; }
        public DbSet<ProfessionalSchedule> ProfessionalSchedules { get; set; }
        public DbSet<PlanInscription> PlanInscriptions { get; set; }
        public DbSet<EventInscription> EventInscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasIndex("Document", "DocumentType").IsUnique();
            modelBuilder.Entity<Service>().HasIndex(s => s.Name).IsUnique();
            modelBuilder.Entity<Schedule>().HasIndex("Day", "StartHour", "FinishHour").IsUnique();
            modelBuilder.Entity<Professional>().HasIndex("UserId").IsUnique();
            modelBuilder.Entity<ProfessionalSchedule>().HasIndex("ProfessionalId", "ScheduleId").IsUnique();
            modelBuilder.Entity<Plan>().HasIndex(p=>p.Name).IsUnique();
            modelBuilder.Entity<Event>().HasIndex(e=>e.Name).IsUnique();
            modelBuilder.Entity<Director>().HasIndex("UserId").IsUnique();
        }
    }
}