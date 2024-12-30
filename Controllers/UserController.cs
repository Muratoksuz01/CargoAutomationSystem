using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using CargoAutomationSystem.Entity;
using Microsoft.AspNetCore.Mvc.Rendering;
using CargoAutomationSystem.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.IO;
using CargoAutomationSystem.Data;
using System.Net.Http.Json;
using System.Xml;
using System.Text.Json;

namespace CargoAutomationSystem.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private readonly CargoDbContext _context;

        public UserController(CargoDbContext context)
        {
            _context = context;
        }

        // Access current user info
        protected UserInfoViewModel CurrentUser => new UserInfoViewModel
        {
            UserId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0,
        };

        // Method to send cargo


        // Handle sending cargo via POST
        [HttpPost]
        public IActionResult SendCargo(SendCargoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Branches = new SelectList(_context.Branches, "BranchId", "BranchName");
                return View(model);
            }

            // Create new cargo record
            var newCargo = new Cargo
            {   
                CargoId=_context.Cargos.Max(c=>c.CargoId)+1,
                SenderId = model.SenderId,
                CurrentBranchId = model.SenderBranchId,
                RecipientName = model.RecipientName,
                RecipientAddress = model.RecipientAddress,
                RecipientPhone = model.RecipientPhone,
                HashCode = GenerateUniqueHashCode(),
                Status = "Taşımada"
            };

            // Save the new cargo
            _context.Cargos.Add(newCargo);
            System.Console.WriteLine($"burdad eni kargı ıd:{newCargo.CargoId}");
            // Create a CargoProcess record for the cargo
            var newProcess = new CargoProcess
            {
                Cargo = newCargo,
                ProcessDate = DateTime.Now,
                Process = "Kargo kabul edildi"
            };
            _context.CargoProcesses.Add(newProcess);

            // Associate sender and branch
            var sender = _context.Users.Include(u => u.UserCargos).FirstOrDefault(u => u.UserId == model.SenderId);
            if (sender != null)
            {
                sender.UserCargos.Add(new UserCargo { Id=_context.UserCargos.Max(i=>i.Id)+1, UserId = sender.UserId, CargoId = newCargo.CargoId });
            }

            var branch = _context.Branches.FirstOrDefault(b => b.BranchId == model.SenderBranchId);
            if (branch != null)
            {
                branch.BranchCargos.Add(new BranchCargo {Id=_context.BranchCargos.Max(i=>i.Id)+1, BranchId = branch.BranchId, CargoId = newCargo.CargoId });
            }

            // Check if recipient exists, otherwise create a temporary user
            var recipient = _context.Users.Include(u => u.UserCargos).FirstOrDefault(u => u.Phone == model.RecipientPhone);
            if (recipient == null)
            {
                recipient = new User
                {
                    Username = model.RecipientName,
                    Email = $"{model.RecipientPhone}@temporary.com",
                    Password = "temporary",
                    Address = model.RecipientAddress,
                    Phone = model.RecipientPhone,
                    IsTemporary = true,
                    ImageUrl="nouser.png",
                    UserCargos = new List<UserCargo> { new UserCargo { CargoId = newCargo.CargoId } }
                };
                _context.Users.Add(recipient);
            }
            else
            {
                recipient.UserCargos.Add(new UserCargo { UserId = recipient.UserId, CargoId = newCargo.CargoId });
            }

            newCargo.RecipientId = recipient.UserId;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }











        // Index method for displaying user's cargo
        public IActionResult Index()
        {
             var user = _context.Users
                .Include(u => u.UserCargos)
                .ThenInclude(uc => uc.Cargo)
                .ThenInclude(c => c.Sender)
                .FirstOrDefault(i => i.UserId == CurrentUser.UserId);
          
            if (user == null) return NotFound("User not found.");

        var userCargos=user.UserCargos
        .Where(u=>u.Cargo.Status!="Teslim Edildi")
        .Select(uc=>new IndexCargoViewModel{
            CargoId=uc.Cargo.CargoId,
            SenderName=uc.Cargo.Sender.Username,
            ReceiverName=uc.Cargo.RecipientName,
            Status=uc.Cargo.Status,
            HashCode=uc.Cargo.HashCode
        }).ToList();


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


        // Track cargo method
        public IActionResult TrackCargo()
        {
            var user = _context.Users
                .Include(u => u.UserCargos)
                .ThenInclude(uc => uc.Cargo)
                .ThenInclude(c => c.Sender)
                .FirstOrDefault(i => i.UserId == CurrentUser.UserId);
          
            if (user == null)
                return NotFound("User not found.");

            // Cargos'u UserCargos üzerinden al
            var cargos = user.UserCargos.Select(uc => new TrackCargoViewModel //            null geliyor 
            {
                CargoId = uc.Cargo.CargoId,
                SenderName = uc.Cargo.Sender.Username, //nulll geliyor 
                RecipientName = uc.Cargo.RecipientName,
                RecipientAddress = uc.Cargo.RecipientAddress,
                Status = uc.Cargo.Status,
                HashCode = uc.Cargo.HashCode
            }).ToList();

            return View(cargos);
        }


        // Remove cargo method
        public IActionResult RemoveCargo(string hashCode)
        {
            var cargoToRemove = _context.Cargos.FirstOrDefault(c => c.HashCode == hashCode);
            if (cargoToRemove == null)
                return RedirectToAction("TrackCargo");

            var user = _context.Users
                .Include(u => u.UserCargos) // UserCargos'u dahil et
                .ThenInclude(uc => uc.Cargo) // UserCargo'dan Cargo'yu dahil et
                .FirstOrDefault(i => i.UserId == CurrentUser.UserId);

            if (user == null)
                return RedirectToAction("TrackCargo");

            // UserCargos koleksiyonundan ilgili Cargo'yu bul ve sil
            var userCargoToRemove = user.UserCargos.FirstOrDefault(uc => uc.CargoId == cargoToRemove.CargoId);
            if (userCargoToRemove != null)
            {
                _context.UserCargos.Remove(userCargoToRemove); // UserCargo ilişkisinden sil
                _context.SaveChanges(); // Değişiklikleri kaydet
            }

            return RedirectToAction("TrackCargo");
        }









        public IActionResult SendCargo()
        {
            var user = _context.Users.FirstOrDefault(i => i.UserId == CurrentUser.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var sendCargoModel = new SendCargoViewModel
            {
                SenderId = user.UserId,
                SenderEmail = user.Email,
                SenderUsername = user.Username,
                SenderAddress = user.Address,
                SenderPhone = user.Phone
            };

            ViewBag.Branches = new SelectList(_context.Branches, "BranchId", "BranchName");

            return View(sendCargoModel);
        }
        // Cargo detail view
        public IActionResult Detail(string hashCode)
        {
            var cargo = _context.Cargos
                .Include(c => c.Sender)
                .Include(c => c.CargoProcesses)
                .FirstOrDefault(c => c.HashCode == hashCode);

            if (cargo == null) return NotFound($"No cargo found with hash code: {hashCode}");

            var currentBranch = _context.Branches.FirstOrDefault(b => b.BranchId == cargo.CurrentBranchId);
            var detail = new DetailViewModel
            {
                CargoId = cargo.CargoId,
                HashCode = cargo.HashCode,
                Status = cargo.Status,
                CurrentBranch = currentBranch?.BranchName,
                RecipientName = cargo.RecipientName,
                RecipientAddress = cargo.RecipientAddress,
                RecipientPhone = cargo.RecipientPhone,
                SenderId = cargo.Sender.UserId,
                SenderUsername = cargo.Sender.Username,
                SenderEmail = cargo.Sender.Email,
                SenderAddress = cargo.Sender.Address,
                SenderPhone = cargo.Sender.Phone,
                CargoProcesses = cargo.CargoProcesses.OrderBy(cp => cp.ProcessDate).ToList()
            };

            return View(detail);
        }


        // Logout method
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Home");
        }

        // Helper to generate unique hash code
        private string GenerateUniqueHashCode()
        {
            return Guid.NewGuid().ToString();
        }

        // Settings page for user profile
        public IActionResult Settings()
        {
            var user = _context.Users.FirstOrDefault(i => i.UserId == CurrentUser.UserId);
            if (user == null) return NotFound("User not found.");

            var model = new SettingsViewModel
            {
                UpdateUsername = new UpdateUsernameViewModel { Username = user.Username },
                EditInfo = new EditInfoViewModel { Email = user.Email, Phone = user.Phone, Address = user.Address },
                UpdatePassword = new UpdatePasswordViewModel(),
                UpdateImage = new UpdateImageViewModel { ImageFile = user.ImageUrl }
            };

            return View(model);
        }

        // Update password
        [HttpPost]
        public IActionResult UpdatePassword(SettingsViewModel model)
        {
            var user = _context.Users.FirstOrDefault(i => i.UserId == CurrentUser.UserId);
            if (user == null) return NotFound("User not found.");

            model.UpdateUsername = new UpdateUsernameViewModel { Username = user.Username };
            model.EditInfo = new EditInfoViewModel { Email = user.Email, Phone = user.Phone, Address = user.Address };
            model.UpdateImage = new UpdateImageViewModel { ImageFile = user.ImageUrl };

            if (!ModelState.IsValid || user.Password != model.UpdatePassword.CurrentPassword)
            {
                ModelState.AddModelError("UpdatePassword.CurrentPassword", "Current password is incorrect.");
                return View("Settings", model);
            }

            user.Password = model.UpdatePassword.NewPassword;
            _context.SaveChanges();

            return RedirectToAction("Settings");
        }

        // Update profile image
        [HttpPost]
        public IActionResult UpdateImage(IFormFile file)
        {
            var user = _context.Users.FirstOrDefault(i => i.UserId == CurrentUser.UserId);
            if (user == null) return NotFound("User not found.");

            if (file != null)
            {
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
                if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                user.ImageUrl = fileName;
                _context.SaveChanges();
            }

            return RedirectToAction("Settings");
        }

        // Update user information
        [HttpPost]
        public IActionResult UpdateInfo(SettingsViewModel model)
        {
            if (!ModelState.IsValid) return View("Settings", model);

            var user = _context.Users.FirstOrDefault(i => i.UserId == CurrentUser.UserId);
            if (user == null) return NotFound("User not found.");

            user.Email = model.EditInfo.Email;
            user.Address = model.EditInfo.Address;
            user.Phone = model.EditInfo.Phone;

            _context.SaveChanges();

            return RedirectToAction("Settings");
        }

        // Remove profile image
        [HttpPost]
        public IActionResult RemoveImage()
        {
            var user = _context.Users.FirstOrDefault(i => i.UserId == CurrentUser.UserId);
            if (user == null) return NotFound("User not found.");

            user.ImageUrl = "nouser.png";
            _context.SaveChanges();
            return RedirectToAction("Settings");
        }

        // Update username
        [HttpPost]
        public IActionResult UpdateUsername(SettingsViewModel model)
        {
            var user = _context.Users.FirstOrDefault(i => i.UserId == CurrentUser.UserId);
            if (user == null) return NotFound("User not found.");

            model.EditInfo = new EditInfoViewModel { Email = user.Email, Phone = user.Phone, Address = user.Address };
            model.UpdateImage = new UpdateImageViewModel { ImageFile = user.ImageUrl };

            if (!ModelState.IsValid) return View("Settings", model);

            user.Username = model.UpdateUsername.Username;
            _context.SaveChanges();

            return RedirectToAction("Settings");
        }
    }
}
