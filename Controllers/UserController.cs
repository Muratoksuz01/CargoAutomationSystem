using System.Security.Claims;
// using CargoAutomationSystem.Entity;
using CargoAutomationSystem.Models;
using CargoAutomationSystem.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using CargoAutomationSystem.Entity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CargoAutomationSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly List<User> Users = DataSeeding.Users;

        private readonly List<Branch> Branches = DataSeeding.Branches;
        private readonly List<Cargo> Cargos = DataSeeding.Cargos;

        protected UserInfoViewModel CurrentUser => new UserInfoViewModel
        {
            UserId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0,
            Username = User.FindFirstValue(ClaimTypes.Name),
            Email = User.FindFirstValue(ClaimTypes.Email),
            Address = User.FindFirstValue("Address"),
            Phone = User.FindFirstValue(ClaimTypes.MobilePhone),
            ImageUrl = User.FindFirstValue("ImageUrl")
        };


        public IActionResult SendCargo()
        {
            var branches = Branches.ToList();

            var sendCargoModel = new SendCargoViewModel
            {
                SenderEmail = CurrentUser.Email,
                SenderUsername = CurrentUser.Username,
                SenderAddress = CurrentUser.Address,
                SenderPhone = CurrentUser.Phone
            };
            ViewBag.Branches = new SelectList(branches, "BranchId", "BranchName");

            return View(sendCargoModel);
        }
        [HttpPost]
        public IActionResult SendCargo(SendCargoViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Yeni kargo oluşturma
             //   var nextId = DataSeeding.Cargos.Any() ? DataSeeding.Cargos.Max(c => c.CargoId) + 1 : 1;

                var newCargo = new Cargo()
                {
                    CargoId=Cargos.ToList().Count()+1,
                    SenderId = CurrentUser.UserId,
                    CurrentBranchId = model.SenderBranchId,
                    RecipientName = model.RecipientName,
                    RecipientAddress = model.RecipientAddress,
                    RecipientPhone = model.RecipientPhone,
                    HashCode = GenerateUniqueHashCode(),
                    Status = "Taşımada"
                };

                Cargos.Add(newCargo);

                System.Console.WriteLine("Yeni Kargo Eklendi:");
                System.Console.WriteLine(newCargo);

                return RedirectToAction("Index", "Home");
            }
            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                foreach (var error in errors)
                {
                    // Key ve hata mesajını konsola yazdır
                    System.Console.WriteLine($"Key:{key}, Hata Mesajı: {error.ErrorMessage}");
                }
            }
            ViewBag.Branches = new SelectList(Branches.ToList(), "BranchId", "BranchName");
            return View(model);
        }







        private string GenerateUniqueHashCode()
        {
            return Guid.NewGuid().ToString();
        }
        public IActionResult Index()

        {
            System.Console.WriteLine("User index sfoksiyonu  ");
            return View(CurrentUser);
        }

        public IActionResult TrackCargo()
        {
            return View();
        }

        public IActionResult LogOut()
        {
            // Oturumu temizle
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Home");
        }

        public IActionResult Setting()
        {
            return View(CurrentUser);
        }

    }
}
