using EnGee.Models;
using EnGee.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjEnGeeDemo.Models;
using prjEnGeeDemo.ViewModels;

namespace prjEnGeeDemo.Controllers
{
    public class ProductController : Controller
    {        
        //以下為樹傑的Action


        EngeeContext db = new EngeeContext();
        private IQueryable<TProduct> ApplyFilters(SSJ_ProductPageViewModel vm, IQueryable<TProduct> query)
        {//filters邏輯使用方法
            // 判斷關鍵字是否存在，若存在則過濾商品名稱
            if (!string.IsNullOrEmpty(vm.TxtKeyword))
            {
                query = query.Where(p => p.ProductName.Contains(vm.TxtKeyword));
            }
            // 判斷主、次分類是否存在，若存在則進行過濾
            if (vm.MainCategoryId.HasValue && vm.SubCategoryId.HasValue)
            {
                query = query.Where(p => p.MainCategoryId == vm.MainCategoryId.Value && p.SubcategoryId == vm.SubCategoryId.Value);
            }
            // 判斷品牌是否存在，若存在則進行過濾
            if (vm.BrandId.HasValue)
            {
                query = query.Where(p => p.BrandId == vm.BrandId.Value);
            }
            return query;
        }
        public IActionResult IndexSSJ(SSJ_ProductPageViewModel vm, int page = 1, int pageSize = 6)
        {// 產品頁面的主要邏輯
            // 從資料庫獲取所有商品
            IQueryable<TProduct> query = db.TProducts;
            // 應用過濾條件
            query = ApplyFilters(vm, query);
            // 計算總頁數與總數
            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // 使用單獨分頁函式
            var paginatedQuery = Paginate(query, page, pageSize);
            var products = paginatedQuery.Select(p => new SSJ_ProductViewModel
            {
                txtProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductDescribe = p.ProductDescribe,
                ProductImagePath = $"/images/ProductImages/{p.ProductImagePath}",
                ProductUnitPoint = p.ProductUnitPoint
            }).ToList();

            // return 給SSJ_ProductPageViewModel在前端顯示
            var model = new SSJ_ProductPageViewModel
            {
                Products = products,
                CurrentPage = page,
                TotalPages = totalPages,
                TxtKeyword = vm.TxtKeyword,
                MainCategoryId = vm.MainCategoryId,
                SubCategoryId = vm.SubCategoryId,
                BrandId = vm.BrandId
            };
            return View(model);
        }

        private IQueryable<TProduct> Paginate(IQueryable<TProduct> query, int page, int pageSize)
        {//分頁函式
            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }
        public IActionResult Details(int id, int txtCount, int deliverytypeid)
        {// 商品詳情頁面
            // 從資料庫中獲取指定ID的商品，並且預先加載相關的資料
            var product = db.TProducts
                            .Include(p => p.Brand)
                            .Include(p => p.MainCategory)
                            .Include(p => p.Subcategory)
                            .Include(p => p.DeliveryType)
                            .SingleOrDefault(p => p.ProductId == id);
            if (product == null)
            { // 若商品不存在，返回404
                return NotFound();
            }
            var PDviewModel = new SSJ_ProductDetailsViewModel
            {// 將商品的各種資訊填充到視圖模型中
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
            };
            return View(PDviewModel);
        }
        //以下好像用不到
        //public IActionResult Create()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public IActionResult Create(TProduct p)
        //{
        //    EngeeContext db = new EngeeContext();
        //    db.TProducts.Add(p);
        //    db.SaveChanges();
        //    return RedirectToAction("IndexSSJ");
        //}
    }
}

