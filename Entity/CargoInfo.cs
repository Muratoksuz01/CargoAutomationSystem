public class CargoInfo
{
    public int CargoInfoId { get; set; }
    public int CargoId { get; set; }
    public string Status { get; set; } // "Taşımada", "İptal Edildi", "Tamamlandı"
    public DateTime Date { get; set; } = DateTime.Now;
    
    // İlişkiler
    public Cargo Cargo { get; set; } // İlgili cargo bilgisi
    public ICollection<CargoProcess> CargoProcesses { get; set; } = new List<CargoProcess>(); // Cargo süreç bilgileri ile ilişki
}
