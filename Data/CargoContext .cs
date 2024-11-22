using Microsoft.EntityFrameworkCore;
using CargoAutomationSystem.Entity;
namespace CargoAutomationSystem.Data
{
    public class CargoContext : DbContext
    {
        public CargoContext(DbContextOptions<CargoContext> options) : base(options) {}

        public DbSet<User> Users { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<CargoProcess> CargoProcesses { get; set; }

        // Veritabanı ilişkilerini ve tabloların yapılandırmalarını burada tanımlayabilirsiniz.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // İlişki ve kısıtlamaları burada tanımlayabilirsiniz.
        }
    }

  
}
