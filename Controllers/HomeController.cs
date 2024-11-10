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


    public class TempMessage
    {
        public string Message { get; set; }  // For the message text
        public bool IsSuccess { get; set; }  // To indicate success or failure
    }
    public IActionResult Index()
    {
        if (TempData["IsSuccess"] != null)
        {
            var tempMessage = TempData["IsSuccess"] as TempMessage;
            if (tempMessage != null)
            {
                ViewData["IsSuccess"] = tempMessage;
            }
        }
        return View();
    }
    public IActionResult Login() => View();


    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var entity = Users.FirstOrDefault(i => i.Email == model.Email && i.Password == model.Password);
            if (entity == null)
            {
                TempData["IsSuccess"] = new TempMessage
                {
                    Message = "Kullanıcı adı veya şifre hatalı!",
                    IsSuccess = false
                };
                return View(model);
            }

            TempData["IsSuccess"] = new TempMessage
            {
                Message = "Giriş başarılı!",
                IsSuccess = true
            };
            return RedirectToAction("Index");
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
            TempData["IsSuccess"] = new { message = "Kayıt başarılı!", IsSuccess = true };
            return RedirectToAction("Index");
        }

        TempData["IsSuccess"] = new { message = "Kayıt işlemi başarısız!", IsSuccess = false };
        return View(model);
    }

    [HttpPost]
    public IActionResult CorporateLogin(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var entity = Branches.FirstOrDefault(i => i.Email == model.Email && i.Password == model.Password);
            if (entity == null)
            {
                TempData["IsSuccess"] = new { message = "Kurumsal giriş hatalı!", IsSuccess = false };
                return View(model);
            }

            TempData["IsSuccess"] = new { message = "Kurumsal giriş başarılı!", IsSuccess = true };
            return RedirectToAction("Index");
        }

        return View(model);
    }


    [HttpPost]
    public IActionResult CorporateRegister(CorporateRegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var entity = Users.FirstOrDefault(i => i.Email == model.Email && i.Password == model.Password);

            if (entity == null)
            {
                TempData["IsSuccess"] = new { message = "Kullanıcı adı veya şifre hatalı!", IsSuccess = false };
            }
            else
            {
                TempData["IsSuccess"] = new { message = "Kurumsal kayıt başarılı!", IsSuccess = true };
            }

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
