using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EnGee.Data;
using EnGee.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("EnGeeContextConnection") ?? throw new InvalidOperationException("Connection string 'EnGeeContextConnection' not found.");

builder.Services.AddDbContext<EnGeeContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<EnGeeUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<EnGeeContext>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "YourAuthCookieName";
        options.LoginPath = new PathString("/Home/LoginLayout");
        options.AccessDeniedPath = new PathString("/Home/AccessDenied");
    });

// Add services to the container.AddSession
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSession();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
