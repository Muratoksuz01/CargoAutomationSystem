using System.Security.Claims;
using CargoAutomationSystem.Data;
using CargoAutomationSystem.Entity;
using CargoAutomationSystem.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CargoAutomationSystem.ViewComponents
{
    public class UserViewComponent : ViewComponent
    {
        private readonly CargoDbContext _context;

        public UserViewComponent(CargoDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var claimsPrincipal = User as ClaimsPrincipal;

            int userId = int.Parse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier));

            // Kullanıcıyı sorguluyoruz ve UserInfoViewModel'e dönüştürüyoruz
            var userInfo = _context.Users
                .Where(u => u.UserId == userId) // Filtreleme önce
                .Select(u => new UserInfoViewModel
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    Address = u.Address,
                    Phone = u.Phone,
                    ImageUrl = u.ImageUrl
                    // Diğer UserInfoViewModel alanlarını ekleyin
                })
                .FirstOrDefault();

            // Modeli doldur
            return View(userInfo);
        }
    }
}
