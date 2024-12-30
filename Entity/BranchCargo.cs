using CargoAutomationSystem.Entity;

  public class BranchCargo
    {   
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int CargoId { get; set; }

        // Navigation properties
        public Branch Branch { get; set; }
        public Cargo Cargo { get; set; }
    }
