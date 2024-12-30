namespace CargoAutomationSystem.Entity
{
    
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
