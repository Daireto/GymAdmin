using GymAdmin.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GymAdmin.Enums;
using GymAdmin.Helpers;
namespace GymAdmin.Data
{

    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Service> Services { get; set; }
        //public DbSet<...> ... { get; set; } --> Entidades

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasIndex("Document", "DocumentType").IsUnique();
            modelBuilder.Entity<Service>().HasIndex(s => s.Name).IsUnique();
        }
    }
}