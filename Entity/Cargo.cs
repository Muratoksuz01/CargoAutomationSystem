using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CargoAutomationSystem.Entity
{
    public class Cargo
    {
        [Key]
        public int CargoId { get; set; }

        [Required]
        [EmailAddress]
        public string SenderEmail { get; set; }

        [Required]
        public string SenderAddress { get; set; }

        [Required]
        public string SenderUserName { get; set; }

        [Required]
        [Phone]
        public string SenderPhone { get; set; }

        [Required]
        public string ReceiverAddress { get; set; }

        [Required]
        [EmailAddress]
        public string ReceiverEmail { get; set; }

        [Required]
        public string ReceiverUserName { get; set; }

        [Required]
        [Phone]
        public string ReceiverPhone { get; set; }

        // Şube ile ilişki
        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        // CargoInfo ile ilişki
        public ICollection<CargoInfo> CargoInfos { get; set; } = new List<CargoInfo>();
    }
}
