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
        public IActionResult Create(TMember tm)
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
            //-------------------------------------------//


            EngeeContext db = new EngeeContext();

            db.Add(tm);
            db.SaveChanges();
            return RedirectToAction("SendEmail");
        }

        //----------0915新增EmailSend Test--------//
        //private readonly IEmailSender _emailsenderIn;
        //public MinMemberController(IEmailSender emailSenderIn)
        //{
        //    _emailsenderIn= emailSenderIn;
        //}
        public IActionResult SendEmail(/*EmailDto request*/)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("EnGee", "engeegift@gmail.com"));
            message.To.Add(new MailboxAddress("ivy1101238@yahoo.com.tw", "ivy1101238@yahoo.com.tw"));
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

