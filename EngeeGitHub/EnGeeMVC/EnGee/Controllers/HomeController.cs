using EnGee.Data;
using EnGee.Models;
using EnGee.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjMvcCoreDemo.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace EnGee.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private CHI_CUserViewModel UserVM;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(CLoginViewModel vm)
        {
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOINGED_USER))
            {
                return RedirectToAction("IndexLoggin");
            }
            return View(vm); // 繼續顯示首頁
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
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(CLoginViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.txtAccount) || string.IsNullOrWhiteSpace(vm.txtPassword))
            {
                ModelState.AddModelError("", "請輸入帳號及密碼。");
                return View();
            }
            string hashedPassword = ComputeSha256Hash(vm.txtPassword);
            TMember user = (new EngeeContext()).TMembers.FirstOrDefault(
                t => t.Email.Equals(vm.txtAccount) && t.Password.Equals(hashedPassword));

            if (user != null && user.Password.Equals(hashedPassword))
            {
                string json = JsonSerializer.Serialize(user);
                HttpContext.Session.SetString(CDictionary.SK_LOINGED_USER, json);
                vm.TMember = user;

                string redirectPage = HttpContext.Session.GetString("RedirectAfterLogin") ?? "Index";
                int? txtProductId = HttpContext.Session.GetInt32("TempProductId");
                int? deliverytypeid = HttpContext.Session.GetInt32("TempDeliverytypeid");
                int? txtCount = HttpContext.Session.GetInt32("TempTxtCount");

                if (redirectPage.Contains("QuickAddToCart"))
                {
                    return RedirectToAction("QuickAddToCart", "Shopping", new { txtProductId });
                }
                else if (redirectPage.Contains("AddToCartAndReturnCarView"))
                {
                    if (txtProductId.HasValue && deliverytypeid.HasValue && txtCount.HasValue)
                    {
                        return RedirectToAction("AddToCartAndReturnCarView", "Shopping", new
                        {
                            txtProductId = (int)txtProductId,
                            txtCount = (int)txtCount,
                            deliverytypeid = (int)deliverytypeid,
                        });
                    }
                    else//加這段是怕找不到值
                    { return RedirectToAction("Index", "Home"); }
                }
                else if (redirectPage.Contains("AddToCart"))
                {
                    if (txtProductId.HasValue && deliverytypeid.HasValue && txtCount.HasValue)
                    {
                        TempData["RedirectToAction"] = "AddToCart"; //辨識字串存temp，使 View 知道需要重啟 AJAX 請求
                        return RedirectToAction("Details", "Product", new { id = txtProductId });//並非導回AddToCart
                    }
                    else//加這段是怕找不到值
                    { return RedirectToAction("Index", "Home"); }
                }
                else if (HttpContext.Session.TryGetValue(CDictionary.SK_RETURN_URL, out byte[] returnUrlBytes))
                {
                    string returnUrl = Encoding.UTF8.GetString(returnUrlBytes);
                    HttpContext.Session.Remove(CDictionary.SK_RETURN_URL);
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index");  
            }
            else
            {
                ModelState.AddModelError("", "帳號或密碼不正確。");
                return View(vm);
            }
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(); // 清除認證
            HttpContext.Session.Clear(); // 清除Session
            return RedirectToAction("Index");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}