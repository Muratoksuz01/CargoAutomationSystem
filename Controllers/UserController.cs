using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using CargoAutomationSystem.Entity;
using Microsoft.AspNetCore.Mvc.Rendering;
using CargoAutomationSystem.Models.Users;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CargoAutomationSystem.Controllers
{
    public class UserController : Controller
    {
        public UserController()
        {
            // CurrentUser
        }
        private readonly List<User> Users = DataSeeding.Users;
        private readonly List<Branch> Branches = DataSeeding.Branches;
        private readonly List<Cargo> Cargos = DataSeeding.Cargos;

        protected UserInfoViewModel CurrentUser => new UserInfoViewModel
        {
            UserId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0,
        };


        public IActionResult SendCargo()
        {
            var user = Users.FirstOrDefault(i => i.UserId == CurrentUser.UserId);
            var sendCargoModel = new SendCargoViewModel
            {
                SenderEmail = user.Email,
                SenderUsername = user.Username,
                SenderAddress = user.Address,
                SenderPhone = user.Phone
            };
            ViewBag.Branches = new SelectList(Branches, "BranchId", "BranchName");

            return View(sendCargoModel);
        }

        [HttpPost]
        public IActionResult SendCargo(SendCargoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Branches = new SelectList(Branches, "BranchId", "BranchName");
                return View(model);
            }
            // Yeni kargo oluştur
            var newCargo = new Cargo
            {
                CargoId = Cargos.Count + 1, // Yeni bir ID atama
                SenderId = model.SenderId,
                CurrentBranchId = model.SenderBranchId,
                RecipientName = model.RecipientName,
                RecipientAddress = model.RecipientAddress,
                RecipientPhone = model.RecipientPhone,
                HashCode = GenerateUniqueHashCode(),
                Status = "Taşımada"
            };

            // Kargoyu global listeye ekle
            Cargos.Add(newCargo);
            System.Console.WriteLine("kargo kargolar vertabanına eklendi");
            // Göndericinin listesine ekle
            var sender = Users.FirstOrDefault(u => u.UserId == model.SenderId);
            if (sender != null)
            {
                sender.Cargos.Add(newCargo);
            System.Console.WriteLine("kargo sender cargosa vertabanına eklendi");

            }

            // Şubenin listesine ekle
            var branch = Branches.FirstOrDefault(b => b.BranchId == model.SenderBranchId);
            if (branch != null)
            {
                branch.Cargos.Add(newCargo);
            System.Console.WriteLine("kargo branch cargosa vertabanına eklendi");

            }

            // Alıcının sisteme kayıtlı olup olmadığını kontrol et
            var recipient = Users.FirstOrDefault(u => u.Phone == model.RecipientPhone);
            if (recipient != null)
            {
                // Kayıtlıysa kargo alıcının listesine eklenir
                recipient.Cargos.Add(newCargo);
                System.Console.WriteLine("alıcı cargosa eklendi");
            }
            else
            {
                // Kayıtlı değilse, geçici kullanıcı olarak eklenir
                var tempUser = new User
                {
                    UserId = Users.Count + 1, // Yeni bir ID atama
                    Username = model.RecipientName,
                    Email = $"{model.RecipientPhone}@temporary.com", // Geçici bir email oluşturma
                    Password = "temporary", // Geçici bir şifre
                    Address = model.RecipientAddress,
                    Phone = model.RecipientPhone,
                    ImageUrl = null,
                    Cargos = new List<Cargo> { newCargo }
                };

                Users.Add(tempUser);
                System.Console.WriteLine($"Yeni geçici kullanıcı oluşturuldu: {tempUser.Username}");
            }

            System.Console.WriteLine("Kargo başarıyla gönderildi.");
            return RedirectToAction("Index");
        }

        
    

    public IActionResult Index()
        {
            // Aktif kullanıcıyı buluyoruz.
            var user = Users.FirstOrDefault(i => i.UserId == CurrentUser.UserId);

            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            // Kullanıcının devam eden kargolarını alıyoruz.
            var userCargos = user.Cargos
                .Where(c => c.Status != "Tamamlandı") // Tamamlanmamış kargolar.
                .Select(c => new IndexCargoViewModel
                {
                    CargoId = c.CargoId,
                    SenderName = Users.FirstOrDefault(u => u.UserId == c.SenderId)?.Username, // Gönderici adı.
                    Status = c.Status,
                    HashCode = c.HashCode
                }).ToList();

            // ViewModel'i oluşturuyoruz.
            var model = new IndexViewModel
            {
                UserInfos = new UserInfoViewModel
                {
                    Username = user.Username,
                    Email = user.Email,
                    Phone = user.Phone,
                    Address = user.Address,
                    ImageUrl = user.ImageUrl
                },
                Cargos = userCargos
            };

            return View(model);
        }


        public IActionResult RemoveCargo(string hashCode)
        {
            var user = Users.FirstOrDefault(i => i.UserId == CurrentUser.UserId);

            System.Console.WriteLine($"Gelen hash kodu: {hashCode}");
            var cargoToRemove = DataSeeding.Cargos.FirstOrDefault(c => c.HashCode == hashCode);
            if (cargoToRemove == null)
            {
                System.Console.WriteLine("Kargo bulunamadı.");
                return RedirectToAction("TrackCargo"); // Eğer kargo bulunamazsa geri yönlendir
            }


            if (user.Cargos.Contains(cargoToRemove))
            {
                user.Cargos.Remove(cargoToRemove);
                System.Console.WriteLine($"Kargo {cargoToRemove.HashCode} kullanıcıdan silindi: {user.Username}");
            }

            return RedirectToAction("TrackCargo");
        }


        public IActionResult Detail(string hashCode)
        {
            var cargo = Cargos.SingleOrDefault(c => c.HashCode == hashCode);
            if (cargo == null) return NotFound($"No cargo found with hash code: {hashCode}");
            var sender = Users.SingleOrDefault(u => u.UserId == cargo.SenderId);
            var currentBranch = Branches.SingleOrDefault(b => b.BranchId == cargo.CurrentBranchId);
            var detail = new DetailViewModel
            {
                CargoId = cargo.CargoId,
                HashCode = cargo.HashCode,
                Status = cargo.Status,
                CurrentBranch = currentBranch.BranchName,
                RecipientName = cargo.RecipientName,
                RecipientAddress = cargo.RecipientAddress,
                RecipientPhone = cargo.RecipientPhone,
                SenderId = sender.UserId, // Sender bilgisi
                SenderUsername = sender.Username,
                SenderEmail = sender.Email,
                SenderAddress = sender.Address,
                SenderPhone = sender.Phone
            };
            return View(detail);
        }

        public IActionResult TrackCargo()
        {
            var user = Users.FirstOrDefault(i => i.UserId == CurrentUser.UserId);

            var cargos = user.Cargos
                .Select(c => new TrackCargoViewModel
                {
                    CargoId = c.CargoId,
                    SenderName = Users.FirstOrDefault(u => u.UserId == c.SenderId)?.Username,
                    RecipientName = c.RecipientName,
                    RecipientAddress = c.RecipientAddress,
                    Status = c.Status,
                    HashCode = c.HashCode
                })
                .ToList();

            return View(cargos);
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Home");
        }

        private string GenerateUniqueHashCode()
        {
            return Guid.NewGuid().ToString();
        }

        public IActionResult Settings()
        {
            var user = Users.FirstOrDefault(i => i.UserId == CurrentUser.UserId);
            var model = new SettingsViewModel
            {
                UpdateUsername = new UpdateUsernameViewModel { Username = user.Username },
                EditInfo = new EditInfoViewModel { Email = user.Email, Phone = user.Phone, Address = user.Address },
                UpdatePassword = new UpdatePasswordViewModel(),
                UpdateImage = new UpdateImageViewModel { ImageFile = user.ImageUrl }
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult UpdatePassword(SettingsViewModel model)
        {
            var user = Users.FirstOrDefault(i => i.UserId == CurrentUser.UserId);
            model.UpdateUsername = new UpdateUsernameViewModel { Username = user.Username };
            model.EditInfo = new EditInfoViewModel { Email = user.Email, Phone = user.Phone, Address = user.Address };
            model.UpdateImage = new UpdateImageViewModel { ImageFile = user.ImageUrl };

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
                return View("Settings", model);
            }

            if (user.Password != model.UpdatePassword.CurrentPassword)
            {
                ModelState.AddModelError("UpdatePassword.CurrentPassword", "Current password is incorrect.");
                return View("Settings", model);
            }

            user.Password = model.UpdatePassword.NewPassword;
            // SaveChanges logic would go here

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
                // Save the uploaded file
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                user.ImageUrl = fileName;
                // SaveChanges logic here
            }

            return RedirectToAction("Settings");
        }

        [HttpPost]
        public IActionResult UpdateInfo(SettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Settings", model);
            }

            var user = Users.FirstOrDefault(i => i.UserId == CurrentUser.UserId);
            user.Email = model.EditInfo.Email;
            user.Address = model.EditInfo.Address;
            user.Phone = model.EditInfo.Phone;

            // SaveChanges logic here

            return RedirectToAction("Settings");
        }

        [HttpPost]
        public IActionResult RemoveImage()
        {
            var user = Users.FirstOrDefault(i => i.UserId == CurrentUser.UserId);
            user.ImageUrl = "";
            // SaveChanges logic here
            return RedirectToAction("Settings");
        }

        [HttpPost]
        public IActionResult UpdateUsername(SettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Settings", model);
            }

            var user = Users.FirstOrDefault(i => i.UserId == CurrentUser.UserId);
            user.Username = model.UpdateUsername.Username;
            // SaveChanges logic here

            return RedirectToAction("Settings");
        }



    }
}
