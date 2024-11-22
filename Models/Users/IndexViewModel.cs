namespace CargoAutomationSystem.Models.Users
{
    public class IndexViewModel()
    {
        public UserInfoViewModel UserInfos { get; set; }
        public List<IndexCargoViewModel> Cargos { get; set; }
    }
}