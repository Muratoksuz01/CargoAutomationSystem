namespace CargoAutomationSystem.Models.Users
{
    public class TrackCargoViewModel()
    {
        
    public int CargoId { get; set; } // Otomatik artan ID
    public int SenderId { get; set; } // Gönderici kullanıcı ID'si (User ile ilişki)
    public  string SenderName { get; set; }
    public int CurrentBranchId { get; set; } // Mevcut şube ID'si (Branch ile ilişki)

    // Alıcı bilgileri, sistemde kayıtlı değilse buradan tutulacak
    public string RecipientName { get; set; }
    public string RecipientAddress { get; set; }
    public string RecipientPhone { get; set; }

    // Diğer alanlar
    public string HashCode { get; set; } // Benzersiz takip kodu
    public string Status { get; set; } // Kargonun durumu
    }

}