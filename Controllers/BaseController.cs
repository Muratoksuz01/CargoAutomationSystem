using CargoAutomationSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class BaseController : Controller
{
    protected UserInfoViewModel CurrentUser => new UserInfoViewModel
    {
        UserId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0,
        Username = User.FindFirstValue(ClaimTypes.Name),
        Email = User.FindFirstValue(ClaimTypes.Email),
        Address = User.FindFirstValue("Address"),
        Phone = User.FindFirstValue(ClaimTypes.MobilePhone),
        ImageUrl = User.FindFirstValue("ImageUrl")
    };
}
