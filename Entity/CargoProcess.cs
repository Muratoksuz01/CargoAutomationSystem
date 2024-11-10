using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CargoAutomationSystem.Entity
{
    public class CargoProcess
    {
        [Key]
        public int CargoProcessId { get; set; }

        [Required]
        public int CargoInfoId { get; set; }

        public CargoInfo CargoInfo { get; set; }

        [Required]
        [StringLength(200)]
        public string Process { get; set; } // Yapılan işlem açıklaması
    }
}
