using System.Security.Claims;
using CargoAutomationSystem.Entity;
using CargoAutomationSystem.Models.Corporate;
using Microsoft.AspNetCore.Mvc;
namespace CargoAutomationSystem.ViewComponents
{
    public class BranchViewComponent : ViewComponent
    {
        private readonly List<User> Users = DataSeeding.Users;
        private readonly List<Branch> Branches = DataSeeding.Branches;
        public IViewComponentResult Invoke()
        {
             var claimsPrincipal = HttpContext.User;
            int BranchId = int.Parse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier));
            var BranchInfo = Branches
                .Where(u => u.BranchId == BranchId) 
                .Select(u => new BranchInfoViewModel
                {
                    BranchId = u.BranchId,
                    BranchName = u.BranchName,
                    Email = u.Email,
                    Address = u.Address,
                })
                .FirstOrDefault();
            return View(BranchInfo);
        }
    }
}

