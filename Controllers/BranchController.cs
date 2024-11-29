using System.Security.Claims;
using CargoAutomationSystem.Entity;
using CargoAutomationSystem.Models.Corporate;
using CargoAutomationSystem.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace CargoAutomationSystem.Controllers
{
    public class BranchController : Controller

    {
        private readonly List<User> Users = DataSeeding.Users;
        private readonly List<Branch> Branches = DataSeeding.Branches;
        private readonly List<Cargo> Cargos = DataSeeding.Cargos;
        private readonly List<CargoProcess> CargoProcesses = DataSeeding.CargoProcesses;
        protected BranchInfoViewModel CurrentBranch => new BranchInfoViewModel
        {
            BranchId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0,
        };


        [HttpGet]
        public IActionResult EditCargo(string hashCode)
        {
            var cargo = Cargos.FirstOrDefault(c => c.HashCode == hashCode);
            if (cargo == null) return NotFound("Kargo bulunamadı.");

            var model = new EditCargoViewModel
            {
                HashCode = cargo.HashCode,
                Branches = Branches.Where(b => b.BranchId != CurrentBranch.BranchId).ToList() // Şube listesini gönderiyoruz
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditCargo(EditCargoViewModel model)
        {
            var branch = Branches.FirstOrDefault(b => b.BranchId == CurrentBranch.BranchId);
            var cargo = Cargos.FirstOrDefault(c => c.HashCode == model.HashCode);
            if (cargo == null) return NotFound("Kargo bulunamadı.");

            if (model.Status == "Teslim Edildi")
            {
                cargo.Status = "Teslim Edildi";

                // Teslim edildi işlem kaydı
                CargoProcesses.Add(new CargoProcess
                {
                    CargoProcessId = CargoProcesses.Count + 1,
                    CargoId = cargo.CargoId,
                    Process = "Teslim Edildi",
                    ProcessDate = DateTime.Now
                });
            }
            else if (model.Status == "Başka Şubeye Aktar" && model.NewBranchId.HasValue)
            {

                cargo.CurrentBranchId = model.NewBranchId.Value;
                cargo.Status = "Şubeye Aktarıldı";

                // Başka şubeye aktarma işlem kaydı
                CargoProcesses.Add(new CargoProcess
                {
                    CargoProcessId = CargoProcesses.Count + 1,
                    CargoId = cargo.CargoId,
                    Process = $"Başka Şubeye Aktarıldı (Şube ID: {model.NewBranchId.Value})",
                    ProcessDate = DateTime.Now
                });
                Branches.FirstOrDefault(b => b.BranchId == model.NewBranchId).Cargos.Add(cargo);
                branch.Cargos.Remove(cargo);
            }
            else
            {
                ModelState.AddModelError("", "Geçerli bir işlem veya şube seçmelisiniz.");
                return View(model);
            }

            return RedirectToAction("Index", "Branch");
        }


        public IActionResult RemoveCargo(string hashCode)
        {
            System.Console.WriteLine("burada");
            var branch = Branches.FirstOrDefault(b => b.BranchId == CurrentBranch.BranchId);
            var cargo =Cargos.FirstOrDefault(c => c.HashCode == hashCode);
            if(cargo==null)
            return RedirectToAction("List");
            if (branch.Cargos.Contains(cargo))
            {
                branch.Cargos.Remove(cargo); // Şubeden kargoyu kaldır
                System.Console.WriteLine($"Kargonun şube bağlantısı kaldırıldı. Hash kodu: {hashCode}");
            }
            return RedirectToAction("List");
        }


        public IActionResult List()
        {
            var branch=Branches.FirstOrDefault(br=>br.BranchId==CurrentBranch.BranchId);

            var cargos = branch.Cargos
                .Select(c => new BListViewModel
                {
                    CargoId = c.CargoId,
                    RecipientName = c.RecipientName,
                    RecipientAddress = c.RecipientAddress,
                    HashCode = c.HashCode,
                    Status = c.Status
                })
                .ToList();
            return View(cargos);
        }

        public IActionResult Details(string hashCode)
{
    // Kargo bilgilerini al
    var cargo = Cargos.SingleOrDefault(c => c.HashCode == hashCode);
    if (cargo == null)
    {
        return NotFound($"No cargo found with hash code: {hashCode}");
    }

    // Gönderici bilgilerini al
    var sender = Users.SingleOrDefault(u => u.UserId == cargo.SenderId);
    
    // Kargo süreçlerini al (örneğin, processlerin bulunduğu bir koleksiyon)
    var cargoProcesses = CargoProcesses.Where(cp => cp.CargoId == cargo.CargoId)
                                       .OrderBy(cp => cp.ProcessDate)
                                       .ToList();

    // Detay modelini oluştur
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
        CargoProcesses = cargoProcesses // Kargo süreçlerini ekle
    };

    System.Console.WriteLine($"Detail fetched for cargo hash code: {hashCode}");
    return View(detail);
}


        public IActionResult Index()
        {
            var branch = Branches.Where(i => i.BranchId == CurrentBranch.BranchId)
          .Select(c => new BIndexViewModel
          {
              BranchInfos = new BranchInfoViewModel
              {
                  BranchName = c.BranchName,
                  Email = c.Email,
                  Address = c.Address,
              },
              Cargos = Cargos
                  .Where(a => a.Status != "Tamamlandı") // Telefon numarasına göre filtreleme
                  .Select(a => new BIndexCargoViewModel
                  {

                      CargoId = a.CargoId,
                      ReceiverName = a.RecipientName,
                      Status = a.Status,
                      HashCode = a.HashCode,
                      ReceiverAddress = a.RecipientAddress
                  }).ToList() // Filtrelenmiş kargoların listesini oluştur
          }).FirstOrDefault();
            System.Console.WriteLine("branch index fonksiyonu çalıştı.");
            return View(branch);
        }


        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("CorporateLogin", "Home");
        }


        public IActionResult Settings()
        {
            System.Console.WriteLine(" branch setting de ");

            var branch = Branches.FirstOrDefault(i => i.BranchId == CurrentBranch.BranchId);
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
                //       TempData["Error"] = "Invalid username.";
                return View("Settings", model);
            }

            var branch = Branches.FirstOrDefault(i => i.BranchId == CurrentBranch.BranchId);
            branch.BranchName = model.UpdateUsername.Username;
            //                                                       sonra burada saveChanges gelecek

            //  TempData["Message"] = "Username updated successfully!";
            return RedirectToAction("Settings");
        }


        [HttpPost]
        public IActionResult UpdateInfo(BSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Settings", model);
            }
            var branch = Branches.FirstOrDefault(i => i.BranchId == CurrentBranch.BranchId);
            branch.Email = model.EditInfo.Email;
            branch.Address = model.EditInfo.Address;
            return RedirectToAction("Settings");
        }

        [HttpPost]
        public IActionResult UpdatePassword(BSettingsViewModel model)
        {
            var branch = Branches.FirstOrDefault(i => i.BranchId == CurrentBranch.BranchId);
            model.UpdateUsername = new BUpdateUsernameViewModel { Username = branch.BranchName };
            model.EditInfo = new BEditInfoViewModel { Email = branch.Email, Address = branch.Address };
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


                return View("Settings", model); // Ana modeli döndürüyoruz.
            }

            if (branch.Password != model.UpdatePassword.CurrentPassword)
            {
                ModelState.AddModelError("UpdatePassword.CurrentPassword", "Current password is incorrect.");
                return View("Settings", model);
            }

            branch.Password = model.UpdatePassword.NewPassword;
            //                                                       sonra burada saveChanges gelecek

            return RedirectToAction("Settings");
        }


    }


}
