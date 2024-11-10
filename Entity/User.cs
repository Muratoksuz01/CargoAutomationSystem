using System.ComponentModel.DataAnnotations;

namespace CargoAutomationSystem.Entity
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Address { get; set; }

        [Phone]
        public string Phone { get; set; }

        public string ImageUrl { get; set; }
    }
}
