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
        public IActionResult ShoppingList_admin(int page = 1)
        {
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
                                  BuyerUsername = member.Username, 
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
                                        ProductName = product.ProductName,
                                        ProductImagePath = $"/images/ProductImages/{product.ProductImagePath}",
                                        ProductUnitPoint = orderDetail.ProductUnitPoint,
                                        OrderQuantity = orderDetail.OrderQuantity,
                                        SellerID = orderDetail.SellerId,
                                        SellerUsername = member.Username,
                                        DeliveryTypeID = orderDetail.DeliveryTypeId,
                                        DeliveryType = deliveryType.DeliveryType,
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
            var order = _db.TOrders.Where(o => o.OrderId == id)
                       .Select(o => new SSJ_ShoppingListOrderViewModel { /*... map properties ...*/ })
                       .FirstOrDefault();

            var orderDetails = _db.TOrderDetails.Where(od => od.OrderId == id)
                       .Select(od => new SSJ_ShoppingListOrderDetailViewModel { /*... map properties ...*/ })
                       .ToList();

            var combinedModel = new SSJ_ShoppingListCombinedViewModel
            {
                Orders = new List<SSJ_ShoppingListOrderViewModel> { order },
                OrderDetails = orderDetails
            };

            return View(combinedModel);
        }

    }
}
