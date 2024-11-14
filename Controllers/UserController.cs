using System.Security.Claims;
using CargoAutomationSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace CargoAutomationSystem.Controllers
{
    public class UserController : Controller
    {

        protected UserInfoViewModel CurrentUser => new UserInfoViewModel
        {
            UserId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0,
            Username = User.FindFirstValue(ClaimTypes.Name),
            Email = User.FindFirstValue(ClaimTypes.Email),
            Address = User.FindFirstValue("Address"),
            Phone = User.FindFirstValue(ClaimTypes.MobilePhone),
            ImageUrl = User.FindFirstValue("ImageUrl")
        };

        public IActionResult Index()
        {
            System.Console.WriteLine("User index sfoksiyonu  ");

            // Kullanıcı bilgilerini UserInfoViewModel'e ekliyoruz

            // Kargo gönderim işlemiyle ilgili hazırlıklar yapılabilir
            return View(CurrentUser); // SendCargo.cshtml dosyasına yönlendirir
        }

        // Kargo gönderme sayfasını gösterir
        public IActionResult SendCargo()
        {
            var sendCargoModel = new SendCargoViewModel
            {
                SenderEmail = CurrentUser.Email,
                SenderUsername = CurrentUser.Username,
                SenderAddress = CurrentUser.Address,
                SenderPhone = CurrentUser.Phone
            };

            return View(sendCargoModel);
        }

        // Kargo takibi sayfasını gösterir
        public IActionResult TrackCargo()
        {
            // Kargo takip işlemi ile ilgili veriler hazırlanabilir
            return View(); // TrackCargo.cshtml dosyasına yönlendirir
        }

        // Kullanıcı oturumunu kapatır
        public IActionResult LogOut()
        {
            // Oturumu temizle
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // Çerezle giriş yapan kullanıcıyı çıkartıyoruz

            return RedirectToAction("Login", "Home"); // Login sayfasına yönlendir
        }

        // Kullanıcı ayarlarını gösterir
        public IActionResult Setting()
        {
            // Kullanıcı ayarlarını yükleyebilir veya güncellenebilir
            var userSettings = new { Username = "User1", Email = "user1@example.com" }; // Örnek veri
            return View(userSettings); // Setting.cshtml dosyasına yönlendirir
        }
    }
}
