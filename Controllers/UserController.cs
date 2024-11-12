using System.Security.Claims;
using CargoAutomationSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace CargoAutomationSystem.Controllers
{
    public class UserController : Controller
    {

        public IActionResult Index()
        {
            System.Console.WriteLine("User index sfoksiyonu  ");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.FindFirstValue(ClaimTypes.Name);
System.Console.WriteLine(username.GetType());
            var email = User.FindFirstValue(ClaimTypes.Email);
            var address = User.FindFirstValue("Address");
            var phone = User.FindFirstValue(ClaimTypes.MobilePhone);
            var imageUrl = User.FindFirstValue("ImageUrl");

            // Kullanıcı bilgilerini UserInfoViewModel'e ekliyoruz
            var userInfos = new UserInfoViewModel
            {
                UserId = int.TryParse(userId, out var id) ? id : 0, // userId null değilse integer olarak eklenir
                Username = username,
                Email = email,
                Address = address,
                Phone = phone,
                ImageUrl = imageUrl
            };
            // Kargo gönderim işlemiyle ilgili hazırlıklar yapılabilir
            return View(userInfos); // SendCargo.cshtml dosyasına yönlendirir
        }

        // Kargo gönderme sayfasını gösterir
        public IActionResult SendCargo()
        {
            // Kargo gönderim işlemiyle ilgili hazırlıklar yapılabilir
            return View(); // SendCargo.cshtml dosyasına yönlendirir
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
            return RedirectToAction("Login", "Account"); // Login sayfasına yönlendir
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
