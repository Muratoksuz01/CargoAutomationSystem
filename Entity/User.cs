namespace CargoAutomationSystem.Entity
{
      public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string ImageUrl { get; set; }
        public bool IsTemporary { get; set; }

        // Relationships
        public List<UserCargo> UserCargos { get; set; } = new List<UserCargo>();
    }
}