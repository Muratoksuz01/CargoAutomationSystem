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

        public IActionResult Setting()
        {
            return View(CurrentUser);
        }
        [HttpPost]
        public IActionResult Setting(UserSettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcıyı buluyoruz
                var currentUser = Users.SingleOrDefault(u => u.UserId == model.UserId);

                if (currentUser != null)
                {
                    // Kullanıcı bilgilerini güncelliyoruz
                    currentUser.Username = model.Username;
                    currentUser.Email = model.Email;
                    currentUser.Address = model.Address;
                    currentUser.Phone = model.Phone;
                    currentUser.ImageUrl = model.ImageUrl;

                    // Bu işlemi veritabanına kaydetmek için uygun bir işlem yapılabilir.
                    // Örneğin, _context.SaveChanges(); gibi.
                }

                return RedirectToAction("Setting"); // Değişiklikler yapıldıktan sonra tekrar ayarlar sayfasına yönlendiriyoruz
            }

            return View(model);
        }







        public IActionResult Detail(string hashCode)
        {
            if (string.IsNullOrEmpty(hashCode))
            {
                return NotFound("Hash code is required.");
            }

            var cargo = DataSeeding.Cargos.SingleOrDefault(c => c.HashCode == hashCode);
            if (cargo == null)
            {
                return NotFound($"No cargo found with hash code: {hashCode}");
            }

            var sender = DataSeeding.Users.SingleOrDefault(u => u.UserId == cargo.SenderId);

            var detail = new DetailViewModel
            {
                CargoId = cargo.CargoId,
                HashCode = cargo.HashCode,
                Status = cargo.Status,

                RecipientName = cargo.RecipientName,
                RecipientAddress = cargo.RecipientAddress,
                RecipientPhone = cargo.RecipientPhone,

                SenderId = sender?.UserId ?? 0,
                SenderUsername = sender?.Username,
                SenderEmail = sender?.Email,
                SenderAddress = sender?.Address,
                SenderPhone = sender?.Phone
            };

            System.Console.WriteLine($"Detail fetched for cargo hash code: {hashCode}");
            return View(detail);
        }

        [HttpGet]
        public IActionResult TrackCargo()
        {
            var userPhone = CurrentUser.Phone;

            if (string.IsNullOrWhiteSpace(userPhone))
            {
                ModelState.AddModelError("", "Kullanıcı telefon bilgisi eksik. Lütfen giriş yapın.");
                return View(new List<TrackCargoViewModel>());
            }

            // Kullanıcının gönderici veya alıcı olduğu kargoları bul
            var cargos = Cargos
                .Where(c => c.RecipientPhone == userPhone)
                .Select(c => new TrackCargoViewModel
                {
                    CargoId = c.CargoId,
                    SenderId = c.SenderId,
                    CurrentBranchId = c.CurrentBranchId,
                    RecipientName = c.RecipientName,
                    RecipientAddress = c.RecipientAddress,
                    RecipientPhone = c.RecipientPhone,
                    HashCode = c.HashCode,
                    Status = c.Status
                })
                .ToList();

            return View(cargos);
        }

        [HttpPost]
        public IActionResult TrackCargo(string hashCode)
        {
            if (string.IsNullOrWhiteSpace(hashCode))
            {
                ModelState.AddModelError("", "Lütfen bir takip kodu girin.");
                return TrackCargo(); // GET metodu tekrar çağırılır
            }

            var cargos = Cargos
                .Where(c => c.HashCode == hashCode)
                .Select(c => new TrackCargoViewModel
                {
                    CargoId = c.CargoId,
                    SenderId = c.SenderId,
                    CurrentBranchId = c.CurrentBranchId,
                    RecipientName = c.RecipientName,
                    RecipientAddress = c.RecipientAddress,
                    RecipientPhone = c.RecipientPhone,
                    HashCode = c.HashCode,
                    Status = c.Status
                })
                .ToList();

            if (!cargos.Any())
            {
                ModelState.AddModelError("", "Bu takip koduna ait kargo bulunamadı.");
            }

            return View(cargos);
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

        public IActionResult LogOut()
        {
            // Oturumu temizle
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Home");
        }

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
                    CargoId = Cargos.ToList().Count() + 1,
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
                return RedirectToAction("Index", "User");
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


    }


}
