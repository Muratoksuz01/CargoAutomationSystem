using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CargoAutomationSystem.Models.Users
{


    public class SendCargoViewModel
    {
        // Gönderici bilgileri (sadece okunabilir şekilde)
        public int SenderId { get; set; }

        [Display(Name = "Gönderici Email")]
             public string? SenderEmail { get; set; }

        [Display(Name = "Gönderici Kullanıcı Adı")]
             public string? SenderUsername { get; set; }

        [Display(Name = "Gönderici Adresi")]
            public string? SenderAddress { get; set; }

        [Display(Name = "Gönderici Telefon")]
             public string? SenderPhone { get; set; }

        // Şube bilgisi, seçilen şubeden gelir
        [Required(ErrorMessage = "Gönderici şubesi zorunludur")]
        [Display(Name = "Şube Seçiniz")]
              public int SenderBranchId { get; set; }

        // Alıcı bilgileri
        [Required(ErrorMessage = "Alıcı adı zorunludur")]
             public string RecipientName { get; set; }

        [Required(ErrorMessage = "Alıcı adresi zorunludur")]
             public string RecipientAddress { get; set; }

        [Required(ErrorMessage = "Telefon numarası zorunludur")]
        //[Phone(ErrorMessage = "Geçerli bir telefon numarası girin")]
        [PhoneNumberValidation]
             public string RecipientPhone { get; set; }

    }
}