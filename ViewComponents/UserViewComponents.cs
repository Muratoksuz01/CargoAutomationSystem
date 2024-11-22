using System.Security.Claims;
using CargoAutomationSystem.Entity;
using CargoAutomationSystem.Models;
using CargoAutomationSystem.Models.Users;
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
