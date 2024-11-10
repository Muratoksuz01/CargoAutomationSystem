using CargoAutomationSystem.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// EF Core ile bağlantı dizesini ekliyoruz
builder.Services.AddDbContext<CargoContext>(options =>
    options.UseInMemoryDatabase("CargoDb")); // Bu satır veritabanını bellekte tutar (in-memory database)

// Servisleri ekleyin
builder.Services.AddScoped<CargoContext>(); // DbContext'i Scoped olarak ekliyoruz

builder.Services.AddControllersWithViews();

var app = builder.Build();  // Uygulamayı oluşturuyoruz

// Hata yönetimi ve güvenlik ayarları
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Diğer uygulama yapılandırmaları
app.UseRouting();
app.UseStaticFiles();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
