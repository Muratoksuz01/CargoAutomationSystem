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


        };



















        public IActionResult Settings()
        {
            System.Console.WriteLine("setting de ");

            var user = Users.FirstOrDefault(i => i.UserId == CurrentUser.UserId);
            var model = new SettingsViewModel
            {
                UpdateUsername = new UpdateUsernameViewModel { Username = user.Username },
                EditInfo = new EditInfoViewModel { Email = user.Email, Phone = user.Phone, Address = user.Address },
                UpdatePassword = new UpdatePasswordViewModel(),
                UpdateImage = new UpdateImageViewModel { ImageFile = user.ImageUrl }
            };
            System.Console.WriteLine(model.UpdateImage.ImageFile);
            return View(model);
        }

        [HttpPost]
        public IActionResult UpdatePassword(SettingsViewModel model)
        {
            var user = Users.FirstOrDefault(i => i.UserId == CurrentUser.UserId);

            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Field: {key}, Error: {error.ErrorMessage}");
                    }
                }

                model.UpdateUsername = new UpdateUsernameViewModel { Username = user.Username };
                model.EditInfo = new EditInfoViewModel { Email = user.Email, Phone = user.Phone, Address = user.Address };
                model.UpdateImage = new UpdateImageViewModel { ImageFile = user.ImageUrl };
                return View("Settings", model); // Ana modeli döndürüyoruz.
            }

            if (user.Password != model.UpdatePassword.CurrentPassword)
            {
                model.UpdateUsername = new UpdateUsernameViewModel { Username = user.Username };
                model.EditInfo = new EditInfoViewModel { Email = user.Email, Phone = user.Phone, Address = user.Address };
                model.UpdateImage = new UpdateImageViewModel { ImageFile = user.ImageUrl };
                ModelState.AddModelError("UpdatePassword.CurrentPassword", "Current password is incorrect.");
                return View("Settings", model);
            }

            user.Password = model.UpdatePassword.NewPassword;
            //                                                       sonra burada saveChanges gelecek

            //     TempData["Message"] = "Password updated successfully!";
            return RedirectToAction("Settings");
        }
        [HttpPost]
        public IActionResult UpdateImage(SettingsViewModel model, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid image upload.";
                return View("Settings", model);
            }

            var user = Users.FirstOrDefault(i => i.UserId == CurrentUser.UserId);
            if (file != null)
            {
                // Dosyayı kaydetmek için bir yol belirleyin (örneğin "wwwroot/uploads")
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                // Benzersiz bir dosya adı oluşturun
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                // Dosyayı kaydedin
                var filePath = Path.Combine(uploadsPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // Kullanıcıya bu dosyanın yolunu atayın
                user.ImageUrl = fileName;

                // SaveChanges burada yapılacak
            }

            //    TempData["Message"] = "Image updated successfully!";
            return RedirectToAction("Settings");
        }


        [HttpPost]
        public IActionResult UpdateInfo(SettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //        TempData["Error"] = "Password update failed.";
                return View("Settings", model); // Ana modeli döndürüyoruz.
            }
            var user = Users.FirstOrDefault(i => i.UserId == CurrentUser.UserId);
            user.Email = model.EditInfo.Email;
            user.Address = model.EditInfo.Address;
            user.Phone = model.EditInfo.Phone;
            return RedirectToAction("Settings");
        }

        [HttpPost]
        public IActionResult RemoveImage()
        {
            var user = Users.FirstOrDefault(i => i.UserId == CurrentUser.UserId);
            user.ImageUrl = "";
            return RedirectToAction("Settings");
        }


        [HttpPost]
        public IActionResult UpdateUsername(SettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //       TempData["Error"] = "Invalid username.";
                return View("Settings", model);
            }

            var user = Users.FirstOrDefault(i => i.UserId == CurrentUser.UserId);
            user.Username = model.UpdateUsername.Username;
            //                                                       sonra burada saveChanges gelecek

            //  TempData["Message"] = "Username updated successfully!";
            return RedirectToAction("Settings");
        }

        public IActionResult SendCargo()
        {
            var user = Users.FirstOrDefault(i => i.UserId == CurrentUser.UserId);

            var branches = Branches.ToList();

            var sendCargoModel = new SendCargoViewModel
            {
                SenderEmail = user.Email,
                SenderUsername = user.Username,
                SenderAddress = user.Address,
                SenderPhone = user.Phone
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
        public IActionResult Index()
        {
            var user = Users.Where(i => i.UserId == CurrentUser.UserId)
             .Select(c => new IndexViewModel
             {
                 UserInfos = new UserInfoViewModel
                 {
                     Username = c.Username,
                     Email = c.Email,
                     Phone = c.Phone,
                     Address = c.Address,
                     ImageUrl = c.ImageUrl
                 },
                 Cargos = Cargos
                     .Where(a => a.RecipientPhone == c.Phone && a.Status != "Tamamlandı") // Telefon numarasına göre filtreleme
                     .Select(a => new IndexCargoViewModel
                     {

                         CargoId = a.CargoId,
                         SenderName = Users.Where(y => y.UserId == a.SenderId).Select(y => y.Username).FirstOrDefault(),
                         Status = a.Status,
                         HashCode = a.HashCode
                     }).ToList() // Filtrelenmiş kargoların listesini oluştur
             }).FirstOrDefault();
            System.Console.WriteLine("User index fonksiyonu çalıştı.");
            return View(user);
        }


        public IActionResult Detail(string hashCode)
        {
         

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
                CurrentBranch=Branches.Where( a=>a.BranchId == cargo.CurrentBranchId).Select(a=>a.BranchName).FirstOrDefault(),

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

        public IActionResult TrackCargo()
        {
            var user = Users.FirstOrDefault(i => i.UserId == CurrentUser.UserId);

            var userPhone = user.Phone;

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
                    SenderName=Users.Where(y => y.UserId == c.SenderId).Select(y => y.Username).FirstOrDefault(),
                    RecipientName = c.RecipientName,
                    RecipientAddress = c.RecipientAddress,
                    RecipientPhone = c.RecipientPhone,
                    HashCode = c.HashCode,
                    Status = c.Status
                })
                .ToList();

            return View(cargos);
        }

        public IActionResult LogOut()
        {
            // Oturumu temizle
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Home");
        }
        private string GenerateUniqueHashCode()
        {
            return Guid.NewGuid().ToString();
        }
    }


}
