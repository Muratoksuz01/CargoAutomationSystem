using Microsoft.AspNetCore.Mvc;

namespace CargoAutomationSystem.Controllers
{
    public class UserController : Controller
    {

         public IActionResult Index()
        {
            // Kargo gönderim işlemiyle ilgili hazırlıklar yapılabilir
            return View(); // SendCargo.cshtml dosyasına yönlendirir
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
