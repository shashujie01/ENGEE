﻿using EnGee.Data;
using EnGee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjEnGeeDemo.ViewModels;
using prjMvcCoreDemo.Models;
using System.Text.Json;
using System.Security.Cryptography;

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
                ViewBag.NewPhotoPath = memDb.PhotoPath;
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
        public IActionResult EditPassword(CHI_CMemberWrap memIn)
        {
            EngeeContext db = new EngeeContext();
            TMember memDb = db.TMembers.FirstOrDefault(t => t.MemberId == memIn.MemberId);

            if (memDb != null)
            {
                // 驗證舊密碼
                if (ValidateOldPassword(memDb, memIn.OldPassword))
                {
                    // 更新新密码
                    memDb.Password = HashPassword(memIn.NewPassword); 

                    db.SaveChanges();

                    return RedirectToAction("UserProfile");
                }
                else
                {
                    ModelState.AddModelError("OldPassword", "舊密碼不正確");
                }
            }

            return View(memIn);
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
        private bool VerifyPassword(string hashedPassword, string password)
        {
            string hashedInputPassword = HashPassword(password);
            return string.Equals(hashedInputPassword, hashedPassword, StringComparison.OrdinalIgnoreCase);
        }
        private bool ValidateOldPassword(TMember user, string oldPassword)
        {
            
            string hashedOldPassword = HashPassword(oldPassword);
            return user.Password == hashedOldPassword;
        }





    }
}



