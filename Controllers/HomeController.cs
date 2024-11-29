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
       private User AuthenticateUser(string email, string password)
    {
        return Users.SingleOrDefault(u => u.Email == email && u.Password == password);

    }
    private Branch AuthenticateBranch(string email, string password)
    {
        return Branches.SingleOrDefault(u => u.Email == email && u.Password == password);

    }
   
    // private readonly List<User> Users;
    // private readonly List<Branch> Branches;

    private readonly List<User> Users = DataSeeding.Users;
        private readonly List<Branch> Branches = DataSeeding.Branches;

    // public HomeController()
    // {
    //     // DataSeeding'den Users listesinin null olup olmadığını kontrol ediyoruz
    //     if (DataSeeding.Users == null)
    //     {
    //         Console.WriteLine("DataSeeding.Users listesi NULL!");
    //         Users = new List<User>(); // Eğer null ise boş bir liste başlatıyoruz
    //     }
    //     else
    //     {
    //         Console.WriteLine("DataSeeding.Users listesi yüklendi.");
    //         Users = DataSeeding.Users.ToList(); // List'e çeviriyoruz
    //     }

    //     // Aynı kontrolü Branches için de yapabiliriz
    //     if (DataSeeding.Branches == null)
    //     {
    //         Console.WriteLine("DataSeeding.Branches listesi NULL!");
    //         Branches = new List<Branch>(); // Eğer null ise boş bir liste başlatıyoruz
    //     }
    //     else
    //     {
    //         Console.WriteLine("DataSeeding.Branches listesi yüklendi.");
    //         Branches = DataSeeding.Branches.ToList(); // List'e çeviriyoruz
    //     }
    // }




[HttpPost]
    public IActionResult Register(RegisterViewModel model, IFormFile? file)
{
    if (!ModelState.IsValid)
    {
        System.Console.WriteLine("Kayıt sırasında hata oluştu");
        foreach (var key in ModelState.Keys)
        {
            var errors = ModelState[key].Errors;
            foreach (var error in errors)
            {
                Console.WriteLine($"Field: {key}, Error: {error.ErrorMessage}");
            }
        }
        return View(model);
    }

    string imageUrl;

    // Eğer dosya gönderilmişse
    if (file != null && file.Length > 0)
    {
        // Dosya adı için benzersiz bir isim oluştur
        var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine("wwwroot/img", uniqueFileName);

        // Dosyayı belirtilen klasöre kaydet
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            file.CopyTo(stream);
        }

        imageUrl = $"{uniqueFileName}"; // URL için dosya yolu
    }
    else
    {
        imageUrl = "nouser.png";
    }

    // Kullanıcıyı bul (geçici bir kullanıcı olup olmadığını kontrol et)
    var tempUser = Users.FirstOrDefault(u => u.Phone == model.PhoneNumber && u.IsTemporary);
    if (tempUser != null)
    {
        // TempUser'ı güncelle
        tempUser.Username = model.UserName;
        tempUser.Email = model.Email;
        tempUser.Password = model.Password; // Şifreyi hashleyerek saklayın
        tempUser.Address = model.Address;
        tempUser.IsTemporary = false; // Artık kalıcı kullanıcı oldu
        tempUser.ImageUrl = imageUrl; // Resim URL'sini güncelle

        System.Console.WriteLine($"TempUser güncellendi: {tempUser.Username}");
    }
    else
    {
        // Yeni kullanıcı oluştur
        var newUser = new User
        {
            UserId = Users.Count + 1,
            Username = model.UserName,
            Email = model.Email,
            Password = model.Password, 
            Address = model.Address,
            Phone = model.PhoneNumber,
            IsTemporary = false,
            ImageUrl = imageUrl 
        };
        Users.Add(newUser);
        System.Console.WriteLine($"Yeni kullanıcı oluşturuldu: {newUser.Username}");
        System.Console.WriteLine($"Yeni email oluşturuldu: {newUser.Email}");
        System.Console.WriteLine($"Yeni password oluşturuldu: {newUser.Password}");
    }

    return RedirectToAction("Login", "Home");
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

    public IActionResult CorporateRegister() => View();
    public IActionResult Register() => View();
    public IActionResult CorporateLogin() => View();
    public IActionResult Login() => View();
    public IActionResult Index() => View();
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
        // Sadece kalıcı kullanıcıları kontrol et
        if (Users.Any(u => u.Phone == PhoneNumber && !u.IsTemporary))
        {
            return Json($"Telefon numarası '{PhoneNumber}' zaten kayıtlı.");
        }
        return Json(true); // Telefon numarası kullanılabilir
    }



}
