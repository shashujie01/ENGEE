using Microsoft.AspNetCore.Mvc;
using EnGee.Models;
using EnGee.ViewModel;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EnGee.Controllers
{
    public class ShoppingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult List()
        {//僅供測試使用，不會上線，V已刪除
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
        {//檢視購物車內容
            List<SSJ_CShoppingCarItem> cart;
            if (!HttpContext.Session.Keys.Contains(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST))
            {
                cart = new List<SSJ_CShoppingCarItem>();
            }
            else
            {
                string json = HttpContext.Session.GetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST);
                cart = JsonSerializer.Deserialize<List<SSJ_CShoppingCarItem>>(json) ?? new List<SSJ_CShoppingCarItem>();
                Console.WriteLine(json);
            }
            return View(cart);
        }

        [HttpPost]
        public ActionResult CartView(SSJ_CAddToCartViewModel vm, int? deliverytypeid)
        {
            if (deliverytypeid.HasValue)
            {
                HttpContext.Session.SetInt32("DeliveryType", deliverytypeid.Value);
            }
            if (vm == null)
            {
                // 可以返回一些錯誤訊息或重定向到其他頁面
                return RedirectToAction("CartView"); // 假設回到首頁或其他適當的頁面
            }

            EngeeContext db = new EngeeContext();
            TProduct p = db.TProducts.FirstOrDefault(t => t.ProductId == vm.txtProductId);
            //db.TProducts.FirstOrDefault設定一個t變數， t.ProductId代表TProducts的ProductId，
            // t.ProductId == vm.txtProductId就是我從HTML取得txtProductId
            //並會丟到SSJ_CAddToCartViewModel 的txtProductId保存，
            //最後再賦值給t.ProductId ，也就是給TProducts的ProductId賦值
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
                item.DeliveryTypeID = (int)deliverytypeid; // 設定選擇的配送方式
                cart.Add(item);
            }

            json = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST, json);
            Console.WriteLine(HttpContext.Session);
            return RedirectToAction("CartView");
        }

        [HttpGet]
        public ActionResult QuickAddToCart(int productId)
        {//加入購物車快捷鍵
            // 設定預設值
            int defaultCount = 1;
            int defaultDeliveryOption = 1;
            HttpContext.Session.SetInt32("DeliveryType", defaultDeliveryOption);//預設值存入Session
            EngeeContext db = new EngeeContext();
            TProduct p = db.TProducts.FirstOrDefault(t => t.ProductId == productId);

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

            SSJ_CShoppingCarItem existingItem = cart.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.count += defaultCount; // 使用預設數量
            }
            else
            {
                SSJ_CShoppingCarItem item = new SSJ_CShoppingCarItem();
                item.point = (int)p.ProductUnitPoint;
                item.ProductId = productId;
                item.count = defaultCount; // 使用預設數量
                item.tproduct = p;
                item.ProductImagePath = $"/images/ProductImages/{p.ProductImagePath}";
                item.DeliveryTypeID = defaultDeliveryOption; // 使用預設的配送方式
                cart.Add(item);
            }

            json = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST, json);

            return RedirectToAction("CartView");
        }

        [HttpPost]
        public JsonResult AddToCartWithAjax(SSJ_CAddToCartViewModel vm, int deliveryOption)
        {//加入購物車，且不跳進購物車檢視
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
                item.DeliveryTypeID = deliveryOption; // 設定選擇的配送方式
                cart.Add(item);
            }
            json = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST, json);

            return Json(new { success = true, message = "已加入購物車" });
        }
        private int GetDeliveryFee(int deliveryTypeId)
        {
            using (var db = new EngeeContext())
            {
                var deliveryType = db.TDeliveryTypes.FirstOrDefault(dt => dt.DeliveryTypeId == deliveryTypeId);
                if (deliveryType != null)
                {
                    return (int)deliveryType.DeliveryFee;
                }
                // Default value if the delivery type is not found
                return 0;
            }
        }


        [HttpPost]
        public ActionResult ConfirmPurchase(SSJ_ConfirmPurchaseViewModel vm,string SelectedProducts)
        { // 檢查購物車中是否有商品
            if (!HttpContext.Session.Keys.Contains(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST))
            {
                return RedirectToAction("CartView");
            }
            int? sessionDeliveryType = HttpContext.Session.GetInt32("DeliveryType");
            if (sessionDeliveryType.HasValue)
            {
                vm.DeliveryType = sessionDeliveryType.Value;
            }
            // 將字符串反序列化為整數列表，Session中獲取CHKBox被選中的ID
            List<int> selectedProductIds = JsonSerializer.Deserialize<List<int>>(SelectedProducts);
            // 從Session中獲取購物車商品
            string json = HttpContext.Session.GetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST);
            List<SSJ_CShoppingCarItem> cart = JsonSerializer.Deserialize<List<SSJ_CShoppingCarItem>>(json);
            // 如果購物車為空，則重新導向到購物車視圖
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("CartView");
            }
            // 創建新的訂單並儲存
            int totalUsagePoints = 0;
            using (EngeeContext db = new EngeeContext())
            {

                TOrder order = new TOrder
                {
                    OrderDate = DateTime.Now,
                    DeliveryTypeId = vm.DeliveryType,
                    DeliveryAddress = vm.DeliveryAddress,
                    BuyerId = 59,//TODO:尚未串接
                    SellerId = 60,//尚未串接
                    OrderStatus = "3",//結帳後為3
                    OrderCatagory = 1,//買賣為1
                    ConvienenNum = "0",//超商尚未串接
                    DeliveryFee = GetDeliveryFee(vm.DeliveryType)//用函式尋找DeliveryFee
                };
                db.TOrders.Add(order);
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateException e)
                {
                    var innerException = e.InnerException;
                    while (innerException != null)
                    {
                        Console.WriteLine(innerException.Message);
                        innerException = innerException.InnerException;
                    }
                }

                // 對於購物車中的每一項商品，都在TOrderDetail表中新增一個詳細資料行
                foreach (var item in cart)
                {
                    if (selectedProductIds.Contains(item.ProductId))//chkbox選中的ID是否包含item.ProductId
                    {
                        TOrderDetail orderDetail = new TOrderDetail
                        {
                            OrderId = order.OrderId,
                            OrderDate = order.OrderDate,
                            ProductId = item.ProductId,
                            ProductUnitPoint = item.point,
                            OrderQuantity = item.count, 
                            BuyerId = order.BuyerId,
                            SellerId = order.SellerId
                        };
                        db.TOrderDetails.Add(orderDetail);
                        
                        totalUsagePoints += item.小計;// 計算小計
                    }
                }
                order.OrderTotalUsagePoints = totalUsagePoints;
                db.SaveChanges();

                // 購買後從Session中清除購物車
                cart.RemoveAll(item => selectedProductIds.Contains(item.ProductId));
                json = JsonSerializer.Serialize(cart);
                HttpContext.Session.SetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST, json);
                //HttpContext.Session.Remove(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST);
                //HttpContext.Session.Remove("DeliveryType");//清除寄送方式Session
                //TODO寄送方式是否跟別的一起傳
            }
            return RedirectToAction("CartView");
        }

        [HttpPost]
        public JsonResult UpdateCartItem(int productId, int newCount)
        {//讀取購物車的JSON，並利用ID找到要修改的資料(購買數量))
            List<SSJ_CShoppingCarItem> cart;

            string json = HttpContext.Session.GetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST);
            cart = JsonSerializer.Deserialize<List<SSJ_CShoppingCarItem>>(json) ?? new List<SSJ_CShoppingCarItem>();

            // 找到具有相應 productId 的項目
            var itemToUpdate = cart.FirstOrDefault(item => item.ProductId == productId);

            if (itemToUpdate != null)
            {
                // 更新數量
                itemToUpdate.count = newCount;

                // 序列化回 JSON 並保存到 Session
                json = JsonSerializer.Serialize(cart);
                HttpContext.Session.SetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST, json);

                Console.WriteLine($"Updated count for Product ID {productId} to {newCount}，{itemToUpdate.count} ");
                return Json(new { success = true, subtotalPoints = itemToUpdate.小計, itemToUpdate.count, totalPoints = cart.Sum(item => item.小計) });
            }
            else
            {
                Console.WriteLine($"false");
                return Json(new { success = false, message = "Product not found" });
            }
        }

        [HttpPost]
        public JsonResult DeleteCartItem(int productId)
        {
            try
            {
                List<SSJ_CShoppingCarItem> cart;
                string json = HttpContext.Session.GetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST);
                cart = JsonSerializer.Deserialize<List<SSJ_CShoppingCarItem>>(json) ?? new List<SSJ_CShoppingCarItem>();
                // 找到具有相應 productId 的項目
                var itemToRemove = cart.FirstOrDefault(item => item.ProductId == productId);
                if (itemToRemove != null)
                {
                    cart.Remove(itemToRemove);
                    json = JsonSerializer.Serialize(cart);
                    HttpContext.Session.SetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST, json);
                    return Json(new { success = true }); 
                }
                else
                {
                    return Json(new { success = false, message = "未找到商品" }); 
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "無法刪除：" + ex.Message }); 
            }
        }
    }
    }
