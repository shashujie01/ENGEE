using Microsoft.AspNetCore.Mvc;
using EnGee.Models;
using EnGee.ViewModel;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using prjMvcCoreDemo.Models;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using System;

namespace EnGee.Controllers
{
    public class SSJ_ShoppingListController : Controller
    {
        private readonly EngeeContext _db;

        public SSJ_ShoppingListController(EngeeContext db)
        {
            _db = db;
        }

        private TMember GetLoggedInUser()
        { // 從Session獲取當前登錄用戶
            string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            return JsonSerializer.Deserialize<TMember>(userJson);
        }
        public IActionResult ShoppingList_admin(int page = 1)
        {
            var user = GetLoggedInUser();
            int loggedUserId = user.MemberId;

            var memberForAccess = _db.TMembers.SingleOrDefault(m => m.MemberId== loggedUserId);
            if ( memberForAccess.Access != 0)
            {
                return RedirectToAction("Login", "Home");
            }

            int pageSize = 12; // 每頁顯示的數量

            // 取得tOrders的資料
            var ordersQuery = from order in _db.TOrders
                              join member in _db.TMembers on order.BuyerId equals member.MemberId
                              select new SSJ_ShoppingListOrderViewModel
                              {
                                  OrderID = order.OrderId,
                                  OrderDate = order.OrderDate,
                                  OrderTotalUsagePoints = order.OrderTotalUsagePoints,
                                  BuyerID = order.BuyerId,
                                  //BuyerUsername = member.Username, 
                                  OrderStatus = order.OrderStatus,
                                  OrderCatagory = order.OrderCatagory,
                                  ConvienenNum = order.ConvienenNum,
                                  DeliveryFee = order.DeliveryFee
                              };

            // 取得tOrderDetail的資料
            var orderDetailsQuery = from orderDetail in _db.TOrderDetails
                                    join product in _db.TProducts on orderDetail.ProductId equals product.ProductId
                                    join member in _db.TMembers on orderDetail.SellerId equals member.MemberId
                                    join deliveryType in _db.TDeliveryTypes on orderDetail.DeliveryTypeId equals deliveryType.DeliveryTypeId
                                    select new SSJ_ShoppingListOrderDetailViewModel
                                    {
                                        OrderID = orderDetail.OrderId,
                                        OrderDetailID = orderDetail.OrderDetailId,
                                        ProductID = orderDetail.ProductId,
                                        ////ProductName = product.ProductName,
                                        //ProductImagePath = $"/images/ProductImages/{product.ProductImagePath}",
                                        ProductUnitPoint = orderDetail.ProductUnitPoint,
                                        OrderQuantity = orderDetail.OrderQuantity,
                                        SellerID = orderDetail.SellerId,
                                        //SellerUsername = member.Username,
                                        DeliveryTypeID = orderDetail.DeliveryTypeId,
                                        //DeliveryType = deliveryType.DeliveryType,
                                        DeliveryAddress = orderDetail.DeliveryAddress
                                    };

            var ordersList = ordersQuery.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var orderDetailsList = orderDetailsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var combinedModel = new SSJ_ShoppingListCombinedViewModel
            {
                Orders = ordersList,
                OrderDetails = orderDetailsList
            };

            ViewBag.PageIndex = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(ordersList.Count / (double)pageSize); // 注意這裡改成只用ordersList的Count

            return View(combinedModel);
        }

          public IActionResult ShoppingList_member (int page = 1)
        {
            var user = GetLoggedInUser();
            int loggedUserId = user.MemberId;

            if (loggedUserId==null)
            {
                return RedirectToAction("Login", "Home");
            }

            int pageSize = 12; // 每頁顯示的數量

            // 取得tOrders的資料
            var ordersQuery = from order in _db.TOrders
                              where order.BuyerId == loggedUserId
                              join member in _db.TMembers on order.BuyerId equals member.MemberId
                              select new SSJ_ShoppingListOrderViewModel
                              {
                                  OrderID = order.OrderId,
                                  OrderDate = order.OrderDate,
                                  OrderTotalUsagePoints = order.OrderTotalUsagePoints,
                                  BuyerID = order.BuyerId,
                                  ///*/*/*BuyerUsern*/*/*/ame = member.Username, 
                                  OrderStatus = order.OrderStatus,
                                  OrderCatagory = order.OrderCatagory,
                                  ConvienenNum = order.ConvienenNum,
                                  DeliveryFee = order.DeliveryFee
                              };
            var orderIds = ordersQuery.Select(o => o.OrderID).ToList();
            // 取得tOrderDetail的資料
            var orderDetailsQuery = from orderDetail in _db.TOrderDetails
                                    where orderIds.Contains(orderDetail.OrderId)
                                    join product in _db.TProducts on orderDetail.ProductId equals product.ProductId
                                    join member in _db.TMembers on orderDetail.SellerId equals member.MemberId
                                    join deliveryType in _db.TDeliveryTypes on orderDetail.DeliveryTypeId equals deliveryType.DeliveryTypeId
                                    select new SSJ_ShoppingListOrderDetailViewModel
                                    {
                                        OrderID = orderDetail.OrderId,
                                        OrderDetailID = orderDetail.OrderDetailId,
                                        ProductID = orderDetail.ProductId,
                                        ////ProductName = product.ProductName,
                                        ////ProductImagePath = $"/images/ProductImages/{product.ProductImagePath}",
                                        ProductUnitPoint = orderDetail.ProductUnitPoint,
                                        OrderQuantity = orderDetail.OrderQuantity,
                                        SellerID = orderDetail.SellerId,
                                        //SellerUsername = member.Username,
                                        DeliveryTypeID = orderDetail.DeliveryTypeId,
                                        //DeliveryType = deliveryType.DeliveryType,
                                        DeliveryAddress = orderDetail.DeliveryAddress
                                    };

            var ordersList = ordersQuery.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var orderDetailsList = orderDetailsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var combinedModel = new SSJ_ShoppingListCombinedViewModel
            {
                Orders = ordersList,
                OrderDetails = orderDetailsList
            };

            ViewBag.PageIndex = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(ordersList.Count / (double)pageSize); // 注意這裡改成只用ordersList的Count

            return View(combinedModel);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return RedirectToAction("ShoppingList_admin");

            TOrder OrderId = _db.TOrders.FirstOrDefault(t => t.OrderId == id);
            if (OrderId != null)
            {
                var orderDetails = _db.TOrderDetails.Where(od => od.OrderId == id).ToList();
                _db.TOrderDetails.RemoveRange(orderDetails);
                _db.TOrders.Remove(OrderId);
                _db.SaveChanges();
            }
            return RedirectToAction("ShoppingList_admin");
        }


        public IActionResult SSJ_EditOrder(int id)
        {
            var order = (from o in _db.TOrders
                         join m in _db.TMembers on o.BuyerId equals m.MemberId
                         where o.OrderId == id
                         select new SSJ_ShoppingListOrderViewModel
                         {
                             OrderID = o.OrderId,
                             OrderDate = o.OrderDate,
                             OrderTotalUsagePoints = o.OrderTotalUsagePoints,
                             BuyerID = o.BuyerId,
                             //////BuyerUsername = m.Username,
                             OrderStatus = o.OrderStatus,
                             OrderCatagory = o.OrderCatagory,
                             ConvienenNum = o.ConvienenNum,
                             DeliveryFee = o.DeliveryFee
                         }).FirstOrDefault();

            var orderDetails = (from orderDetail in _db.TOrderDetails
                                join product in _db.TProducts on orderDetail.ProductId equals product.ProductId
                                join member in _db.TMembers on orderDetail.SellerId equals member.MemberId
                                join deliveryType in _db.TDeliveryTypes on orderDetail.DeliveryTypeId equals deliveryType.DeliveryTypeId
                                where orderDetail.OrderId == id
                                select new SSJ_ShoppingListOrderDetailViewModel
                                {
                                    OrderID = orderDetail.OrderId,
                                    OrderDetailID = orderDetail.OrderDetailId,
                                    ProductID = orderDetail.ProductId,
                                    ////ProductName = product.ProductName,
                                    ////ProductImagePath = $"/images/ProductImages/{product.ProductImagePath}",
                                    ProductUnitPoint = orderDetail.ProductUnitPoint,
                                    OrderQuantity = orderDetail.OrderQuantity,
                                    SellerID = orderDetail.SellerId,
                                    //SellerUsername = member.Username,
                                    DeliveryTypeID = orderDetail.DeliveryTypeId,
                                    //DeliveryType = deliveryType.DeliveryType,
                                    DeliveryAddress = orderDetail.DeliveryAddress
                                }).ToList();

            var combinedModel = new SSJ_ShoppingListCombinedViewModel
            {
                Orders = new List<SSJ_ShoppingListOrderViewModel> { order },
                OrderDetails = orderDetails
            };

            return View(combinedModel);
        }
        [HttpPost]
        public IActionResult SSJ_EditOrder(SSJ_ShoppingListCombinedViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}
            if (model == null || model.Orders == null || !model.Orders.Any())
            {
                // Handle the error, maybe return an error view or throw an exception.
            }
            // Fetch the corresponding order from the database
            var orderInDb = _db.TOrders.Find(model.Orders.FirstOrDefault().OrderID);
            if (orderInDb != null)
            {
                orderInDb.OrderId = model.Orders.FirstOrDefault().OrderID;
                orderInDb.OrderDate = model.Orders.FirstOrDefault().OrderDate;
                orderInDb.OrderTotalUsagePoints = model.Orders.FirstOrDefault().OrderTotalUsagePoints;
                orderInDb.BuyerId = model.Orders.FirstOrDefault().BuyerID;
                orderInDb.OrderStatus = model.Orders.FirstOrDefault().OrderStatus;
                orderInDb.OrderCatagory = model.Orders.FirstOrDefault().OrderCatagory;
                orderInDb.ConvienenNum = model.Orders.FirstOrDefault().ConvienenNum;
                orderInDb.DeliveryFee = model.Orders.FirstOrDefault().DeliveryFee;
            }

            // Update each order detail
            foreach (var orderDetailModel in model.OrderDetails)
            {
                var orderDetailInDb = _db.TOrderDetails.Find(orderDetailModel.OrderDetailID);
                if (orderDetailInDb != null)
                {
                    orderDetailInDb.ProductId = orderDetailModel.ProductID;
                    orderDetailInDb.ProductUnitPoint = orderDetailModel.ProductUnitPoint;
                    orderDetailInDb.OrderQuantity = orderDetailModel.OrderQuantity;
                    orderDetailInDb.SellerId = orderDetailModel.SellerID;
                    orderDetailInDb.DeliveryTypeId = orderDetailModel.DeliveryTypeID;
                    orderDetailInDb.DeliveryAddress = orderDetailModel.DeliveryAddress;
                }
            }

            // Save changes to the database
            _db.SaveChanges();

            // Redirect to a confirmation page
            return RedirectToAction("ShoppingList_admin");
        }
    }
}
