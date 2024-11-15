namespace CargoAutomationSystem.Entity
{
    public class User
    {
        private readonly List<User> Users = DataSeeding.Users;

        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string ImageUrl { get; set; }

        // İlişkiler
        public ICollection<Cargo> SentCargos { get; set; } = new List<Cargo>(); // Kullanıcıya ait gönderilen kargolar
        public ICollection<Cargo> ReceivedCargos { get; set; } = new List<Cargo>(); // Kullanıcıya ait alınan kargolar

        public override string ToString()
        {
            return $"UserId: {UserId}, Username: {Username}, Email: {Email}, Address: {Address}, Phone: {Phone}, ImageUrl: {ImageUrl}";
        }
      

    }
}