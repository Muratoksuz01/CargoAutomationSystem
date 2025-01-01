namespace CargoAutomationSystem.Models
{

    public class EmailSettings
    {
        public string SMTPServer { get; set; }
        public int Port { get; set; }
        public string SenderEmail { get; set; }
        public string Password { get; set; }
    }
}