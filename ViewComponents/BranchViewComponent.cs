using System.Security.Claims;
using CargoAutomationSystem.Data;
using CargoAutomationSystem.Entity;
using CargoAutomationSystem.Models.Corporate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CargoAutomationSystem.ViewComponents
{
    public class BranchViewComponent : ViewComponent
    {
        private readonly CargoDbContext _context;

        public BranchViewComponent(CargoDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var claimsPrincipal = HttpContext.User;
            int branchId = int.Parse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier));

            // Retrieve the branch information from the database
            var branchInfo = _context.Branches
                .Where(b => b.BranchId == branchId)
                .Select(b => new BranchInfoViewModel
                {
                    BranchId = b.BranchId,
                    BranchName = b.BranchName,
                    Email = b.Email,
                    Address = b.Address
                })
                .FirstOrDefault();

            // Return the branch info to the view
            return View(branchInfo);
        }
    }
}
