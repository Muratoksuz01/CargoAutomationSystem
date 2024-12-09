namespace CargoAutomationSystem.Entity
{
    public class Cargo
    {
        public int CargoId { get; set; }
        public int SenderId { get; set; }  
        public int RecipientId { get; set; }
        public int CurrentBranchId { get; set; }
        public string RecipientName { get; set; }
        public string RecipientAddress { get; set; }
        public string RecipientPhone { get; set; }
        public string HashCode { get; set; }
        public string Status { get; set; }

        // Relationships
        public User Sender { get; set; }
        public User Recipient { get; set; }
        public Branch CurrentBranch { get; set; }

        public List<CargoProcess> CargoProcesses { get; set; } = new List<CargoProcess>();
        public List<UserCargo> UserCargos { get; set; } = new List<UserCargo>();
        public List<BranchCargo> BranchCargos { get; set; } = new List<BranchCargo>();
    }

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

    public class Branch
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }

        // Relationships
        public List<BranchCargo> BranchCargos { get; set; } = new List<BranchCargo>();
    }

    public class UserCargo
    {
        public int UserId { get; set; }
        public int CargoId { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Cargo Cargo { get; set; }
    }

    public class BranchCargo
    {
        public int BranchId { get; set; }
        public int CargoId { get; set; }

        // Navigation properties
        public Branch Branch { get; set; }
        public Cargo Cargo { get; set; }
    }

    public class CargoProcess
    {
        public int CargoProcessId { get; set; }
        public int CargoId { get; set; }
        public string Process { get; set; } 
        public DateTime ProcessDate { get; set; } = DateTime.Now;

        // Relationships
        public Cargo Cargo { get; set; }
    }
}
