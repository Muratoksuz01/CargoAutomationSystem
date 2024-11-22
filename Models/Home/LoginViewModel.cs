using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace CargoAutomationSystem.Models.Home
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre gereklidir.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }

}
