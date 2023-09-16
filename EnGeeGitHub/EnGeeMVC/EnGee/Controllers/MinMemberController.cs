using EnGee.Models;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit;
using EnGee.ViewModels;
using EnGee.Services.EmailService;
using Microsoft.AspNetCore.Identity.UI.Services;
using MailKit.Security;
using MailKit.Net.Smtp;

namespace EnGee.Controllers
{
    public class MinMemberController : Controller
    {
        public IActionResult EmailValid()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(TMember tm, string username, string email)
        {
            //---------------條件限制規則--------------------//
            if (tm.Gender == null)
            {
                tm.Gender = "2";
            }
            if (tm.RegistrationDate == null)
            {
                tm.RegistrationDate = DateTime.Now;  //註冊日預設當日
            }
            if (tm.Access == null)
            {
                if(tm.CharityProof != null)
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
            //----------------0916修改比對資料庫與模型是否有重複username及email---------------------------//

            EngeeContext db = new EngeeContext();
            var reusername=db.TMembers.Any(x => x.Username == username);
            var reemail=db.TMembers.Any (x => x.Email == email);

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
            db.Add(tm);
            db.SaveChanges();
            return RedirectToAction("SendEmail", new { emailto = email });  //email是實際模型輸入值，命名為emailto傳給SendEmail
        }

        //----------0915新增EmailSend Test--------//

        public IActionResult SendEmail(string emailto)   //email是實際模型輸入值，命名為emailto傳給SendEmail
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("EnGee", "engeegift@gmail.com"));
            message.To.Add(new MailboxAddress(emailto, emailto)); //email是實際模型輸入值，命名為emailto傳給SendEmail
            message.Subject = "EnGee會員註冊驗證信";
            message.Body = new TextPart("plain") { Text = "請點擊以下連結，完成註冊會員最後一步驟 >>  https://engee2023.azurewebsites.net/" };





            using var smtp = new SmtpClient();
            //smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
            //localhost測試時ssl加密先關閉
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("engeegift@gmail.com", "ijaqyfmyvlwpkjui");
            smtp.Send(message);
            smtp.Disconnect(true);

            return RedirectToAction("EmailValid");
        }

    }
}

