using Microsoft.AspNetCore.Mvc;

namespace CargoAutomationSystem.Controllers
{
    public class BranchController : Controller
    {
        // "All Cargos" linki için çalışan List() metodu
         public IActionResult Index()
        {
            // Tüm kargo verilerini model olarak view'e taşıyabilirsiniz
            return View(); // İlgili view sayfası olan 'List.cshtml' görüntülenir
        }
        public IActionResult List()
        {
            // Tüm kargo verilerini model olarak view'e taşıyabilirsiniz
            return View(); // İlgili view sayfası olan 'List.cshtml' görüntülenir
        }

        // "Log Out" için çalışan LogOut() metodu
        public IActionResult LogOut()
        {
            // Kullanıcı oturumunu sonlandırma işlemleri
            HttpContext.Session.Clear();  // Oturumu temizler
            return RedirectToAction("Login", "Home"); // Login sayfasına yönlendirir
        }

        // "Ayarlar" için çalışan Setting() metodu
        public IActionResult Setting()
        {
            // Kullanıcıya ait ayar sayfasını view olarak dönebilirsiniz
            return View(); // 'Setting.cshtml' view sayfasını gösterir
        }
    }
}
