using EnGee.Models;
using EnGee.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjEnGeeDemo.Models;
using prjEnGeeDemo.ViewModels;
using prjMvcCoreDemo.Models;
using System.Text.Json;

namespace EnGee.Controllers
{
    public class JING_ProductController : Controller
    {
        private IWebHostEnvironment _enviro = null;
        public JING_ProductController(IWebHostEnvironment p)
        {
            _enviro = p;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List(string txtKeyword, int page = 1)
        {
            int pageSize = 10; // 每頁顯示的數量
            EngeeContext db = new EngeeContext();
            IQueryable<TProduct> datas = from p in db.TProducts
                                         select p;

            IQueryable<TProduct> productsQuery = datas.Include(p => p.Brand)
                                                      .Include(p => p.MainCategory)
                                                      .Include(p => p.Subcategory);

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
                string path = _enviro.WebRootPath + "/images/product/" + photoName;
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
                string path = _enviro.WebRootPath + "/images/product/" + photoName;
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
                    string path = _enviro.WebRootPath + "/images/product/" + photoName;
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

        //以下為樹傑的Action

        EngeeContext db = new EngeeContext();
        private IQueryable<TProduct> ApplyFilters(JING_ProductPageViewModel vm, IQueryable<TProduct> query)
        {//filters邏輯使用方法
            if (!string.IsNullOrEmpty(vm.txtKeyword))
            {
                query = query.Where(p => p.ProductName.Contains(vm.txtKeyword));
            }
            if (vm.MainCategoryId.HasValue && vm.SubCategoryId.HasValue)
            {
                query = query.Where(p => p.MainCategoryId == vm.MainCategoryId.Value && p.SubcategoryId == vm.SubCategoryId.Value);
            }
            if (vm.BrandId.HasValue)
            {
                query = query.Where(p => p.BrandId == vm.BrandId.Value);
            }
            return query;
        }
        public IActionResult IndexJING(JING_ProductPageViewModel vm, int page = 1, int pageSize = 6)
        {
            var products = db.TProducts
.Where(p => p.DonationStatus == 1) // 添加篩選條件
.Select(p => new JING_ProductViewModel

{
    ProductId = p.ProductId,
    ProductName = p.ProductName,
    ProductDescribe = p.ProductDescribe,
    ProductImagePath = $"/images/ProductImages/{p.ProductImagePath}",
    ProductUnitPoint = p.ProductUnitPoint
});



            // Pagination
            var totalCount = products.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            products = products.Skip((page - 1) * pageSize).Take(pageSize);

            var model = new JING_ProductPageViewModel
            {
                Products = products.ToList(),
                CurrentPage = page,
                TotalPages = totalPages,
                txtKeyword = vm.txtKeyword,
                MainCategoryId = vm.MainCategoryId,
                SubCategoryId = vm.SubCategoryId,
                BrandId = vm.BrandId
            };
            return View(model);
        }

        public IActionResult DetailsJING(int id)
        {
            // 首先，取得當前用戶的ID
            int currentMemberId = GetCurrentMemberId();

            // 如果用戶未登入，則重定向到登錄頁面或其他適當的地方
            if (currentMemberId == -1)
            {
                return RedirectToAction("Login", "Home"); // 登入控制器名稱
            }

            // 接下來，取得商品詳情
            var product = db.TProducts
                            .Include(p => p.Brand)
                            .Include(p => p.MainCategory)
                            .Include(p => p.Subcategory)
                            .Include(p => p.DeliveryType)
                            .SingleOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            var PDviewModel = new JING_ProductDetailsViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                BrandName = product.Brand?.BrandName,
                MainCategoryName = product.MainCategory?.MainCategory,
                SubcategoryName = product.Subcategory?.Subcategory,
                ProductDescribe = product.ProductDescribe,
                ProductImagePath = $"/images/ProductImages/{product.ProductImagePath}",
                ProductUnitPoint = product.ProductUnitPoint,
                ProductRemainingQuantity = product.ProductRemainingQuantity,
                ProductExpirationDate = product.ProductExpirationDate,
                ProductUsageStatus = product.ProductUsageStatus,
                DonationStatus = product.DonationStatus,
                DateOfSale = product.DateOfSale,
                DeliveryTypeName = product.DeliveryType?.DeliveryType,
                DeliveryFee = product.DeliveryType?.DeliveryFee ?? 0,
                //SellerName = product.Seller?.MemberName,
                ProductSaleStatus = product.ProductSaleStatus,
                LoggedInUserId = currentMemberId == -1 ? (int?)null : currentMemberId // 新增這一行
            };
            PDviewModel.Messages = db.TMessages
                                  .Include(m => m.Member)  // 包含會員信息
                                  .Where(m => m.ProductId == id)
                                  .ToList();


            // 使用currentMemberId檢查商品是否已被當前用戶加入「我的最愛」
            PDviewModel.IsFavorite = db.TMemberFavorites
                                     .Any(f => f.MemberId == currentMemberId && f.ProductId == id);

            return View(PDviewModel);
        }

        private int GetCurrentMemberId()
        {
            string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            if (string.IsNullOrEmpty(userJson))
            {
                return -1;
            }

            TMember loggedInUser = JsonSerializer.Deserialize<TMember>(userJson);
            return loggedInUser.MemberId;
        }
    
    }
}
