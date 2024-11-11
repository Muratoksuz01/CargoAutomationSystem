using CargoAutomationSystem.Entity;

public class Cargo
{
    public int CargoId { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public int CurrentBranchId { get; set; }
    public string HashCode { get; set; }
    public string Status { get; set; }
    
    // İlişkiler
    public User Sender { get; set; } // Gönderici kullanıcı bilgisi
    public User Receiver { get; set; } // Alıcı kullanıcı bilgisi
    public Branch CurrentBranch { get; set; } // Mevcut şube bilgisi
    public ICollection<CargoInfo> CargoInfos { get; set; } = new List<CargoInfo>(); // Cargo bilgileri ile ilişki
}
