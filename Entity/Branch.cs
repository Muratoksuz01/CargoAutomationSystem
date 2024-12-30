namespace CargoAutomationSystem.Entity
{
    

 

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

  

  

  
}
