using EnGee.Data;
using EnGee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjEnGeeDemo.ViewModels;
using prjMvcCoreDemo.Models;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using MimeKit;

namespace EnGee.Controllers
{
    public class Chi_MemberController : SuperController
    {
        private IWebHostEnvironment _enviro = null;
        public Chi_MemberController(IWebHostEnvironment p, CHI_CUserViewModel userViewModel)
        : base(userViewModel)
        {
            _enviro = p;
        }
        public IActionResult UserProfile()
        {

            using (var dbContext = new EngeeContext())
            {
                // 這裡可以直接使用 Session 中的用戶資訊，無需再次驗證
                string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
                TMember loggedInUser = JsonSerializer.Deserialize<TMember>(userJson);

                TMember userFromDatabase = dbContext.TMembers.FirstOrDefault(t => t.Email.Equals(loggedInUser.Email));
                if (userFromDatabase != null)
                {
                    CHI_CMemberWrap memberWrap = new CHI_CMemberWrap
                    {
                        member = userFromDatabase
                    };
                    return View(memberWrap);
                }
            }
            // 如果未找到用戶或其他錯誤情況導向登錄頁面
            return RedirectToAction("Login", "Home");
        }
            public IActionResult Edit(int? id)
            {
                if (id == null)
                    return RedirectToAction("UserProfile");
                EngeeContext db = new EngeeContext();
                TMember mem = db.TMembers.FirstOrDefault(t => t.MemberId == id);
                if (mem == null)
                    return RedirectToAction("UserProfile");
                CHI_CMemberWrap memWp = new CHI_CMemberWrap();
                memWp.member = mem;
                return View(memWp);
            }
            [HttpPost]
            public IActionResult Edit(CHI_CMemberWrap memIn)
            {
                EngeeContext db = new EngeeContext();
                TMember memDb = db.TMembers.FirstOrDefault(t => t.MemberId == memIn.MemberId);

                if (memDb != null)
                {
                if (memIn.photo != null && memIn.photo.Length > 0)
                {
                    // 取得圖檔類型
                    var fileExtension = Path.GetExtension(memIn.photo.FileName).ToLower();

                    // 檢查圖檔類型
                    if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                    {
                        string photoName = Guid.NewGuid().ToString() + fileExtension;
                        string path = _enviro.WebRootPath + "/images/UserImage/" + photoName;
                        memIn.photo.CopyTo(new FileStream(path, FileMode.Create));
                        memDb.PhotoPath = photoName;
                    }
                    else
                    {
                        
                        ModelState.AddModelError("photo", "只接受jpg、jpeg和png格式的圖片");
                        return View(memIn);
                    }
                }

                memDb.Fullname = memIn.Fullname;
                memDb.Birth= (DateTime)memIn.Birth;//20230919合併出錯時加入
                memDb.Gender = memIn.Gender;
                memDb.Email= memIn.Email;
                memDb.Address=memIn.Address;
                memDb.Phone= memIn.Phone;
                memDb.Introduction= memIn.Introduction;
                
                db.SaveChanges();

                // 儲存新的頭像
                HttpContext.Session.SetString(CDictionary.SK_LOINGED_USER_PHOTO, memDb.PhotoPath);
            }
                return RedirectToAction("UserProfile");
            }

        public IActionResult EditPassword()
        {
            string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            TMember loggedInUser = JsonSerializer.Deserialize<TMember>(userJson);

            CHI_CMemberWrap memberWrap = new CHI_CMemberWrap
            {
                member = loggedInUser
            };

            return View(memberWrap);
        }

        [HttpPost]
        public IActionResult EditPassword(string oldPassword, string NewPassword)
        {
            string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            TMember loggedInUser = JsonSerializer.Deserialize<TMember>(userJson);

            if (loggedInUser != null && ValidateOldPassword(loggedInUser, oldPassword))
            {
                string hashedNewPassword = HashPassword(NewPassword);
                loggedInUser.Password = hashedNewPassword;

                string updatedUserJson = JsonSerializer.Serialize(loggedInUser);
                HttpContext.Session.SetString(CDictionary.SK_LOINGED_USER, updatedUserJson);

                TempData["SuccessMessage"] = "密碼更新成功";
                return View();
            }
            else
            {                
                ModelState.AddModelError(string.Empty, "舊密碼不正確");
                return View();
            }
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
        private bool ValidateOldPassword(TMember user, string oldPassword)
        {
            return user.Password == oldPassword;
        }
         public IActionResult Apply(int? id)
            {
                if (id == null)
                    return RedirectToAction("UserProfile");
                EngeeContext db = new EngeeContext();
                TMember mem = db.TMembers.FirstOrDefault(t => t.MemberId == id);
                if (mem == null)
                    return RedirectToAction("UserProfile");
                CHI_CMemberWrap memWp = new CHI_CMemberWrap();
                memWp.member = mem;
                return View(memWp);
            }
        [HttpPost]
        public IActionResult Apply(CHI_CMemberWrap memIn)
        {
            EngeeContext db = new EngeeContext();
            TMember memDb = db.TMembers.FirstOrDefault(t => t.MemberId == memIn.MemberId);
            if (memDb != null)
            {
                if (memIn.photoCharityProof != null && memIn.photoCharityProof.Length > 0)
                {
                    Console.WriteLine(memIn.photoCharityProof);
                    // 取得圖檔類型
                    var fileExtension = Path.GetExtension(memIn.photoCharityProof.FileName).ToLower();

                    // 檢查圖檔類型
                    if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                    {
                        string photoName = Guid.NewGuid().ToString() + fileExtension;
                        Console.WriteLine("Generated photoName: " + photoName);
                        string path = _enviro.WebRootPath + "/images/CharityProof/" + photoName;
                        memIn.photoCharityProof.CopyTo(new FileStream(path, FileMode.Create));
                        memDb.CharityProof = photoName;
                    }
                    else
                    {

                        ModelState.AddModelError("photo", "只接受jpg、jpeg和png格式的圖片");
                        return View(memIn);
                    }
                }
                memDb.Fullname = memIn.Fullname;
                memDb.Username = memIn.Username;

                db.SaveChanges();

                // 儲存新的頭像
                //HttpContext.Session.SetString(CDictionary.SK_LOINGED_USER_PHOTO, memDb.CharityProof);
            }
            return RedirectToAction("UserProfile");
        }



    }
}



