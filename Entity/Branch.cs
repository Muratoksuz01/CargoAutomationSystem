using System.ComponentModel.DataAnnotations;

namespace CargoAutomationSystem.Entity
{
    public class Branch
    {
        [Key]
        public int BranchId { get; set; }

        [Required]
        [StringLength(100)]
        public string BranchName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Address { get; set; }

        // Şubedeki kargolar için ilişkili özellik
        public ICollection<Cargo> Cargos { get; set; } = new List<Cargo>();
    }
}
