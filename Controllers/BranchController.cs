using System.Security.Claims;
using CargoAutomationSystem.Entity;
using CargoAutomationSystem.Models.Corporate;
using CargoAutomationSystem.Models.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using CargoAutomationSystem.Data;

namespace CargoAutomationSystem.Controllers
{
    [Authorize(Roles = "Branch")]
    public class BranchController : Controller
    {
        protected BranchInfoViewModel CurrentBranch => new BranchInfoViewModel
        {
            BranchId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0,
        };

        private readonly CargoDbContext _context;

        public BranchController(CargoDbContext context)
        {
            _context = context;  // Dependency injection ile context alınır
        }

        [HttpGet]
        public IActionResult EditCargo(string hashCode)
        {
            var cargo = _context.Cargos.FirstOrDefault(c => c.HashCode == hashCode);
            if (cargo == null) return NotFound("Kargo bulunamadı.");

            var model = new EditCargoViewModel
            {
                HashCode = cargo.HashCode,
                Branches = _context.Branches
                                    .Where(b => b.BranchId != CurrentBranch.BranchId)
                                    .ToList() // Veritabanındaki şubeler
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditCargo(EditCargoViewModel model)
        {
            var branch = _context.Branches.FirstOrDefault(b => b.BranchId == CurrentBranch.BranchId);
            var cargo = _context.Cargos.FirstOrDefault(c => c.HashCode == model.HashCode);
            if (cargo == null) return NotFound("Kargo bulunamadı.");

            if (model.Status == "Teslim Edildi")
            {
                cargo.Status = "Teslim Edildi";

                // Teslim edildi işlem kaydı
                _context.CargoProcesses.Add(new CargoProcess
                {
                    CargoId = cargo.CargoId,
                    Process = "Teslim Edildi",
                    ProcessDate = DateTime.Now
                });
            }
            else if (model.Status == "Başka Şubeye Aktar" && model.NewBranchId.HasValue)
            {
                var newBranch = _context.Branches.Include(b => b.BranchCargos).FirstOrDefault(b => b.BranchId == model.NewBranchId.Value);
                if (newBranch != null)
                {
                    // Kargoyu mevcut şubeden çıkar
                    var branchCargo = _context.BranchCargos.FirstOrDefault(bc => bc.BranchId == branch.BranchId && bc.CargoId == cargo.CargoId);
                    if (branchCargo != null)
                    {
                        _context.BranchCargos.Remove(branchCargo); // Şubeden çıkarıyoruz
                    }

                    // Kargoyu yeni şubeye ekle
                    _context.BranchCargos.Add(new BranchCargo
                    {
                        BranchId = newBranch.BranchId,
                        CargoId = cargo.CargoId
                    });

                    cargo.Status = model.Status;

                    // Başka şubeye aktarma işlem kaydı
                    _context.CargoProcesses.Add(new CargoProcess
                    {
                        CargoId = cargo.CargoId,
                        Process = $"Başka Şubeye Aktarıldı (Şube: {newBranch.BranchName})",
                        ProcessDate = DateTime.Now
                    });
                }
            }
            else
            {
                ModelState.AddModelError("", "Geçerli bir işlem veya şube seçmelisiniz.");
                return View(model);
            }

            _context.SaveChanges(); // Değişiklikleri kaydediyoruz
            return RedirectToAction("Index", "Branch");
        }

        public IActionResult RemoveCargo(string hashCode)
        {
            var branch = _context.Branches.FirstOrDefault(b => b.BranchId == CurrentBranch.BranchId);
            var cargo = _context.Cargos.FirstOrDefault(c => c.HashCode == hashCode);
            if (cargo == null) return RedirectToAction("List");

            var branchCargo = _context.BranchCargos.FirstOrDefault(bc => bc.BranchId == branch.BranchId && bc.CargoId == cargo.CargoId);
            if (branchCargo != null)
            {
                _context.BranchCargos.Remove(branchCargo); // Kargoyu şubeden çıkar
            }

            _context.SaveChanges(); // Değişiklikleri kaydet
            return RedirectToAction("List");
        }

        public IActionResult List()
        {
            var branch = _context.Branches
                .Include(b => b.BranchCargos)
                    .ThenInclude(bc => bc.Cargo)
                .FirstOrDefault(br => br.BranchId == CurrentBranch.BranchId);

            if (branch == null) return NotFound("Şube bulunamadı.");

            var cargos = branch.BranchCargos.Select(bc => bc.Cargo).ToList(); // Şubeye ait kargoları al

            var cargoViewModels = cargos
                .Select(c => new BListViewModel
                {
                    CargoId = c.CargoId,
                    RecipientName = c.RecipientName,
                    RecipientAddress = c.RecipientAddress,
                    HashCode = c.HashCode,
                    Status = c.Status
                })
                .ToList();

            return View(cargoViewModels);
        }

        public IActionResult Details(string hashCode)
        {
            var cargo = _context.Cargos.SingleOrDefault(c => c.HashCode == hashCode);
            if (cargo == null)
            {
                return NotFound($"No cargo found with hash code: {hashCode}");
            }

            var sender = _context.Users.SingleOrDefault(u => u.UserId == cargo.SenderId);

            var cargoProcesses = _context.CargoProcesses.Where(cp => cp.CargoId == cargo.CargoId)
                                                       .OrderBy(cp => cp.ProcessDate)
                                                       .ToList();

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
                SenderPhone = sender?.Phone,
                CargoProcesses = cargoProcesses
            };

            return View(detail);
        }

        public IActionResult Index()
        {
            var branch = _context.Branches
                .Where(i => i.BranchId == CurrentBranch.BranchId)
                .Select(c => new BIndexViewModel
                {
                    BranchInfos = new BranchInfoViewModel
                    {
                        BranchName = c.BranchName,
                        Email = c.Email,
                        Address = c.Address,
                    },
                    Cargos = c.BranchCargos
                        .Where(bc => bc.Cargo.Status != "Teslim Edildi" &&
                                     _context.CargoProcesses.Any(cp => cp.CargoId == bc.CargoId &&
                                        (cp.Process.StartsWith("Başka Şubeye") || cp.Process.StartsWith("Kargo kabul"))))
                        .Select(bc => new BIndexCargoViewModel
                        {
                            CargoId = bc.Cargo.CargoId,
                            ReceiverName = bc.Cargo.RecipientName,
                            Status = bc.Cargo.Status,
                            HashCode = bc.Cargo.HashCode,
                            ReceiverAddress = bc.Cargo.RecipientAddress
                        }).ToList()
                }).FirstOrDefault();

            return View(branch);
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("CorporateLogin", "Home");
        }

        public IActionResult Settings()
        {
            var branch = _context.Branches.FirstOrDefault(i => i.BranchId == CurrentBranch.BranchId);
            if (branch == null) return NotFound("Şube bulunamadı.");

            var model = new BSettingsViewModel
            {
                UpdateUsername = new BUpdateUsernameViewModel { Username = branch.BranchName },
                EditInfo = new BEditInfoViewModel { Email = branch.Email, Address = branch.Address },
                UpdatePassword = new BUpdatePasswordViewModel(),
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateUsername(BSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Settings", model);
            }

            var branch = _context.Branches.FirstOrDefault(i => i.BranchId == CurrentBranch.BranchId);
            if (branch != null)
            {
                branch.BranchName = model.UpdateUsername.Username;
                _context.SaveChanges(); // Değişiklikleri kaydediyoruz
            }

            return RedirectToAction("Settings");
        }

        [HttpPost]
        public IActionResult UpdateInfo(BSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Settings", model);
            }

            var branch = _context.Branches.FirstOrDefault(i => i.BranchId == CurrentBranch.BranchId);
            if (branch != null)
            {
                branch.Email = model.EditInfo.Email;
                branch.Address = model.EditInfo.Address;
                _context.SaveChanges(); // Değişiklikleri kaydediyoruz
            }

            return RedirectToAction("Settings");
        }

        [HttpPost]
        public IActionResult UpdatePassword(BSettingsViewModel model)
        {
            var branch = _context.Branches.FirstOrDefault(i => i.BranchId == CurrentBranch.BranchId);
            model.UpdateUsername = new BUpdateUsernameViewModel { Username = branch?.BranchName };
            model.EditInfo = new BEditInfoViewModel { Email = branch?.Email, Address = branch?.Address };

            if (!ModelState.IsValid)
            {
                return View("Settings", model); // Ana modeli döndürüyoruz.
            }

            if (branch?.Password != model.UpdatePassword.CurrentPassword)
            {
                ModelState.AddModelError("UpdatePassword.CurrentPassword", "Current password is incorrect.");
                return View("Settings", model);
            }

            branch.Password = model.UpdatePassword.NewPassword;
            _context.SaveChanges(); // Değişiklikleri kaydediyoruz

            return RedirectToAction("Settings");
        }
    }
}
