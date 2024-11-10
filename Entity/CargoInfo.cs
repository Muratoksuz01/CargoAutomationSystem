using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CargoAutomationSystem.Entity
{
    public class CargoInfo
    {
        [Key]
        public int CargoInfoId { get; set; }

        [Required]
        public int CargoId { get; set; }

        public Cargo Cargo { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } // "Taşımada", "İptal Edildi", "Tamamlandı" gibi değerler alabilir

        [Required]
        public DateTime Date { get; set; } // İşlem tarihi

        // CargoProcess ile ilişki
        public ICollection<CargoProcess> CargoProcesses { get; set; } = new List<CargoProcess>();
    }
}
