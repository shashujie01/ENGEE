using EnGee.Models;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using EnGee.ViewModels;
using EnGee.Services.EmailService;
using Microsoft.AspNetCore.Identity.UI.Services;
using MailKit.Security;
using MailKit.Net.Smtp;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using EnGee.Areas.Identity.Data;

namespace EnGee.Controllers
{
    public class MinMemberController : Controller
    {

        //------------------ 0917---------------------------------------//
        private readonly UserManager<EnGeeUser> _userManager;


        public MinMemberController(UserManager<EnGeeUser> userManager, SignInManager<EnGeeUser> signInManager)
        {
            _userManager = userManager;

        }

        public IActionResult EmailValidFail()
        {
            return View();//註冊加密失敗頁面
        }

        //-------------------------------------------------------------//
        public IActionResult EmailValid()
        {
            return View();//填寫表單後請使用者至信箱收信畫面
        }
        //-----------------------------------------------------------------------------------------//
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TMember tm, string username, string email, string gender)
        {


            //---------------條件限制規則--------------------//
            if (tm.Gender == null)
            {
                tm.Gender = "2";
            }
            else
            {
                tm.Gender = gender;
            }
            if (tm.RegistrationDate == null)
            {
                tm.RegistrationDate = DateTime.Now;  //註冊日預設當日
            }
            if (tm.Access == null)
            {
                if (tm.CharityProof != null)
                {
                    tm.Access = 3;  //公益團體權限3
                }
                else
                {
                    tm.Access = 1; //一般使用者
                }
            }
            if (tm.Point == null)
            {
                tm.Point = 0;
            }
            //----------------0916新增Email 點連結才會實際將會員註冊資料新增至資料庫---------------------//
            var randomToken = GenerateRandomToken();  // GenerateRandomToken()方法隨機產生20位數字字母字串，傳給變數randomToken
            tm.RandomToken = randomToken;

            //----------------0916修改比對資料庫與模型是否有重複username及email---------------------------//

            EngeeContext db = new EngeeContext();
            var reusername = db.TMembers.Any(x => x.Username == username);
            var reemail = db.TMembers.Any(x => x.Email == email);

            if (reusername)
            {
                ModelState.AddModelError("Username", "此帳號已經被使用。");
                return View();
            }
            if (reemail)
            {
                ModelState.AddModelError("Email", "此信箱已經被使用。");
                return View();
            }
            //----------------0916新增Email 點連結才會實際將會員註冊資料新增至資料庫---------------------//
            Response.Cookies.Append("memberstorageData", JsonConvert.SerializeObject(tm));  //將資料轉成jason檔存在cookie

            SendVerificationEmail(email, randomToken);
            //--------------0917 hash salt-   先將Tmember部分屬性轉移給模型EnGeeUser------------------------------//
            var user = new EnGeeUser   //實作類別EnGeeUser
            {
                UserName = username,
                Email = email,
               

            };

            // 使用 ASP.NET Core Identity 提供的 UserManager 來進行hash salt
            var result = await _userManager.CreateAsync(user, tm.Password);
            if (result.Succeeded)
            {
                    return RedirectToAction("EmailValid");
                //user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, tm.Password);  //此處再次進行加密可能是為了進一步確保密碼安全性。
                // 
                //var saveResult = _userManager.UpdateAsync(user).Result;  //確保將任何變更（包括密碼）保存到資料庫
                //if (saveResult.Succeeded)
                //{
                //}
                //else
                //{
                //    ModelState.AddModelError("", "註冊失敗");
                //    return RedirectToAction("EmailValidFail", "MinMember");
                //}
                    
            }
            else
            {
                // 用户创建失败的处理逻辑
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Error: {error.Description}");
                }
                ModelState.AddModelError("", "註冊失敗");
                return RedirectToAction("EmailValidFail", "MinMember");
            }
          

        }


        //-----------0916新增Email 連結可以跳轉至頁面-------------------------//
        private string GenerateRandomToken()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            var token= new string(Enumerable.Repeat(chars, 20).Select(s => s[random.Next(s.Length)]).ToArray());
            return token;
        }

        private IActionResult SendVerificationEmail(string emailto, string token)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("EnGee", "engeegift@gmail.com"));
            message.To.Add(new MailboxAddress(emailto, emailto)); //email是實際模型輸入值，命名為emailto傳給SendEmail
            message.Subject = "EnGee會員註冊驗證信";
            //-------信件連結會跳轉至MinMember/verifyEmail方法--------------//
            var verificationLink = Url.Action("VerifyEmail", "MinMember", new {token } ,Request.Scheme); //Request.Scheme確保 URL 使用正確的協議（例如，http 或 https）
            message.Body = new TextPart("plain") { Text = $"請點擊以下連結，完成註冊會員最後一步驟：\n{verificationLink}" };
            //---------------------//

            using var smtp = new SmtpClient();
            //smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
            //localhost測試時ssl加密先關閉
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("engeegift@gmail.com", "ijaqyfmyvlwpkjui");
            smtp.Send(message);
            smtp.Disconnect(true);
            return RedirectToAction("VerifyEmail");
        }

        //-----------0916新增Email 點連結才會實際將會員註冊資料新增至資料庫-------------------------//
        public IActionResult VerifyEmail(string token)
        {

            var memberStorageCookie = Request.Cookies["memberstorageData"];

            var tmCookie = JsonConvert.DeserializeObject<TMember>(memberStorageCookie);
            tmCookie.RandomToken = token;
            EngeeContext db = new EngeeContext();
            db.Add(tmCookie);
            db.SaveChanges();

            Response.Cookies.Delete("memberstorageData");

            return RedirectToAction("Login", "Home"); // (控制器/方法)
        }


        public IActionResult CreateStepForm(TMember tm)
        {
            return View();
        }

        public IActionResult CreateSignup(TMember tm)
        {
            return View();
        }

        //----------0920新增追蹤頁面------------------//
        public IActionResult Favor()
        {
            return View();
        }

	}
    
}

