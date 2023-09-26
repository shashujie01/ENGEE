using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EnGee.Data;
using EnGee.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using EnGee.CollectService;
using EnGee.Models;
using EnGee.Services.EmailService;
using EnGee.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using EnGee.Hubs;
using EnGee.Repositories;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("EnGeeContextConnection") ?? throw new InvalidOperationException("Connection string 'EnGeeContextConnection' not found.");

builder.Services.AddDbContext<EnGeeContext>(options => options.UseSqlServer(connectionString));

//builder.Services.y<EnGeeUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<EnGeeContext>();

//Min新增
// 添加 Identity 服務設定AddDefaultIdentit
//builder.Services.AddIdentity<EnGeeUser, IdentityRole>()
//    .AddEntityFrameworkStores< EnGeeContext > ()
//    .AddDefaultTokenProviders();

//builder.Services.AddScoped<UserManager<EnGeeUser>>();
//builder.Services.AddScoped<SignInManager<EnGeeUser>>();
// Rong新增
builder.Services.AddDbContext<EngeeContext>(options => options.UseSqlServer(connectionString));

// 怡菁新增 MemberFavoriteRepository 到依賴注入容器
builder.Services.AddScoped<MemberFavoriteRepository>();

//加入 SignalR  NL
builder.Services.AddSignalR();

builder.Services.AddHttpClient();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "YourAuthCookieName";
        options.LoginPath = new PathString("/Home/Login");
        options.AccessDeniedPath = new PathString("/Home/AccessDenied");
    });


// Add services to the container.AddSession
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

// Rong新增
builder.Services.AddSingleton<IHostedService, CollectStatusUpdate>();

//CHI新增
builder.Services.AddSingleton<CHI_CUserViewModel>();



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

//加入 Hub NL
app.MapHub<ChatHub>("/chatHub");

app.Run();


