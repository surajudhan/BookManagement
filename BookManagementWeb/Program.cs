using BookManagement.Business.Services;
using BookManagement.Business.Services.IServices;
using BookManagement.DataAccess.Data;
using BookManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. 
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductServices, ProductServices>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IApplicationUserServices, ApplicationUserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.ConfigureApplicationCookie(Options =>
{
    Options.LoginPath = "/Identity/Account/Login";
    Options.LogoutPath = "/Identity/Account/Logout";
    Options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    Options.ExpireTimeSpan = TimeSpan.FromDays(30);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); // always mention before the authorization // user is valid
app.UseAuthorization();  //authorize to which accesses are allowed to user

app.MapStaticAssets();

app.MapControllerRoute(name: "MyArea",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    ).WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "Customer" })
    .WithStaticAssets();


app.Run();
