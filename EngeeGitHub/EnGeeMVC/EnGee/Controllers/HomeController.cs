using EnGee.Data;
using EnGee.Models;
using EnGee.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using prjMvcCoreDemo.Models;
using System.Diagnostics;
using System.Text.Json;

namespace EnGee.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOINGED_USER))
            {
                return RedirectToAction("IndexLoggin"); 
            }
            return View(); // 繼續顯示首頁
        }

        public IActionResult IndexLoggin()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult News()
        {
            return View();
        }
        public IActionResult LoginLayout()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult LoginLayout(CLoginViewModel vm)
        //{
        //    TMember user = (new EngeeContext()).TMembers.FirstOrDefault(
        //        t => t.Email.Equals(vm.txtAccount) && t.Password.Equals(vm.txtPassword));
        //    if (user != null && user.Password.Equals(vm.txtPassword))
        //    {
        //        string json = JsonSerializer.Serialize(user);
        //        HttpContext.Session.SetString(CDictionary.SK_LOINGED_USER, json);

        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(); // 清除使用者的驗證 Cookie
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult LoginLayout(CLoginViewModel vm)
        {
            TMember user = (new EngeeContext()).TMembers.FirstOrDefault(
                t => t.Email.Equals(vm.txtAccount) && t.Password.Equals(vm.txtPassword));

            if (user != null && user.Password.Equals(vm.txtPassword))
            {
                HttpContext.Session.SetString(CDictionary.SK_LOINGED_USER, JsonSerializer.Serialize(user));

                Console.WriteLine("Entering HandleRedirectAfterLogin");
                string redirectPage = HttpContext.Session.GetString("RedirectAfterLogin") ?? "Index";
                int? txtProductId = HttpContext.Session.GetInt32("TempProductId");
                int? deliverytypeid = HttpContext.Session.GetInt32("TempDeliverytypeid");
                int? txtCount = HttpContext.Session.GetInt32("TempTxtCount");
                if (redirectPage.Contains("QuickAddToCart"))
                {
                    return RedirectToAction("QuickAddToCart", "Shopping", new { txtProductId });
                }
                else if (redirectPage.Contains("CartView"))
                {
                    return RedirectToAction("CartView", "Shopping", new { txtProductId });
                }
                else if (redirectPage.Contains("AddToCartAndReturnCarView"))
                {
                    if (txtProductId.HasValue && deliverytypeid.HasValue && txtCount.HasValue)
                    {
                        Console.WriteLine($"Redirecting to AddToCartAndReturnCarView with txtProductId={txtProductId}, txtCount={txtCount}, deliverytypeid={deliverytypeid}");
                        return RedirectToAction("AddToCartAndReturnCarView", "Shopping", new
                        {
                            txtProductId = (int)txtProductId,
                            txtCount = (int)txtCount,
                            deliverytypeid = (int)deliverytypeid,
                        });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else if (redirectPage.Contains("AddToCart"))
                {
                    if (txtProductId.HasValue && deliverytypeid.HasValue && txtCount.HasValue)
                    {
                        TempData["RedirectToAction"] = "AddToCart"; //辨識字串存temp，使 View 知道需要重啟 AJAX 請求
                        return RedirectToAction("Details", "Product", new { id = txtProductId });//並非導回AddToCart
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                //return HandleRedirectAfterLogin();
            }

            return View();
        }
    }
}