namespace CargoAutomationSystem.Entity
{
    
public class Cargo
{
    public int CargoId { get; set; }
    public int SenderId { get; set; }  // Gönderen kullanıcı
    public int CurrentBranchId { get; set; }  // Şube ID
    public string RecipientName { get; set; }
    public string RecipientAddress { get; set; }
    public string RecipientPhone { get; set; }
    public string HashCode { get; set; }
    public string Status { get; set; }
}
}

