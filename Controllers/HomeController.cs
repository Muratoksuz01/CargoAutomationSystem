using Microsoft.AspNetCore.Mvc;
using CargoAutomationSystem.Models;
using CargoAutomationSystem.Data;
using CargoAutomationSystem.Entity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
    public IActionResult Register(RegisterViewModel model, IFormFile? file)
    {
        ModelState.Remove("ImageUrl");
        if (ModelState.IsValid)
        {
            string imageUrl = null;

            // Eğer dosya yüklenmişse işlem yap
            if (file != null && file.Length > 0)
            {
                // Dosya ismini oluştur (benzersiz yapmak için Guid kullanabilirsiniz)
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                // Dosya kaydedileceği dizin (örneğin "wwwroot/images/")
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img");

                // Dizin yoksa oluştur
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Tam dosya yolu
                var filePath = Path.Combine(uploadPath, fileName);

                // Dosyayı fiziksel olarak kaydet
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // Kaydedilen dosyanın URL'ini oluştur
                imageUrl = fileName;
            }

            // Kullanıcıyı kaydet
            Users.Add(new User
            {
                Email = model.Email,
                Password = model.Password,
                Username = model.UserName,
                Phone = model.PhoneNumber,
                Address = model.Address,
                ImageUrl = imageUrl // Kaydedilen resim URL'sini atıyoruz
            });

            return RedirectToAction("Index");
        }
        foreach (var state in ModelState)
        {
            string key = state.Key; // Alan adı
            foreach (var error in state.Value.Errors)
            {
                Console.WriteLine($"Field: {key}, Error: {error.ErrorMessage}");
            }
        }
        return View(model);
    }

    [AcceptVerbs("GET", "POST")]
    public IActionResult VerifyUserName(string UserName)
    {
        // Kullanıcı adlarını kontrol et
        if (Users.Any(u => u.Username == UserName))
        {
            return Json($"Kullanıcı adı '{UserName}' zaten alınmış.");
        }
        return Json(true); // Kullanıcı adı kullanılabilir
    }

    [AcceptVerbs("GET", "POST")]
    public IActionResult VerifyEmail(string Email)
    {
        // Email adreslerini kontrol et
        if (Users.Any(u => u.Email == Email))
        {
            return Json($"Email '{Email}' zaten kullanılıyor.");
        }
        return Json(true); // Email kullanılabilir
    }

    [AcceptVerbs("GET", "POST")]
    public IActionResult VerifyPhone(string PhoneNumber)
    {  
        // Telefon numaralarını kontrol et
        if (Users.Any(u => u.Phone == PhoneNumber))
        {
            return Json($"Telefon numarası '{PhoneNumber}' zaten kayıtlı.");
        }
        return Json(true); // Telefon numarası kullanılabilir
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
                // new Claim(ClaimTypes.Name, user.Username),                      // Kullanıcı adı
                // new Claim(ClaimTypes.Email, user.Email),                        // Kullanıcı email
                // new Claim("Address", user.Address ?? ""),
                // new Claim(ClaimTypes.MobilePhone, user.Phone ?? ""),
                // new Claim("ImageUrl", user.ImageUrl ?? ""),
                // new Claim("Password", user.Password)


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
