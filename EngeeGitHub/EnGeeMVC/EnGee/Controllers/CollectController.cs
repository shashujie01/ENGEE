using EnGee.Models;
using EnGee.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing.Printing;


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
        public IActionResult CollectIndex(Rong_keywordViewModel k, int mainId, int subId, int page = 1)
        {
            int pageSize = 12;
            int stillCollect = db.TCollects.Count(c => c.CollectStatus == true);
            int totalPage = (int)Math.Ceiling((double)stillCollect / pageSize);

            IEnumerable<Rong_CollectIndexViewModel> collectindex =
                (from co in db.TCollects
                 join m in db.TMembers on co.MemberId equals m.MemberId
                 where co.CollectStatus == true
                 select new Rong_CollectIndexViewModel
                 {
                     CollectId = co.CollectId,
                     Username = m.Username,
                     CollectTitle = co.CollectTitle,
                     CollectStartDate = co.CollectStartDate.ToString("yyyy/MM/dd"),
                     CollectEndDate = co.CollectEndDate.ToString("yyyy/MM/dd"),
                     CollectImagePath = co.CollectImagePath,
                     MainCategoryId = co.MainCategoryId,
                     SubcategoryId = co.SubcategoryId,
                     CollectStatus = co.CollectStatus,
                 })
                .Skip((page - 1) * pageSize).Take(pageSize).ToList();

            if (mainId != 0 || subId != 0)
            {
                collectindex = collectindex.Where(co => co.MainCategoryId == mainId || co.SubcategoryId == subId)
                    .Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }
            if (!string.IsNullOrEmpty(k.txtKeyword))
            {
                collectindex = collectindex.Where(i => i.CollectTitle.Contains(k.txtKeyword))
                    .Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }
            ViewData["CurrentPage"] = page;
            ViewData["TotalPage"] = totalPage;

            var mainca = db.TCosmeticMainCategories.ToList();
            var subca = db.TCosmeticSubcategories.ToList();
            ViewBag.MainCategory = mainca;
            ViewBag.Subcategory = subca;

            return View(collectindex);

        }

        // 許願詳細頁面
        public IActionResult CollectInfomation(int? id)
        {
            if (id == null)
                return RedirectToAction("CollectIndex");
            var cinfo =
                (from c in db.TCollects
                 join d in db.TDeliveryTypes on c.DeliveryTypeId equals d.DeliveryTypeId
                 join m in db.TMembers on c.MemberId equals m.MemberId into memberJoin
                 from m in memberJoin.DefaultIfEmpty()
                 where c.CollectId == id
                 select new Rong_CollectInfomationViewModel
                 {
                     CollectId = c.CollectId,
                     Username = m.Username,
                     CollectTitle = c.CollectTitle,
                     CollectCaption = c.CollectCaption,
                     CollectStartDate = c.CollectStartDate.ToString("yyyy/MM/dd"),
                     CollectEndDate = c.CollectEndDate.ToString("yyyy/MM/dd"),
                     DeliveryTypeId = c.DeliveryTypeId,
                     DeliveryType = d.DeliveryType,
                     DeliveryAddress = c.DeliveryAddress != null ? c.DeliveryAddress : "",
                     ConvenienNum = c.ConvenienNum != null ? c.ConvenienNum : "",
                     CollectImagePath = c.CollectImagePath,
                     CollectItemName = c.CollectItemName,
                     CollectAmount = c.CollectAmount
                 })
                .FirstOrDefault();

            return View(cinfo);
        }

        // 我要許願

        public IActionResult Create()
        {
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

            TCollect c = new TCollect()
            {
                MemberId = ci.MemberId,
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
            return RedirectToAction("CollectInfomation");
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

            return RedirectToAction("CollectInfomation");
        }

        // 捐贈表單
        public IActionResult Donate(int? id)
        {
            if (id == null)
                return RedirectToAction("CollectIndex");
            var doninfo =
                (from c in db.TCollects
                 join d in db.TDeliveryTypes on c.DeliveryTypeId equals d.DeliveryTypeId
                 join m in db.TMembers on c.MemberId equals m.MemberId into memberJoin
                 from m in memberJoin.DefaultIfEmpty()
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
            if (collect != null)
            {

                int newCollectAmount = collect.CollectAmount - d.DonationAmount;
                collect.CollectAmount = newCollectAmount;

                TDonationOrder don = new TDonationOrder()
                {
                    MemberId = d.MemberId,
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
            }
            return RedirectToAction("CollectIndex");
        }

        // 捐贈管理
        public IActionResult DonationManagement(Rong_keywordViewModel k)
        {
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
            if (id == null)
                return RedirectToAction("DonationManagement");
            TDonationOrder d = db.TDonationOrders.FirstOrDefault(t => t.DonationOrderId == id);
            if (d == null)
                return RedirectToAction("DonationManagement");
            Rong_CDonationWrap dWrap = new Rong_CDonationWrap();
            dWrap.don = d;
            return View(dWrap);
        }
        [HttpPost]
        public IActionResult EditDonation(Rong_CDonationWrap dIn)
        {
            TDonationOrder dDb = db.TDonationOrders.FirstOrDefault(t => t.DonationOrderId == dIn.DonationOrderId);

            if (dDb != null)
            {
                dDb.DonarName = dIn.DonarName;
                dDb.DonarPhone = dIn.DonarPhone;
                dDb.DonationAmount = dIn.DonationAmount;
                dDb.DonationStatus = dIn.DonationStatus;
                db.SaveChanges();
            }

            return RedirectToAction("DonationManagement");
        }
        // 捐贈刪除
        public IActionResult DeleteDonationManagement(int? id)
        {
            if (id == null)
                return RedirectToAction("DonationManagement");
            TDonationOrder d = db.TDonationOrders.FirstOrDefault(t => t.DonationOrderId == id);
            if (d != null)
            {
                db.TDonationOrders.Remove(d);
                db.SaveChanges();
            }
            return RedirectToAction("DonationManagement");
        }
    }
}
