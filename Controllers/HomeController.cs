using Microsoft.AspNetCore.Mvc;
using CargoAutomationSystem.Models.Home;
using CargoAutomationSystem.Models.Corporate;
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

    private User AuthenticateUser(string email, string password)
    {
        return Users.SingleOrDefault(u => u.Email == email && u.Password == password);
   
    }
     private Branch AuthenticateBranch(string email, string password)
    {
        return Branches.SingleOrDefault(u => u.Email == email && u.Password == password);
   
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
            };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
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
    public async Task<IActionResult> CorporateLogin(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var branch = AuthenticateBranch(model.Email, model.Password);
            if (branch != null)
            {
                // Kullanıcı bilgileri doğruysa, claims oluşturuluyor
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, branch.BranchId.ToString()),  // Kullanıcı ID'si
            };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToAction("Index", "Branch"); // Başarılı girişten sonra yönlendirme
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
    public IActionResult CorporateRegister(CorporateRegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            Users.Add(new User { Email = model.Email, Password = model.Password });
            return RedirectToAction("Index");
        }

        return View(model);
    }

    public IActionResult CorporateRegister()=>View();
    public IActionResult Register()=>View();
    public IActionResult CorporateLogin()=>View();
    public IActionResult Login() => View();
    public IActionResult Index()=>View();
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


}
