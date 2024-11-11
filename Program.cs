using CargoAutomationSystem.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CargoContext>(options =>
    options.UseInMemoryDatabase("CargoDb")); 

builder.Services.AddScoped<CargoContext>(); 

builder.Services.AddControllersWithViews();

var app = builder.Build();  

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRouting();
app.UseStaticFiles();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
