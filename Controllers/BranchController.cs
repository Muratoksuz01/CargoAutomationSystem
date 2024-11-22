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


























        public IActionResult List()
        {
            return View();
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Home");
        }

        public IActionResult Setting()
        {
            return View();
        }
    }


}
