using CargoAutomationSystem.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Veritabanı bağlamı ekleniyor (InMemory Database kullanılıyor)
builder.Services.AddDbContext<CargoContext>(options =>
    options.UseInMemoryDatabase("CargoDb")); 

builder.Services.AddScoped<CargoContext>(); 

// Kimlik doğrulama ve çerez yapılandırması ekleniyor
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";       // Giriş yapılmadığında yönlendirilecek sayfa
        options.LogoutPath = "/Home/Logout";     // Çıkış işlemi sonrası yönlendirme
        options.AccessDeniedPath = "/Home/AccessDenied"; // Erişim engellendiğinde yönlendirme
    });

// Session için yapılandırma ekleniyor
builder.Services.AddDistributedMemoryCache();  // Session için gerekli bellek tabanlı önbellek
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session süresi, isteğe göre değiştirilebilir
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Diğer servisler
builder.Services.AddControllersWithViews();

var app = builder.Build();  

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Kimlik doğrulama ve yetkilendirme middleware'leri ekleniyor
app.UseAuthentication();
app.UseAuthorization();

// Session'ı kullanabilmek için UseSession'ı ekleyin
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=CorporateLogin}/{id?}");

app.Run();
