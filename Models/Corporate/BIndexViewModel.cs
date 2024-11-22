namespace CargoAutomationSystem.Models.Corporate
{
    public class BIndexViewModel()
    {
        public BranchInfoViewModel BranchInfos { get; set; }
        public List<BIndexCargoViewModel> Cargos { get; set; }
    }
}