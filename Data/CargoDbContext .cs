using CargoAutomationSystem.Entity;
using Microsoft.EntityFrameworkCore;
namespace CargoAutomationSystem.Data
{
    public class CargoDbContext : DbContext
    {
        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<CargoProcess> CargoProcesses { get; set; }
        public DbSet<UserCargo> UserCargos { get; set; }
        public DbSet<BranchCargo> BranchCargos { get; set; }

        public CargoDbContext(DbContextOptions<CargoDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User-Cargo many-to-many relationship
            modelBuilder.Entity<UserCargo>()
                .HasKey(uc => new { uc.UserId, uc.CargoId });

            modelBuilder.Entity<UserCargo>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserCargos)  // User'daki UserCargos koleksiyonu "IEnumerable<UserCargo>" olmalı
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Silme davranışı eklendi

            modelBuilder.Entity<UserCargo>()
                .HasOne(uc => uc.Cargo)
                .WithMany(c => c.UserCargos)  // Cargo'daki UserCargos koleksiyonu "IEnumerable<UserCargo>" olmalı
                .HasForeignKey(uc => uc.CargoId)
                .OnDelete(DeleteBehavior.Cascade); // Silme davranışı eklendi

            // Branch-Cargo many-to-many relationship
            modelBuilder.Entity<BranchCargo>()
                .HasKey(bc => new { bc.BranchId, bc.CargoId });

            modelBuilder.Entity<BranchCargo>()
                .HasOne(bc => bc.Branch)
                .WithMany(b => b.BranchCargos)
                .HasForeignKey(bc => bc.BranchId)
                .OnDelete(DeleteBehavior.Cascade); // Silme davranışı eklendi

            modelBuilder.Entity<BranchCargo>()
                .HasOne(bc => bc.Cargo)
                .WithMany(c => c.BranchCargos)
                .HasForeignKey(bc => bc.CargoId)
                .OnDelete(DeleteBehavior.Cascade); // Silme davranışı eklendi
        }
    }
}