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
        protected BranchInfoViewModel CurrentBranch => new BranchInfoViewModel
        {
            BranchId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0,
        };
        

public IActionResult RemoveCargo(string hashCode)
{
    foreach (var branch in DataSeeding.Branches)
    {
        var cargo = branch.Cargos.FirstOrDefault(c => c.HashCode == hashCode);
        if (cargo != null)
        {
            branch.Cargos.Remove(cargo); // Şubeden kargoyu kaldır
            System.Console.WriteLine($"Kargonun şube bağlantısı kaldırıldı. Hash kodu: {hashCode}");
            break;
        }
    }

    return RedirectToAction("List");
}















        public IActionResult List()
        {
            var cargos = Cargos
                .Where(c => c.CurrentBranchId == CurrentBranch.BranchId)
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
