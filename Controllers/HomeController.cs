using Microsoft.AspNetCore.Mvc;
using CargoAutomationSystem.Models;
using CargoAutomationSystem.Data;
using CargoAutomationSystem.Entity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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
    private User AuthenticateUser(string email, string password)
    {
         return Users.SingleOrDefault(u => u.Email == email && u.Password == password);
        // return new User
        // {
        //     UserId = 1,
        //     Username = "JohnDoe",
        //     Email = "johndoe@example.com",
        //     Address = "123 Main St, City"
        // };
    }


    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = AuthenticateUser(model.Email, model.Password);
            if (user != null)
            {
                // Kullanıcı bilgileri doğruysa, claims oluşturuluyor
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),  // Kullanıcı ID'si
                new Claim(ClaimTypes.Name, user.Username),                      // Kullanıcı adı
                new Claim(ClaimTypes.Email, user.Email),                        // Kullanıcı email
                new Claim("Address", user.Address ?? ""),
                new Claim(ClaimTypes.MobilePhone, user.Phone ?? ""),
                new Claim("ImageUrl", user.ImageUrl ?? "")

            };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Kimlik doğrulama cookies'i oluşturuluyor
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "User"); // Başarılı girişten sonra yönlendirme
            }

            else
            {
                ModelState.AddModelError("", "Geçersiz kullanıcı adı veya şifre");
                return View(model); // Giriş başarısızsa tekrar giriş ekranına dön
            }
        }

        ModelState.AddModelError("", "formu kontorl ediniz ");
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
            {
                ViewBag.IsSuccess = "basarısız kullanıcı girişimi";
                return View(model);
            }

            return RedirectToAction("Index", "Branch");
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
