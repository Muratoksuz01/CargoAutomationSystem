using CargoAutomationSystem.Entity;

  public class UserCargo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CargoId { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Cargo Cargo { get; set; }
    }
