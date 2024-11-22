using System.ComponentModel.DataAnnotations;

namespace CargoAutomationSystem.Models.Corporate
{
    
    public class CorporateRegisterViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı gereklidir.")]
        [StringLength(100, ErrorMessage = "Kullanıcı adı {0} karakterden fazla olamaz.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre gereklidir.")]
        [StringLength(100, ErrorMessage = "Şifre {0} karakterden kısa olamaz.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifre tekrar gereklidir.")]
        [Compare(nameof(Password), ErrorMessage = "Şifreler uyuşmuyor.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "kurumsal address gerekli ")]
        [StringLength(1000, ErrorMessage = "Kullanıcı adı {0} karakterden fazla olamaz. Minumum {1} karakter olmalıdır.", MinimumLength = 10)]
        public string Address { get; set; }
    }
}