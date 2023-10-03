using EnGee.Models;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using EnGee.ViewModels;
using EnGee.Services.EmailService;
using Microsoft.AspNetCore.Identity.UI.Services;
using MailKit.Security;
using MailKit.Net.Smtp;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;

namespace EnGee.Controllers
{
	public class MinMemberController : Controller
	{
		public IActionResult EmailValidFail()
		{
			return View();
		}

		public IActionResult EmailValid()
		{
			return View();
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Create(TMember tm, string username, string email, string gender, string password)
		{
			if (tm.RegistrationDate == null)
			{
				tm.RegistrationDate = DateTime.Now;
			}
			if (tm.Access == null)
			{
				if (tm.CharityProof != null)
				{
					tm.Access = 3;
				}
				else
				{
					tm.Access = 1;
				}
			}
			if (tm.Point == null)
			{
				tm.Point = 0;
			}

			var randomToken = GenerateRandomToken();
			tm.RandomToken = randomToken;
			var hashpassword = HashPassword(password);          //hash256加密
			tm.Password = hashpassword;

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

			var tmJson = JsonConvert.SerializeObject(tm);
			Response.Cookies.Append("memberstorageData", tmJson);

			SendVerificationEmail(email, randomToken);

			return RedirectToAction("EmailValid");
		}

		private string GenerateRandomToken()
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			Random random = new Random();
			var token = new string(Enumerable.Repeat(chars, 20).Select(s => s[random.Next(s.Length)]).ToArray());
			return token;
		}

		private IActionResult SendVerificationEmail(string emailto, string token)
		{
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress("EnGee", "engeegift@gmail.com"));
			message.To.Add(new MailboxAddress(emailto, emailto));
			message.Subject = "EnGee會員註冊驗證信";

			var verificationLink = Url.Action("VerifyEmail", "MinMember", new { token }, Request.Scheme);
			message.Body = new TextPart("plain") { Text = $"請點擊以下連結，完成信箱註冊：\n{verificationLink}" };

			using var smtp = new SmtpClient();
			smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
			smtp.Authenticate("engeegift@gmail.com", "ijaqyfmyvlwpkjui");
			smtp.Send(message);
			smtp.Disconnect(true);

			return RedirectToAction("VerifyEmail");
		}

		public IActionResult VerifyEmail(string token, TMember tm)
		{
			var memberStorageCookie = Request.Cookies["memberstorageData"];
			var tmCookie = JsonConvert.DeserializeObject<TMember>(memberStorageCookie);
			EngeeContext db = new EngeeContext();
			db.Add(tmCookie);
			db.SaveChanges();

			Response.Cookies.Delete("memberstorageData");

			return RedirectToAction("Login", "Home");
		}

		public IActionResult CreateStepForm(TMember tm)
		{
			return View();
		}

		public IActionResult CreateSignup(TMember tm)
		{
			return View();
		}

		private string HashPassword(string password)
		{
			using (SHA256 sha256 = SHA256.Create())
			{
				byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
				StringBuilder builder = new StringBuilder();
				for (int i = 0; i < hashedBytes.Length; i++)
				{
					builder.Append(hashedBytes[i].ToString("x2"));
				}

				return builder.ToString();
			}
		}
	}
}
