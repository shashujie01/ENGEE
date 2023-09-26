using EnGee.Models;
using EnGee.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjMvcCoreDemo.Models;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Security.Cryptography;
using System.Text.Json;



namespace EnGee.Controllers
{
    public class CollectController : Controller
    {
        private IWebHostEnvironment _enviro = null;

        public CollectController(IWebHostEnvironment c)
        {
            _enviro = c;
        }

        private EngeeContext db = new EngeeContext();

        // 許願池管理
        public IActionResult CollectManagement(Rong_keywordViewModel k)
        {
            // 登入判斷
            string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            if (userJson == null)
                return RedirectToAction("Login", "Home");
            else
            {
                TMember loggedInUser = JsonSerializer.Deserialize<TMember>(userJson);

                if (loggedInUser.Access == 1 || loggedInUser.Access == 3)
                {
                    return RedirectToAction("CollectIndex");
                }
            }

            IEnumerable<Rong_CollectManagementViewModel> collectmanage =
                from co in db.TCollects
                join d in db.TDeliveryTypes on co.DeliveryTypeId equals d.DeliveryTypeId
                select new Rong_CollectManagementViewModel
                {
                    CollectId = co.CollectId,
                    MemberId = co.MemberId,
                    CollectTitle = co.CollectTitle,
                    CollectStartDate = co.CollectStartDate.ToString("yyyy/MM/dd"),
                    CollectEndDate = co.CollectEndDate.ToString("yyyy/MM/dd"),
                    DeliveryType = d.DeliveryType,
                    CollectStatus = co.CollectStatus
                };

            if (!string.IsNullOrEmpty(k.txtKeyword))
            {
                collectmanage = collectmanage.Where(i => i.CollectTitle.Contains(k.txtKeyword));
            }
            else
            {
                return View(collectmanage);
            }

            return View(collectmanage);
        }

        // 許願池首頁
        public IActionResult CollectIndex(Rong_keywordViewModel k, int mainId, int subId, int page = 1, int sortBy = 1)
        {
            

            IEnumerable<Rong_CollectIndexViewModel> collectindex =
                from co in db.TCollects
                 join m in db.TMembers on co.MemberId equals m.MemberId
                 where co.CollectStatus == true
                 select new Rong_CollectIndexViewModel
                 {
                     CollectId = co.CollectId,
                     MemberId = co.MemberId,
                     Username = m.Username,
                     CollectTitle = co.CollectTitle,
                     CollectStartDate = co.CollectStartDate.ToString("yyyy/MM/dd"),
                     CollectEndDate = co.CollectEndDate.ToString("yyyy/MM/dd"),
                     CollectImagePath = co.CollectImagePath,
                     MainCategoryId = co.MainCategoryId,
                     SubcategoryId = co.SubcategoryId,
                     CollectStatus = co.CollectStatus,
                     member = m,
                     TotalPublished = db.TCollects
                        .Where(c => c.MemberId == m.MemberId && c.CollectStatus == true)
                        .Count(),
                 };
            
            // 日期排序
            
            List<Rong_CollectIndexViewModel> orderedCollectIndex = collectindex.ToList();
            if (sortBy == 1)
            {
                orderedCollectIndex = orderedCollectIndex.OrderBy(co => co.CollectStartDate).ToList();
            }
            else
            {
                orderedCollectIndex = orderedCollectIndex.OrderByDescending(co => co.CollectStartDate).ToList();
            }

            // 化妝品分類
            if (subId != 0)
            {
                orderedCollectIndex = orderedCollectIndex.Where(co =>co.SubcategoryId == subId).ToList();
            }
            else if (mainId != 0)
            { 
                orderedCollectIndex = orderedCollectIndex.Where(co => co.MainCategoryId == mainId).ToList(); 
            }
            else 
            {
                orderedCollectIndex = orderedCollectIndex.Where(co => co.CollectStatus == true).ToList();
            }

            // 搜尋功能
            if (!string.IsNullOrEmpty(k.txtKeyword))
            {
                orderedCollectIndex = orderedCollectIndex.Where(i => i.CollectTitle.Contains(k.txtKeyword)).ToList();
            }

            // 分頁
            int pageSize = 12;
            //  依照前端商品有幾項來分頁
            int stillCollect = orderedCollectIndex.Count();
            Console.WriteLine("Total Still Collects: " + stillCollect);
            int totalPage = (int)Math.Ceiling((double)stillCollect / pageSize);
            ViewData["CurrentPage"] = page;
            ViewData["TotalPage"] = totalPage;
            //ViewData["StillCollect"] = stillCollect;
            //  讓分類維持依照時間排序
            ViewData["CurrentSortBy"] = sortBy;

            // 化妝品分類
            var mainca = db.TCosmeticMainCategories.ToList();
            var subca = db.TCosmeticSubcategories.ToList();
            ViewBag.MainCategory = mainca;
            ViewBag.Subcategory = subca;

            // 登入判斷
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOINGED_USER))
            {
                ViewBag.ShowWishButton = false;
            }
            else
            {
                string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
                TMember loggedInUser = JsonSerializer.Deserialize<TMember>(userJson);
                if (loggedInUser.Access == 3)
                {
                    ViewBag.ShowWishButton = true;
                }
                else
                    ViewBag.ShowWishButton = false;
            }

            collectindex = orderedCollectIndex
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return View(collectindex);
        }

        // 許願詳細頁面
        public IActionResult CollectInformation(int? id)
        {
            if (id == null)
                return RedirectToAction("CollectIndex");
            var cinfo =
                (from c in db.TCollects
                 join d in db.TDeliveryTypes on c.DeliveryTypeId equals d.DeliveryTypeId
                 join m in db.TMembers on c.MemberId equals m.MemberId into memberJoin
                 from m in memberJoin.DefaultIfEmpty()
                 where c.CollectId == id
                 select new Rong_CollectInformationViewModel
                 {
                     CollectId = c.CollectId,
                     MemberId = c.MemberId,
                     Username = m.Username,
                     CollectTitle = c.CollectTitle,
                     CollectCaption = c.CollectCaption,
                     CollectStartDate = c.CollectStartDate.ToString("yyyy/MM/dd"),
                     CollectEndDate = c.CollectEndDate.ToString("yyyy/MM/dd"),
                     DeliveryTypeId = c.DeliveryTypeId,
                     DeliveryType = d.DeliveryType,
                     DeliveryFee = (int)d.DeliveryFee,
                     DeliveryAddress = c.DeliveryAddress != null ? c.DeliveryAddress : "",
                     ConvenienNum = c.ConvenienNum != null ? c.ConvenienNum : "",
                     CollectImagePath = c.CollectImagePath,
                     CollectItemName = c.CollectItemName,
                     CollectAmount = c.CollectAmount,
                     member = m,  //   取得發布者的資料
                 })
                .FirstOrDefault();

            //  目前已徵求到的數量
            int TotalDonationAmount = 0;
            TotalDonationAmount = db.TDonationOrders
                .Where(d => d.CollectId == id)
                .Sum(d => d.DonationAmount);
            ViewBag.TotalDonationAmount = TotalDonationAmount;

            

            //  該會員共發布幾篇許願
            int TotalPublished = 0;
            TotalPublished = db.TCollects
                .Where(c => c.MemberId == cinfo.MemberId)
                .Count();
            ViewBag.Published = TotalPublished;


            // 登入判斷
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOINGED_USER))
            {
                ViewBag.ShowEditAndDeleteButton = false;
                ViewBag.ShowAlert = true;
                ViewBag.ShowDonateBtn = true;
            }
            else
            {
                string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
                TMember loggedInUser = JsonSerializer.Deserialize<TMember>(userJson);
                if (loggedInUser.MemberId == cinfo.MemberId)
                {
                    ViewBag.ShowEditAndDeleteButton = true;
                    ViewBag.ShowDonateBtn = false;
                }
                else
                {
                    ViewBag.ShowEditAndDeleteButton = false;
                    ViewBag.ShowDonateBtn = true;
                }
            }

            return View(cinfo);
        }

        // 我要許願
        public IActionResult Create()
        {
            string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            if (userJson == null)
                return RedirectToAction("Login", "Home");
            else
            {
                TMember loggedInUser = JsonSerializer.Deserialize<TMember>(userJson);

                if (loggedInUser.Access != 3)
                {
                    return RedirectToAction("CollectIndex");
                }
            }
            var mainca = db.TCosmeticMainCategories.ToList();
            var subca = db.TCosmeticSubcategories.ToList();
            var deliverytype = db.TDeliveryTypes.ToList();
            ViewBag.DeliveryType = deliverytype;
            ViewBag.MainCategory = mainca;
            ViewBag.Subcategory = subca;
            return View();
        }
        [HttpPost]
        public IActionResult Create(Rong_CollectImageViewModel ci)
        {
            if (ci.photo != null)
            {
                string photoName = Guid.NewGuid().ToString() + ".jpg";
                string path = _enviro.WebRootPath + "/images/donation/" + photoName;
                ci.photo.CopyTo(new FileStream(path, FileMode.Create));
                ci.CollectImagePath = photoName;
            }

            // 登入判斷
            string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            TMember loggedInUser = JsonSerializer.Deserialize<TMember>(userJson);
            if (loggedInUser.MemberId != null)
            {
                ViewBag.MemberId = loggedInUser.MemberId;
            }
            TCollect c = new TCollect()
            {
                MemberId = ViewBag.MemberId,
                CollectTitle = ci.CollectTitle,
                CollectCaption = ci.CollectCaption,
                CollectStartDate = ci.CollectStartDate,
                CollectEndDate = ci.CollectEndDate,
                DeliveryTypeId = ci.DeliveryTypeId,
                DeliveryAddress = ci.DeliveryAddress,
                ConvenienNum = ci.ConvenienNum,
                CollectStatus = ci.CollectStatus,
                CollectImagePath = ci.CollectImagePath,
                CollectItemName = ci.CollectItemName,
                MainCategoryId = ci.MainCategoryId,
                SubcategoryId = ci.SubcategoryId,
                CollectAmount = ci.CollectAmount
            };
            db.TCollects.Add(c);
            db.SaveChanges();
            return RedirectToAction("CollectInformation");
        }
        // 管理_許願刪除
        public IActionResult DeleteCollectManagement(int? id)
        {
            
            if (id == null)
                return RedirectToAction("CollectManagement");
            TCollect c = db.TCollects.FirstOrDefault(t => t.CollectId == id);
            if (c != null)
            {
                db.TCollects.Remove(c);
                db.SaveChanges();
            }
            return RedirectToAction("CollectManagement");
        }
        // 會員_許願刪除
        public IActionResult DeleteCollectMember(int? id)
        {
            if (id == null)
                return RedirectToAction("Create");

            TCollect c = db.TCollects.FirstOrDefault(t => t.CollectId == id);
            string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            TMember loggedInUser = JsonSerializer.Deserialize<TMember>(userJson);
            if (loggedInUser.MemberId != c.MemberId)
            {
                return RedirectToAction("CollectIndex");
            }
            if (c != null)
            {
                
                db.TCollects.Remove(c);
                db.SaveChanges();
            }
            return RedirectToAction("CollectIndex");
        }

        // 許願修改
        public IActionResult EditCollect(int? id)
        {
            if (id == null)
                return RedirectToAction("CollectIndex");
            TCollect c = db.TCollects.FirstOrDefault(t => t.CollectId == id);
            string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            if (userJson == null)
                return RedirectToAction("Login", "Home");
            else
            {
                TMember loggedInUser = JsonSerializer.Deserialize<TMember>(userJson);

                if (loggedInUser.Access == 1 || (loggedInUser.Access == 3 && loggedInUser.MemberId != c.MemberId))
                {
                    return RedirectToAction("CollectIndex");
                }
            }
            
            if (c == null)
                return RedirectToAction("CollectIndex");
            Rong_CCollectWrap cWrap = new Rong_CCollectWrap();
            cWrap.coll = c;
            var mainca = db.TCosmeticMainCategories.ToList();
            var subca = db.TCosmeticSubcategories.ToList();
            var deliverytype = db.TDeliveryTypes.ToList();
            ViewBag.DeliveryType = deliverytype;
            ViewBag.MainCategory = mainca;
            ViewBag.Subcategory = subca;
            return View(cWrap);
        }
        [HttpPost]
        public IActionResult EditCollect(Rong_CCollectWrap cIn)
        {
            TCollect cDb = db.TCollects.FirstOrDefault(t => t.CollectId == cIn.CollectId);

            if (cDb != null)
            {
                if (cIn.photo != null)
                {
                    string photoName = Guid.NewGuid().ToString() + ".jpg";
                    string path = _enviro.WebRootPath + "/images/donation/" + photoName;
                    cIn.photo.CopyTo(new FileStream(path, FileMode.Create));
                    cIn.CollectImagePath = photoName;
                }
                else
                {
                    cIn.CollectImagePath = cDb.CollectImagePath;
                }

                cDb.CollectTitle = cIn.CollectTitle;
                cDb.CollectCaption = cIn.CollectCaption;
                cDb.CollectEndDate = cIn.CollectEndDate;
                cDb.DeliveryTypeId = cIn.DeliveryTypeId;
                cDb.DeliveryAddress = cIn.DeliveryAddress;
                cDb.ConvenienNum = cIn.ConvenienNum;
                cDb.CollectStatus = cIn.CollectStatus;
                cDb.CollectImagePath = cIn.CollectImagePath;
                cDb.MainCategoryId = cIn.MainCategoryId;
                cDb.SubcategoryId = cIn.SubcategoryId;
                cDb.CollectAmount = cIn.CollectAmount;
                db.SaveChanges();
            }

            return RedirectToAction("CollectInformation");
        }

        // 捐贈表單
        public IActionResult Donate(int? id)
        {
            // 登入判斷
            string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            if (userJson == null)
                return RedirectToAction("Login", "Home");
            else
            {
                TMember loggedInUser = JsonSerializer.Deserialize<TMember>(userJson);

                if (loggedInUser.MemberId != null)
                {
                    ViewBag.MemberId = loggedInUser.MemberId;
                    ViewBag.MemberPoint = loggedInUser.Point;
                }
            }
            
            if (id == null)
                return RedirectToAction("CollectIndex");
            var doninfo =
                (from c in db.TCollects
                 join d in db.TDeliveryTypes on c.DeliveryTypeId equals d.DeliveryTypeId
                 where c.CollectId == id
                 select new Rong_DonateViewModel
                 {
                     CollectId = c.CollectId,
                     DeliveryTypeId = c.DeliveryTypeId,
                     DeliveryType = d.DeliveryType,
                     DeliveryAddress = c.DeliveryAddress != null ? c.DeliveryAddress : "",
                     DeliveryFee = d.DeliveryFee,
                     ConvenienNum = c.ConvenienNum != null ? c.ConvenienNum : "",
                     CollectItemName = c.CollectItemName,
                     CollectAmount = c.CollectAmount,
                 })
                .FirstOrDefault();
            return View(doninfo);
        }
        [HttpPost]
        public IActionResult Donate(Rong_DonateViewModel d)
        {
            var collect = db.TCollects.FirstOrDefault(c => c.CollectId == d.CollectId);

            // 登入判斷
            string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            TMember loggedInUser = JsonSerializer.Deserialize<TMember>(userJson);
            var member = db.TMembers.FirstOrDefault(m => m.MemberId == loggedInUser.MemberId);

            if (loggedInUser.MemberId != null)
            {
                ViewBag.MemberId = loggedInUser.MemberId;
                ViewBag.MemberPoint = loggedInUser.Point;
            }

            // 扣除點數 + 扣除徵求數量
            if (loggedInUser != null && collect != null && ViewBag.MemberId != null)
            {
                var selectedDeliveryType = db.TDeliveryTypes.FirstOrDefault(dt => dt.DeliveryTypeId == d.DeliveryTypeId);
                if (selectedDeliveryType != null)
                {
                    int deliveryFee = (int)selectedDeliveryType.DeliveryFee;
                    if (ViewBag.MemberPoint >= deliveryFee)
                    {
                        int newMemberPoint = (int)(ViewBag.MemberPoint - deliveryFee);
                        member.Point = newMemberPoint;

                        int newCollectAmount = collect.CollectAmount - d.DonationAmount;
                        collect.CollectAmount = newCollectAmount;
                        TDonationOrder don = new TDonationOrder()
                        {
                            MemberId = ViewBag.MemberId,
                            CollectId = d.CollectId,
                            OrderDate = DateTime.Now,
                            DeliveryTypeId = d.DeliveryTypeId,
                            DonarName = d.DonarName,
                            DonarPhone = d.DonarPhone,
                            DonationStatus = d.DonationStatus,
                            DonationAmount = d.DonationAmount
                        };
                        db.TDonationOrders.Add(don);
                        db.SaveChanges();

                        return RedirectToAction("CollectIndex");
                    }
                }
            }
            return RedirectToAction("CollectIndex");
        }

        // 捐贈訂單管理
        public IActionResult DonationManagement(Rong_keywordViewModel k)
        {
            string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            if (userJson == null)
                return RedirectToAction("Login", "Home");
            else
            {
                TMember loggedInUser = JsonSerializer.Deserialize<TMember>(userJson);

                if (loggedInUser.Access == 1 || loggedInUser.Access == 3)
                {
                    return RedirectToAction("CollectIndex");
                }
            }
            
            IEnumerable<Rong_DonationManagementViewModel> donationmanage =
                from don in db.TDonationOrders
                join d in db.TDeliveryTypes on don.DeliveryTypeId equals d.DeliveryTypeId
                join c in db.TCollects on don.CollectId equals c.CollectId
                select new Rong_DonationManagementViewModel
                {
                    DonationOrderId = don.DonationOrderId,
                    MemberId = don.MemberId,
                    CollectId = don.CollectId,
                    OrderDate = don.OrderDate.ToString("yyyy/MM/dd"),
                    DeliveryType = d.DeliveryType,
                    DonarName = don.DonarName,
                    DonarPhone = don.DonarPhone,
                    CollectItemName = c.CollectItemName,
                    DonationAmount = don.DonationAmount,
                    DonationStatus = don.DonationStatus
                };

            if (!string.IsNullOrEmpty(k.txtKeyword))
            {
                donationmanage = donationmanage.Where(i => i.DonarName.Contains(k.txtKeyword));
            }
            else
            {
                return View(donationmanage);
            }

            return View(donationmanage);
        }

        // 捐贈修改
        public IActionResult EditDonation(int? id)
        {
            string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            if (userJson == null)
                return RedirectToAction("Login", "Home");
            else
            {
                TMember loggedInUser = JsonSerializer.Deserialize<TMember>(userJson);

                if (loggedInUser.Access == 1 || loggedInUser.Access == 3)
                {
                    return RedirectToAction("CollectIndex");
                }
                else
                {
                    if (id == null)
                        return RedirectToAction("DonationManagement");
                }
            }
            TDonationOrder d = db.TDonationOrders
                        .Include(t => t.Collect)
                        .FirstOrDefault(t => t.DonationOrderId == id);

            if (d == null)
                return RedirectToAction("DonationManagement");
            Rong_CDonationWrap dWrap = new Rong_CDonationWrap();
            dWrap.don = d;
            int collectAmount = d.Collect != null ? d.Collect.CollectAmount : 0;
            dWrap.CollectAmount = collectAmount;
            dWrap.CollectItemName = d.Collect != null ? d.Collect.CollectItemName : "";
            return View(dWrap);
        }
        [HttpPost]
        public IActionResult EditDonation(Rong_CDonationWrap dIn)
        {
            string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            if (userJson == null)
                return RedirectToAction("Login", "Home");

            TMember loggedInUser = JsonSerializer.Deserialize<TMember>(userJson);

            if (loggedInUser.Access == 0 || loggedInUser.Access == 2)
            {

                TDonationOrder dDb = db.TDonationOrders.FirstOrDefault(t => t.DonationOrderId == dIn.DonationOrderId);

                if (dDb != null)
                {   
                    //  把修改的捐贈數量加減回徵求數量
                    var collect = db.TCollects.FirstOrDefault(c => c.CollectId == dDb.CollectId);
                    if (collect != null) 
                    {
                        if (dIn.DonationAmount > dDb.DonationAmount) 
                        {
                            int newCollectAmount = collect.CollectAmount - (dIn.DonationAmount - dDb.DonationAmount);
                        }
                        else if (dIn.DonationAmount < dDb.DonationAmount)
                        {
                            int newCollectAmount = collect.CollectAmount + (dDb.DonationAmount - dIn.DonationAmount);
                            collect.CollectAmount = newCollectAmount;
                        }
                    }
                    dDb.DonarName = dIn.DonarName;
                    dDb.DonarPhone = dIn.DonarPhone;
                    dDb.DonationAmount = dIn.DonationAmount;
                    dDb.DonationStatus = dIn.DonationStatus;
                    db.SaveChanges();
                }

                return RedirectToAction("DonationManagement");
            }

            else
            {
                return RedirectToAction("CollectIndex");
            }
        }

        // 捐贈刪除
        public IActionResult DeleteDonationManagement(int? id)
        {

            if (id == null)
                return RedirectToAction("DonationManagement");
            TDonationOrder d = db.TDonationOrders.FirstOrDefault(t => t.DonationOrderId == id);
            if (d != null)
            {
                //  把刪除的捐贈數量加回徵求數量
                var collect = db.TCollects.FirstOrDefault(c => c.CollectId == d.CollectId);

                //  把刪除的運費加回會員點數
                var selectedDeliveryType = db.TDeliveryTypes.FirstOrDefault(dt => dt.DeliveryTypeId == d.DeliveryTypeId);
                var member = db.TMembers.FirstOrDefault(m => m.MemberId == d.MemberId);

                if (collect != null && selectedDeliveryType != null) 
                {
                    int newCollectAmount = collect.CollectAmount + d.DonationAmount;
                    collect.CollectAmount = newCollectAmount;

                    int deliveryFee = (int)selectedDeliveryType.DeliveryFee;
                    int newPoints = (int)member.Point + deliveryFee;
                    member.Point = newPoints;

                    db.TDonationOrders.Remove(d);
                    db.SaveChanges();
                }

            }
            return RedirectToAction("DonationManagement");
        }

        
    }
}
