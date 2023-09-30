using EnGee.Models;
using EnGee.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using prjEnGeeDemo.Models;
using prjEnGeeDemo.ViewModels;

namespace prjEnGeeDemo.Controllers
{
    public class ProductController : Controller
    {
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

            IQueryable<TProduct> query = db.TProducts.Where(p => p.ProductSaleStatus != 1 & p.DonationStatus==0);
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
            //為了加入購物車充新導回Details+ID的頁面
            if (TempData["RedirectToAction"] != null && TempData["RedirectToAction"].ToString() == "AddToCart")
            {//傳給前端
                ViewBag.RedirectToAction = "AddToCart";
            }

            // 從資料庫中獲取指定ID的商品，並且預先加載相關的資料
            var product = db.TProducts
                            .Include(p => p.Brand)
                            .Include(p => p.MainCategory)
                            .Include(p => p.Subcategory)
                            .Include(p => p.DeliveryType)
                            .SingleOrDefault(p => p.ProductId == id && p.ProductSaleStatus != 1);
            if (product == null || product.ProductSaleStatus == 1)
            {
                TempData["Message"] = "所選商品銷售一空";
                return RedirectToAction("IndexSSJ");
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
        [HttpGet]
        public IActionResult GetDeliveryTypeDetails(int value)
        {
            var result = db.TDeliveryTypes.FirstOrDefault(t => t.DeliveryTypeId == value);

            if (result != null)
            {
                return Json((int)result.DeliveryFee);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetProductSellerID(int value)
        {
            var product = await db.TProducts.FirstOrDefaultAsync(t => t.ProductId == value);
            if (product != null)
            {
                var member = await db.TMembers.FirstOrDefaultAsync(m => m.MemberId == product.SellerId);
                if (member != null)
                {
                    return Json(member.Username);
                }
                else
                {
                    return Json("查無賣家資料");
                }
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetDeliveryTypeIdForProduct(int productId)
        {
            var product = await db.TProducts.FirstOrDefaultAsync(t => t.ProductId == productId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }
            int deliveryTypeId = product.DeliveryTypeId;
            return Json(deliveryTypeId);
        }
        [HttpGet]
        public async Task<IActionResult> GetProductRemainingQuantityForProduct(int productId)
        {
            var product = await db.TProducts.FirstOrDefaultAsync(t => t.ProductId == productId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }
            int ProductRemainingQuantity = product.ProductRemainingQuantity;
            return Json(ProductRemainingQuantity);
        }
    }
}

