using Microsoft.AspNetCore.Mvc;
using CargoAutomationSystem.Models.Home;
using CargoAutomationSystem.Models.Corporate;
using CargoAutomationSystem.Entity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using CargoAutomationSystem.Models.Users;
using Microsoft.AspNetCore.Authorization;
using CargoAutomationSystem.Data;

namespace CargoAutomationSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly CargoDbContext _context;

        public HomeController(CargoDbContext context)
        {
            _context = context; // Dependency injection ile context alınır
        }

        private User AuthenticateUser(string email, string password)
        {
            return _context.Users.SingleOrDefault(u => u.Email == email && u.Password == password);
        }

        private Branch AuthenticateBranch(string email, string password)
        {
            return _context.Branches.SingleOrDefault(u => u.Email == email && u.Password == password);
        }
        public IActionResult Index() => View();




        [HttpPost]
        public IActionResult GetCargoByHash(string hashCode)
        {
            var cargo = _context.Cargos.SingleOrDefault(c => c.HashCode == hashCode);
            if (cargo == null)
            {
                TempData["ErrorMessage"] = "kargo bulunamadı.";
                return RedirectToAction("NotFoundPage", "Home");
            }

            // Gönderici bilgilerini alıyoruz
            var sender = _context.Users.SingleOrDefault(u => u.UserId == cargo.SenderId);

            // Bulunduğu şube bilgisini alıyoruz
            var currentBranch = _context.Branches.SingleOrDefault(b => b.BranchId == cargo.CurrentBranchId);

            // Kargo süreçlerini alıyoruz
            var cargoProcesses = _context.CargoProcesses.Where(cp => cp.CargoId == cargo.CargoId)
                                                       .OrderBy(cp => cp.ProcessDate)
                                                       .ToList();

            // ViewModel'ı dolduruyoruz
            var detail = new DetailViewModel
            {
                CargoId = cargo.CargoId,
                HashCode = cargo.HashCode,
                Status = cargo.Status,
                CurrentBranch = currentBranch.BranchName,
                RecipientName = cargo.RecipientName,
                RecipientAddress = cargo.RecipientAddress,
                RecipientPhone = cargo.RecipientPhone,
                SenderId = sender.UserId,
                SenderUsername = sender.Username,
                SenderEmail = sender.Email,
                SenderAddress = sender.Address,
                SenderPhone = sender.Phone,
                CargoProcesses = cargoProcesses // Kargo süreçlerini buraya ekliyoruz
            };

            // View'e gönderiyoruz
            return View("Detail", detail);
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
                        new Claim(ClaimTypes.Role, "Branch")
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
            ModelState.AddModelError("", "formu kontrol ediniz ");
            return View(model);
        }

        [HttpPost]
        public IActionResult CorporateRegister(CorporateRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                _context.Branches.Add(new Branch { Email = model.Email, Password = model.Password, Address = model.Address, BranchName = model.UserName });
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public IActionResult CorporateRegister()
        {

            return View();
        }
        public IActionResult CorporateLogin()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Branch");
            }
            return View();
        }





        public IActionResult Register() => View();
        public IActionResult Login(string? returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "User");
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
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
            var tempUser = _context.Users.FirstOrDefault(u => u.Phone == model.PhoneNumber && u.IsTemporary);
            if (tempUser != null)
            {
                // TempUser'ı güncelle
                tempUser.Username = model.UserName;
                tempUser.Email = model.Email;
                tempUser.Password = model.Password; // Şifreyi hashleyerek saklayın
                tempUser.Address = model.Address;
                tempUser.IsTemporary = false; // Artık kalıcı kullanıcı oldu
                tempUser.ImageUrl = imageUrl; // Resim URL'sini güncelle

                _context.SaveChanges();
            }
            else
            {
                // Yeni kullanıcı oluştur
                var newUser = new User
                {
                    UserId = _context.Users.Count() + 1,
                    Username = model.UserName,
                    Email = model.Email,
                    Password = model.Password,
                    Address = model.Address,
                    Phone = model.PhoneNumber,
                    IsTemporary = false,
                    ImageUrl = imageUrl
                };
                _context.Users.Add(newUser);
                _context.SaveChanges();
            }

            return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
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
                        new Claim(ClaimTypes.Role, "User")
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "User"); // Başarılı girişten sonra yönlendirme
                }
                else
                {
                    ModelState.AddModelError("", "Geçersiz kullanıcı adı veya şifre");
                    return View(model); // Giriş başarısızsa tekrar giriş ekranına dön
                }
            }
            ModelState.AddModelError("", "formu kontrol ediniz ");
            return View(model);
        }







        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyUserName(string UserName)
        {
            // Kullanıcı adlarını kontrol et
            if (_context.Users.Any(u => u.Username == UserName))
            {
                return Json($"Kullanıcı adı '{UserName}' zaten alınmış.");
            }
            return Json(true); // Kullanıcı adı kullanılabilir
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyEmail(string Email)
        {
            // Email adreslerini kontrol et
            if (_context.Users.Any(u => u.Email == Email))
            {
                return Json($"Email '{Email}' zaten kullanılıyor.");
            }
            return Json(true); // Email kullanılabilir
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyPhone(string PhoneNumber)
        {
            // Sadece kalıcı kullanıcıları kontrol et
            if (_context.Users.Any(u => u.Phone == PhoneNumber && !u.IsTemporary))
            {
                return Json($"Telefon numarası '{PhoneNumber}' zaten kayıtlı.");
            }
            return Json(true); // Telefon numarası kullanılabilir
        }


        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult NotFoundPage()
        {
            return View();
        }

    }
}
