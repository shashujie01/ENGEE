using Microsoft.AspNetCore.Mvc;
using EnGee.Models;
using EnGee.ViewModel;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using prjMvcCoreDemo.Models;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace EnGee.Controllers
{
    public class ShoppingController : SuperController
    {//TODO購買的取貨方式、BuyerId、SellerId還沒串接
        private readonly EngeeContext _db;
        // 建構子，用於依賴注入
        public ShoppingController(EngeeContext db, CHI_CUserViewModel userViewModel) : base(userViewModel)
        {
            _db = db;////20230919合併出錯時加入
        }


        private TMember GetLoggedInUser()
        { // 從Session獲取當前登錄用戶
            string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            return JsonSerializer.Deserialize<TMember>(userJson);
        }

        private List<SSJ_CShoppingCarItem> GetCartItems()
        {// 從Session獲取購物車項目
            if (!HttpContext.Session.Keys.Contains(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST))
            {
                return new List<SSJ_CShoppingCarItem>();
            }
            string json = HttpContext.Session.GetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST);
            return JsonSerializer.Deserialize<List<SSJ_CShoppingCarItem>>(json) ?? new List<SSJ_CShoppingCarItem>();
        }

        private void SaveCartItems(List<SSJ_CShoppingCarItem> cart)
        { // 將購物車項目保存到Session
            string json = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST, json);
        }

        private int GetDeliveryFee(int deliveryTypeId)
        { // 根據送貨類型獲取運費
            using (var db = new EngeeContext())
            {
                var deliveryType = db.TDeliveryTypes.FirstOrDefault(dt => dt.DeliveryTypeId == deliveryTypeId);
                if (deliveryType != null)
                {
                    return (int)deliveryType.DeliveryFee;
                }
                // 若找不到送貨類型，則返回預設值
                return 0;
            }
        }

        private List<SSJ_CShoppingCarItem> GetAndUpdateCart(int productId, int count, int deliveryOption)
        { // 獲取或更新購物車資料
            
            var cart = GetCartItems();// 使用 GetCartItems 方法獲取購物車項目
            SSJ_CShoppingCarItem existingItem = cart.FirstOrDefault(i => i.ProductId == productId);
            TProduct p = _db.TProducts.FirstOrDefault(t => t.ProductId == productId);
            if (p != null)
            {
                if (existingItem != null)
                {// 如果商品已存在，更新數量
                    existingItem.count += count;
                }
                else
                {// 如果商品不存在，創建新項目並添加到購物車
                    SSJ_CShoppingCarItem item = new SSJ_CShoppingCarItem
                    {
                        point = (int)p.ProductUnitPoint,
                        ProductId = productId,
                        count = count,
                        tproduct = p,
                        ProductImagePath = $"/images/ProductImages/{p.ProductImagePath}",
                        DeliveryTypeID = deliveryOption
                    };
                    cart.Add(item);
                }
                // 保存更新後的購物車到 Session
                string json = JsonSerializer.Serialize(cart);
                HttpContext.Session.SetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST, json);
            }
            return cart;
        }
        //------------------------
        public override void OnActionExecuting(ActionExecutingContext context)
        {//// overview:
         //// 判斷目前執行的動作是否為特定Action，如果會員沒登入，紀錄原本要使用的action至session(Key:RedirectAfterLogin)、
         //// txtProductId(Key:TempProductId)、 deliverytypeid(Key:TempDeliverytypeid)、 txtCount(Key:TempTxtCount)，最後跳轉至登入頁面
            base.OnActionExecuting(context); // 調用基類的 OnActionExecuting 方法來進行基本驗證
            if (context.ActionDescriptor.DisplayName.Contains("QuickAddToCart"))
            {
                if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOINGED_USER))
                {
                    int txtProductId = (int)context.ActionArguments["txtProductId"];
                    HttpContext.Session.SetInt32("TempProductId", txtProductId);
                    HttpContext.Session.SetString("RedirectAfterLogin", context.ActionDescriptor.DisplayName); 
                    context.Result = new RedirectToActionResult("LoginLayout", "Home", null);
                }
            }
            else if (context.ActionDescriptor.DisplayName.Contains("CartView"))
            {
                if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOINGED_USER))
                {
                    HttpContext.Session.SetString("RedirectAfterLogin", context.ActionDescriptor.DisplayName); 
                    context.Result = new RedirectToActionResult("LoginLayout", "Home", null);
                }
            }
            else if (context.ActionDescriptor.DisplayName.Contains("AddToCartAndReturnCarView"))
            {
                if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOINGED_USER))
                {
                    if (context.ActionArguments.ContainsKey("txtProductId"))
                        {  HttpContext.Session.SetInt32("TempProductId", (int)context.ActionArguments["txtProductId"]);}
                    if (context.ActionArguments.ContainsKey("deliverytypeid")) 
                    {HttpContext.Session.SetInt32("TempDeliverytypeid", (int)context.ActionArguments["deliverytypeid"]);}
                    if (context.ActionArguments.ContainsKey("txtCount"))
                    { HttpContext.Session.SetInt32("TempTxtCount", (int)context.ActionArguments["txtCount"]); }
                    HttpContext.Session.SetString("RedirectAfterLogin", context.ActionDescriptor.DisplayName); 
                    context.Result = new RedirectToActionResult("LoginLayout", "Home", null);
                }
            }
            else if (context.ActionDescriptor.DisplayName.Contains("AddToCart"))
            {
                if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOINGED_USER))
                {
                    if (context.ActionArguments.ContainsKey("txtProductId"))
                    { HttpContext.Session.SetInt32("TempProductId", (int)context.ActionArguments["txtProductId"]); }
                    if (context.ActionArguments.ContainsKey("deliverytypeid"))
                    { HttpContext.Session.SetInt32("TempDeliverytypeid", (int)context.ActionArguments["deliverytypeid"]); }
                    if (context.ActionArguments.ContainsKey("txtCount"))
                    { HttpContext.Session.SetInt32("TempTxtCount", (int)context.ActionArguments["txtCount"]); }
                    HttpContext.Session.SetString("RedirectAfterLogin", context.ActionDescriptor.DisplayName);
                    context.Result = new JsonResult(new { needToLogin = true });
                }
            }
        }

        public IActionResult CartView()
        {// 顯示購物車畫面
            TMember loggedInUser = GetLoggedInUser();
            TMember userFromDatabase = _db.TMembers.FirstOrDefault(t => t.Email.Equals(loggedInUser.Email));
            ViewBag.userFromDatabase = userFromDatabase;
            List<SSJ_CShoppingCarItem> cart = GetCartItems();
            return View(cart);
        }

        public ActionResult AddToCart(SSJ_CShoppingCarItem vm ,int txtProductId, int txtCount, int deliverytypeid)
        {
            if (deliverytypeid!=0)
            {
                HttpContext.Session.SetInt32("DeliveryType", deliverytypeid);
            }
            if (vm == null)
            {
                return Json(new { success = false, message = "No product details provided." });
            }
            else//test
            {
                Console.WriteLine(vm.ProductId);//0
                Console.WriteLine(vm.count);//0
                Console.WriteLine(vm.DeliveryTypeID);//1
                Console.WriteLine(vm.DeliveryType);//宅配
                Console.WriteLine(vm.小計);//0
            }
            TProduct p = _db.TProducts.FirstOrDefault(t => t.ProductId == txtProductId);
            if (p == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }
            List<SSJ_CShoppingCarItem> cart = GetCartItems();
            SSJ_CShoppingCarItem existingItem = cart.FirstOrDefault(i => i.ProductId == txtProductId);
            if (existingItem != null)
            {
                existingItem.count += txtCount;
            }
            else
            {
                cart.Add(new SSJ_CShoppingCarItem
                {   tproduct = p,
                    point = (int)p.ProductUnitPoint,
                    ProductId = txtProductId,
                    count = txtCount,
                    ProductImagePath = $"/images/ProductImages/{p.ProductImagePath}",
                    DeliveryTypeID = deliverytypeid
                });
            }
            SaveCartItems(cart);
            return Json(new { success = true, message = "Product added to cart." });
        }

        public ActionResult AddToCartAndReturnCarView(SSJ_CShoppingCarItem vm,int txtProductId, int txtCount, int deliverytypeid)
        {//Detail中的直接購買(跳轉至cartview)
            if (deliverytypeid != 0)
            {
                HttpContext.Session.SetInt32("DeliveryType", deliverytypeid);
            }

            if (vm == null)
            {
                return Json(new { success = false, message = "No product details provided." });
            }
            else//test
            {
                Console.WriteLine(vm.ProductId);
                Console.WriteLine(vm.count);
                Console.WriteLine(vm.DeliveryTypeID);
                Console.WriteLine(vm.DeliveryType);
                Console.WriteLine(vm.小計);
            }

            TProduct p = _db.TProducts.FirstOrDefault(t => t.ProductId == txtProductId);
            if (p == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }

            List<SSJ_CShoppingCarItem> cart = GetCartItems();
            SSJ_CShoppingCarItem existingItem = cart.FirstOrDefault(i => i.ProductId == txtProductId);

            if (existingItem != null)
            {//檢查是否購物車內已經存在
                existingItem.count += (int)txtCount;
            }
            else
            {
                cart.Add(new SSJ_CShoppingCarItem
                {
                    tproduct = p,
                    point = (int)p.ProductUnitPoint,
                    ProductId = txtProductId,
                    count = txtCount,
                    ProductImagePath = $"/images/ProductImages/{p.ProductImagePath}",
                    DeliveryTypeID = (int)deliverytypeid
                });
            }
            SaveCartItems(cart);
            return RedirectToAction("CartView");
        }
      
        public ActionResult QuickAddToCart(int txtProductId)
        { // 購物頁商品快速加入商品到購物車
            // 設定預設的配送選項
            int defaultCount = 1;
            int defaultDeliveryOption = 1;
            HttpContext.Session.SetInt32("DeliveryType", defaultDeliveryOption);//不應該存入SESSION，應該直接對cart session操作
            // 用GetAndUpdateCart將商品加入購物車
            int productId = txtProductId;
            GetAndUpdateCart(productId, defaultCount, defaultDeliveryOption);
            return RedirectToAction("CartView");
        }

        [HttpPost]
        public ActionResult ConfirmPurchase(SSJ_ConfirmPurchaseViewModel vm,string SelectedProducts, int txtProductId, int txtCount, int deliverytypeid)
        {
            //確認結帳傳至後端
            var cart = GetCartItems();
            if (cart == null || cart.Count == 0) return RedirectToAction("CartView");// 檢查購物車中是否有商品
            int? sessionDeliveryType = HttpContext.Session.GetInt32("DeliveryType");
            if (sessionDeliveryType.HasValue)
            {
                vm.DeliveryType = sessionDeliveryType.Value;
            }
            // 將字符串反序列化為整數列表，Session中獲取CHKBox被選中的ID
            List<int> selectedProductIds = JsonSerializer.Deserialize<List<int>>(SelectedProducts);
            // 從Session中獲取購物車商品
            string json = HttpContext.Session.GetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST);
             cart = JsonSerializer.Deserialize<List<SSJ_CShoppingCarItem>>(json);
            // 如果購物車為空，則重新導向到購物車視圖
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("CartView");
            }
            else //test
            {
                    Console.WriteLine(cart);
            }

            // 創建新的訂單並儲存

            if (vm != null)//test
            {
                Console.WriteLine(vm.DeliveryType);
                Console.WriteLine(vm.DeliveryAddress);
            }

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
                //TODO 無法單獨清除選中CHKBOX的
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
        {//更新小計、totalPoint用
            // 從Session中獲取購物車列表
            var cart = GetCartItems();
            // 在購物車中尋找與指定ID匹配的商品
            var itemToUpdate = cart.FirstOrDefault(item => item.ProductId == productId);
            // 如果找不到該商品，返回失敗
            if (itemToUpdate == null) return Json(new { success = false, message = "Product not found" });
            // 更新商品數量
            itemToUpdate.count = newCount;
            // 保存新的購物車列表到Session
            SaveCartItems(cart);
            // 返回成功和更新後的資訊
            return Json(new { success = true, subtotalPoints = itemToUpdate.小計, itemToUpdate.count, totalPoints = cart.Sum(item => item.小計) });
        }
        [HttpPost]
        public JsonResult DeleteCartItem(int productId)
        {// 從購物車Session中刪除某一商品
            // 從Session中獲取購物車列表
            var cart = GetCartItems();
            // 在購物車中尋找與指定ID匹配的商品
            var itemToRemove = cart.FirstOrDefault(item => item.ProductId == productId);
            // 如果找不到該商品，返回失敗
            if (itemToRemove == null) return Json(new { success = false, message = "未找到商品" });
            // 從購物車中移除該商品
            cart.Remove(itemToRemove);
            // 保存新的購物車列表到Session
            SaveCartItems(cart);
            // 返回成功
            return Json(new { success = true });
        }
        //--------------墳墓--------------
        //[HttpPost] /*直接購買與加入購物車分離出CARTVIEW，待測驗OK刪除*/
        ////Details會post商品資料過來
        //public ActionResult CartView(SSJ_CAddToCartViewModel vm, int? deliverytypeid)
        //{
        //    if (deliverytypeid.HasValue)
        //    {
        //        HttpContext.Session.SetInt32("DeliveryType", deliverytypeid.Value);//TODO用處?
        //    }
        //    if (vm == null)
        //    { return RedirectToAction("CartView"); }

        //    TProduct p = _db.TProducts.FirstOrDefault(t => t.ProductId == vm.txtProductId);
        //    if (p == null)
        //    {
        //        return RedirectToAction("CartView");
        //    }
        //    List<SSJ_CShoppingCarItem> cart = GetCartItems();
        //    SSJ_CShoppingCarItem existingItem = cart.FirstOrDefault(i => i.ProductId == vm.txtProductId);
        //    if (existingItem != null)
        //    {
        //        existingItem.count += vm.txtCount;
        //    }
        //    else
        //    {
        //        cart.Add(new SSJ_CShoppingCarItem
        //        {
        //            point = (int)p.ProductUnitPoint,
        //            ProductId = vm.txtProductId,
        //            count = vm.txtCount,
        //            tproduct = p,
        //            ProductImagePath = $"/images/ProductImages/{p.ProductImagePath}",
        //            DeliveryTypeID = (int)deliverytypeid
        //        });
        //    }
        //    SaveCartItems(cart);
        //    return RedirectToAction("IndexSSJ", "Product");
        //}
        //-----------墳墓-------------
    }
}
