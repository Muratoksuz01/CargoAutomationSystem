namespace CargoAutomationSystem.Entity
{

    public class Branch
    {
        public int BranchId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string BranchName { get; set; }
        public string Address { get; set; }

        // İlişkiler
        public ICollection<Cargo> Cargos { get; set; } = new List<Cargo>();
    }


}