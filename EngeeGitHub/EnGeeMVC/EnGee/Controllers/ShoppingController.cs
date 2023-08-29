using Microsoft.AspNetCore.Mvc;
using EnGee.Models;
using EnGee.ViewModel;
using System.Text.Json;

namespace EnGee.Controllers
{
    public class ShoppingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult List()
        {//僅供測試使用，不會上線
            EngeeContext db = new EngeeContext();
            var datas = from p in db.TProducts select p;
            List<SSJ_CProductWrap> list = new List<SSJ_CProductWrap>();
            foreach (var t in datas)
            {
                SSJ_CProductWrap w = new SSJ_CProductWrap();
                w.product = t;
                list.Add(w);
            }
            return View(list);
        }
        public IActionResult CartView()
        {
            List<SSJ_CShoppingCarItem> cart;
            if (!HttpContext.Session.Keys.Contains(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST))
            {
                cart = new List<SSJ_CShoppingCarItem>();
            }

            else
            {
                string json = HttpContext.Session.GetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST);
                cart = JsonSerializer.Deserialize<List<SSJ_CShoppingCarItem>>(json) ?? new List<SSJ_CShoppingCarItem>();
            }

            return View(cart);
        }
        public ActionResult AddToCart(int? id)
        {

            ViewBag.ProductId = id;
            return View();
        }
        [HttpPost]
        public ActionResult AddToCart(SSJ_CAddToCartViewModel vm, string deliveryOption)
        {
            if (vm == null)
            {
                return RedirectToAction("CartView");
            }

            EngeeContext db = new EngeeContext();
            TProduct p = db.TProducts.FirstOrDefault(t => t.ProductId == vm.txtProductId);

            if (p == null)
            {
                return RedirectToAction("CartView");
            }

            string json = "";
            List<SSJ_CShoppingCarItem> cart = null;

            if (HttpContext.Session.Keys.Contains(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST))
            {
                json = HttpContext.Session.GetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST);
                cart = JsonSerializer.Deserialize<List<SSJ_CShoppingCarItem>>(json);
            }
            else
            {
                cart = new List<SSJ_CShoppingCarItem>();
            }

            SSJ_CShoppingCarItem existingItem = cart.FirstOrDefault(i => i.ProductId == vm.txtProductId);

            if (existingItem != null)
            {
                existingItem.count += vm.txtCount; // 增加數量
            }
            else
            {
                SSJ_CShoppingCarItem item = new SSJ_CShoppingCarItem();
                item.point = (int)p.ProductUnitPoint;
                item.ProductId = vm.txtProductId;
                item.count = vm.txtCount;
                item.tproduct = p;
                item.ProductImagePath = $"/images/ProductImages/{p.ProductImagePath}";
                item.DeliveryOption = deliveryOption; // 設定選擇的配送方式
                cart.Add(item);
            }

            json = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST, json);

            return RedirectToAction("CartView");
        }


        [HttpPost]
        public JsonResult AddToCartWithAjax(SSJ_CAddToCartViewModel vm , string deliveryOption)
        {
            if (vm == null)
            {
                return Json(new { success = false, message = "加入購物車失敗" });
            }

            EngeeContext db = new EngeeContext();
            TProduct p = db.TProducts.FirstOrDefault(t => t.ProductId == vm.txtProductId);

            if (p == null)
            {
                return Json(new { success = false, message = "產品未找到" });
            }

            string json = "";
            List<SSJ_CShoppingCarItem> cart = null;

            if (HttpContext.Session.Keys.Contains(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST))
            {
                json = HttpContext.Session.GetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST);
                cart = JsonSerializer.Deserialize<List<SSJ_CShoppingCarItem>>(json);
            }
            else
            {
                cart = new List<SSJ_CShoppingCarItem>();
            }

            SSJ_CShoppingCarItem existingItem = cart.FirstOrDefault(i => i.ProductId == vm.txtProductId);

            if (existingItem != null)
            {
                existingItem.count += vm.txtCount; // 增加數量
            }
            else
            {
                SSJ_CShoppingCarItem item = new SSJ_CShoppingCarItem();
                item.point = (int)p.ProductUnitPoint;
                item.ProductId = vm.txtProductId;
                item.count = vm.txtCount;
                item.tproduct = p;
                item.ProductImagePath = $"/images/ProductImages/{p.ProductImagePath}";
                item.DeliveryOption = deliveryOption; // 設定選擇的配送方式
                cart.Add(item);
            }

            json = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST, json);

            return Json(new { success = true, message = "已加入購物車" });
        }
    }
}



