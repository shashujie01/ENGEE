using EnGee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjEnGeeDemo.Models;
using prjMvcCoreDemo.Models;
using System.Text.Json;

namespace EnGee.Controllers
{
    public class NL_ProductController : SuperController
    {
        private IWebHostEnvironment _enviro = null;
        public NL_ProductController(IWebHostEnvironment p, CHI_CUserViewModel userViewModel) : base(userViewModel)
        {//20230919合併出錯時加入
            _enviro = p;
        }

        private TMember GetLoggedInUser()
        {
            string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            return JsonSerializer.Deserialize<TMember>(userJson);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List(string txtKeyword, int page = 1)
        {
            int pageSize = 10; // 每頁顯示的數量

            TMember loggedInUser = GetLoggedInUser();
            var userId = loggedInUser.MemberId;

            EngeeContext db = new EngeeContext();
            IQueryable<TProduct> datas = from p in db.TProducts
                                         select p;

            IQueryable<TProduct> productsQuery = datas.Include(p => p.Brand)
                                                      .Include(p => p.MainCategory)
                                                      .Include(p => p.Subcategory)
                                                      .Where(p => p.SellerId == userId);

            if (!string.IsNullOrEmpty(txtKeyword))
            {
                productsQuery = productsQuery.Where(p => p.ProductName.Contains(txtKeyword));
            }

            var totalProducts = productsQuery.Count();
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            var filteredProducts = productsQuery.Skip((page - 1) * pageSize)
                                                .Take(pageSize)
                                                .ToList();

            List<NL_ProductModel> list = new List<NL_ProductModel>();
            foreach (var t in filteredProducts)
            {
                NL_ProductModel w = new NL_ProductModel();
                w.Product = t;
                w.Brand = t.Brand;
                w.MainCategory = t.MainCategory;
                w.Subcategory = t.Subcategory;
                list.Add(w);
            }

            ViewBag.PageIndex = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.Keyword = txtKeyword;

            return View(list);
        }

        public IActionResult Add()
        {
            EngeeContext db = new EngeeContext();

            var brandCategories = db.TBrands.ToList();
            var MainCategory = db.TCosmeticMainCategories.ToList();
            var Subcategory = db.TCosmeticSubcategories.ToList();
            var DeliveryType = db.TDeliveryTypes.ToList();

            ViewBag.BrandCategories = brandCategories;
            ViewBag.MainCategory = MainCategory;
            ViewBag.Subcategory = Subcategory;
            ViewBag.TDeliveryType = DeliveryType;

            TMember loggedInUser = GetLoggedInUser();                           
            ViewBag.userId = loggedInUser.MemberId;               

            return View();           
        }
        [HttpPost]
        public IActionResult Add(NL_ProductModel p, IFormFile photo)
        {
            EngeeContext db = new EngeeContext();

            if (photo != null && photo.Length > 0)
            {
                string originalFileName = Path.GetFileName(photo.FileName);
                string fileExtension = Path.GetExtension(originalFileName);

                string photoName = Guid.NewGuid().ToString() + fileExtension;
                string path = _enviro.WebRootPath + "/images/ProductImages/" + photoName;
                p.photo.CopyTo(new FileStream(path, FileMode.Create));
                p.ProductImagePath = photoName;
            }
            db.TProducts.Add(p.Product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult AddOne()
        {
            EngeeContext db = new EngeeContext();

            var brandCategories = db.TBrands.ToList();
            var MainCategory = db.TCosmeticMainCategories.ToList();
            var Subcategory = db.TCosmeticSubcategories.ToList();
            var DeliveryType = db.TDeliveryTypes.ToList();

            ViewBag.BrandCategories = brandCategories;
            ViewBag.MainCategory = MainCategory;
            ViewBag.Subcategory = Subcategory;
            ViewBag.TDeliveryType = DeliveryType;

            TMember loggedInUser = GetLoggedInUser();
            ViewBag.userId = loggedInUser.MemberId;

            return View();
        }

        [HttpPost]
        public IActionResult AddOne(NL_ProductModel p, IFormFile photo)
        {
            EngeeContext db = new EngeeContext();

            if (photo != null && photo.Length > 0)
            {
                string originalFileName = Path.GetFileName(photo.FileName);
                string fileExtension = Path.GetExtension(originalFileName);

                string photoName = Guid.NewGuid().ToString() + fileExtension;
                string path = _enviro.WebRootPath + "/images/ProductImages/" + photoName;
                p.photo.CopyTo(new FileStream(path, FileMode.Create));
                p.ProductImagePath = photoName;
            }
            db.TProducts.Add(p.Product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
                return RedirectToAction("List");
            EngeeContext db = new EngeeContext();
            TProduct prod = db.TProducts.FirstOrDefault(t => t.ProductId == id);
            if (prod != null)
            {
                db.TProducts.Remove(prod);
                db.SaveChanges();
            }
            return RedirectToAction("List");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("List");
            EngeeContext db = new EngeeContext();

            var brandCategories = db.TBrands.ToList();
            var MainCategory = db.TCosmeticMainCategories.ToList();
            var Subcategory = db.TCosmeticSubcategories.ToList();
            var DeliveryType = db.TDeliveryTypes.ToList();

            ViewBag.BrandCategories = brandCategories;
            ViewBag.MainCategory = MainCategory;
            ViewBag.Subcategory = Subcategory;
            ViewBag.TDeliveryType = DeliveryType;

            TProduct prod = db.TProducts.FirstOrDefault(t => t.ProductId == id);
            if (prod == null)
                return RedirectToAction("List");
            NL_ProductModel NLPM = new NL_ProductModel();
            NLPM.Product = prod;
            return View(NLPM);
        }
        [HttpPost]
        public IActionResult Edit(NL_ProductModel prodIn, IFormFile photo)
        {
            EngeeContext db = new EngeeContext();
            TProduct prodDb = db.TProducts.FirstOrDefault(t => t.ProductId == prodIn.ProductId);

            if (prodDb != null)
            {
                if (photo != null && photo.Length > 0)
                {
                    string originalFileName = Path.GetFileName(photo.FileName);
                    string fileExtension = Path.GetExtension(originalFileName);

                    string photoName = Guid.NewGuid().ToString() + fileExtension;
                    string path = _enviro.WebRootPath + "/images/ProductImages/" + photoName;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        photo.CopyTo(stream);
                    }
                    prodDb.ProductImagePath = photoName;
                }

                prodDb.ProductName = prodIn.ProductName;
                prodDb.BrandId = prodIn.BrandId;
                prodDb.MainCategoryId = prodIn.MainCategoryId;
                prodDb.SubcategoryId = prodIn.SubcategoryId;
                prodDb.ProductDescribe = prodIn.ProductDescribe;
                prodDb.ProductUnitPoint = prodIn.ProductUnitPoint;
                prodDb.ProductRemainingQuantity = prodIn.ProductRemainingQuantity;
                prodDb.ProductExpirationDate = prodIn.ProductExpirationDate;
                prodDb.ProductUsageStatus = prodIn.ProductUsageStatus;
                prodDb.DonationStatus = prodIn.DonationStatus;
                prodDb.DateOfSale = prodIn.DateOfSale;
                prodDb.DeliveryTypeId = prodIn.DeliveryTypeId;
                prodDb.SellerId = prodIn.SellerId;
                prodDb.ProductSaleStatus = prodIn.ProductSaleStatus;

                db.SaveChanges();
            }
            return RedirectToAction("List");
        }
    }
}
