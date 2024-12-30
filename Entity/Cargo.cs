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
}

