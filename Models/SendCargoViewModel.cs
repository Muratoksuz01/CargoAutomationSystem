using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class SendCargoViewModel
{
    // Gönderici bilgileri (sadece okunabilir şekilde)
    [Display(Name = "Gönderici Email")]
    [ReadOnly(true)]
    public string SenderEmail { get; set; }

    [Display(Name = "Gönderici Kullanıcı Adı")]
    [ReadOnly(true)]
    public string SenderUsername { get; set; }

    [Display(Name = "Gönderici Adresi")]
    [ReadOnly(true)]
    public string SenderAddress { get; set; }

    [Display(Name = "Gönderici Telefon")]
    [ReadOnly(true)]
    public string SenderPhone { get; set; }

    // Şube bilgisi, seçilen şubeden gelir
    [Required(ErrorMessage = "Gönderici şubesi zorunludur")]
    public int SenderBranchId { get; set; } 

    // Alıcı bilgileri
    [Required(ErrorMessage = "Alıcı adı zorunludur")]
    public string RecipientName { get; set; }

    [Required(ErrorMessage = "Alıcı adresi zorunludur")]
    public string RecipientAddress { get; set; }

    [Required(ErrorMessage = "Telefon numarası zorunludur")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası girin")]
    public string RecipientPhone { get; set; }

}
