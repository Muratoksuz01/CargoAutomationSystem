using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CargoAutomationSystem.Models;
using CargoAutomationSystem.Data;
using CargoAutomationSystem.Entity;
using System.Reflection.Metadata.Ecma335;

namespace CargoAutomationSystem.Controllers;

public class HomeController : Controller
{
    private readonly CargoContext _context;

    private readonly List<User> Users = DataSeeding.Users;
    private readonly List<Branch> Branches = DataSeeding.Branches;



    public IActionResult Index()
{     
        return View();
    }
    
 

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var entity = Users.FirstOrDefault(i => i.Email == model.Email && i.Password == model.Password);
            if (entity == null)
            {
                ViewBag.IsSuccess = "kullanıcı bulunamadı ";
                return View(model);
            }
            return RedirectToAction("Index","User");
        }

        return View(model);
    }
    [HttpPost]
    public IActionResult Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Simulating a user registration
            Users.Add(new User { Email = model.Email, Password = model.Password });
            return RedirectToAction("Index");
        }

        return View(model);
    }

    [HttpPost]
    public IActionResult CorporateLogin(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var entity = Branches.FirstOrDefault(i => i.Email == model.Email && i.Password == model.Password);
            if (entity == null)
            {   ViewBag.IsSuccess="basarısız kullanıcı girişimi";
                return View(model);
            }
        
            return RedirectToAction("Index","Branch");
        }

        return View(model);
    }

   public IActionResult Login() => View();

    [HttpPost]
    public IActionResult CorporateRegister(CorporateRegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            Users.Add(new User { Email = model.Email, Password = model.Password });
            return RedirectToAction("Index");
        }

        return View(model);
    }





    public IActionResult CorporateRegister()
    {
        return View();
    }
    public IActionResult Register()
    {
        return View();
    }
    public IActionResult CorporateLogin()
    {
        return View();
    }








}
