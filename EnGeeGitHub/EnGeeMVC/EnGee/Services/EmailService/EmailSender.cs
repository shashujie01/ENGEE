using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using EnGee.ViewModels;
using Microsoft.AspNetCore.Mvc;
using EnGee.Controllers;

namespace EnGee.Services.EmailService
{
    public class EmailSender : IEmailService
    {
        //private readonly IConfiguration _config;

        //public EmailSender(IConfiguration config)
        //{
        //    _config = config;
        //}

        public void SendEmail(/*EmailDto request*/)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("engee", "engeegift@gmail.com"));
            message.To.Add(new MailboxAddress("ivy", "ivy1101238@yahoo.com.tw"));
            message.Subject = "MailKit Test";
            message.Body = new TextPart("plain") { Text = "Hi from MailKit!" };





            using var smtp = new SmtpClient();
            //smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
            //localhost測試時ssl加密先關閉
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("engeegift@gmail.com", "ijaqyfmyvlwpkjui");
            smtp.Send(message);
            smtp.Disconnect(true);

           
        }


    }

}