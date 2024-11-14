using System.Security.Claims;
using CargoAutomationSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace CargoAutomationSystem.ViewComponents
{
    public class UserViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var claimsPrincipal = User as ClaimsPrincipal;

            // Claims üzerinden kullanıcı bilgilerini alıyoruz
            var userId = claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = claimsPrincipal?.Identity?.Name;
            var email = claimsPrincipal?.FindFirstValue(ClaimTypes.Email);
            var address = claimsPrincipal?.FindFirstValue("Address");
            var phone = claimsPrincipal?.FindFirstValue(ClaimTypes.MobilePhone);
            var imageUrl = claimsPrincipal?.FindFirstValue("ImageUrl");

            // Modeli doldur
            var userInfo = new UserInfoViewModel
            {
                UserId = int.TryParse(userId, out var id) ? id : 0,
                Username = username,
                Email = email,
                Address = address,
                Phone = phone,
                ImageUrl = imageUrl
            };

            return View(userInfo);
        }
    }
}
