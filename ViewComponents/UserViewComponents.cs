using System.Security.Claims;
using CargoAutomationSystem.Entity;
using CargoAutomationSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace CargoAutomationSystem.ViewComponents
{
    public class UserViewComponent : ViewComponent
    {

        private readonly List<User> Users = DataSeeding.Users;
        private readonly List<Branch> Branches = DataSeeding.Branches;
        public IViewComponentResult Invoke()
        {
            var claimsPrincipal = User as ClaimsPrincipal;

            // Claims üzerinden kullanıcı bilgilerini alıyoruz
         //   var userId = claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier);
            // var username = claimsPrincipal?.Identity?.Name;
            // var email = claimsPrincipal?.FindFirstValue(ClaimTypes.Email);
            // var address = claimsPrincipal?.FindFirstValue("Address");
            // var phone = claimsPrincipal?.FindFirstValue(ClaimTypes.MobilePhone);
            // var imageUrl = claimsPrincipal?.FindFirstValue("ImageUrl");

            int userId = int.Parse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier));

            // Kullanıcıyı sorguluyoruz ve UserInfoViewModel'e dönüştürüyoruz
            var userInfo = Users
                .Where(u => u.UserId == userId) // Filtreleme önce
                .Select(u => new UserInfoViewModel
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    Address = u.Address,
                    Phone = u.Phone,
                    ImageUrl=u.ImageUrl
                    // Diğer UserInfoViewModel alanlarını ekleyin
                })
                .FirstOrDefault();

            // Modeli doldur
          

            return View(userInfo);
        }
    }
}
