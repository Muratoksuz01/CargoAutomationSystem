namespace CargoAutomationSystem.Models.Users
{
    public class IndexCargoViewModel()
    {
        public int CargoId { get; set; } 
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string Status { get; set; }
        public string HashCode { get; set; }
    }
}