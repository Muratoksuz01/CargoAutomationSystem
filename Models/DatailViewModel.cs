namespace CargoAutomationSystem.Models
{

    public class DetailViewModel
    {
        public int CargoId { get; set; }
        public string HashCode { get; set; }
        public string Status { get; set; }
        public string RecipientName { get; set; }
        public string RecipientAddress { get; set; }
        public string RecipientPhone { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string SenderEmail { get; set; }
        public string SenderAddress { get; set; }
        public string SenderPhone { get; set; }
    }
}