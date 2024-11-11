public class CargoProcess
{
    public int CargoProcessId { get; set; }
    public int CargoInfoId { get; set; }
    public string Process { get; set; } // "Şubeden Gönderildi", "Alıcıya Ulaştı", "Gönderi İptal"
    public DateTime ProcessDate { get; set; } = DateTime.Now;
    
    // İlişkiler
    public CargoInfo CargoInfo { get; set; } // İlgili cargo bilgi süreci
}
