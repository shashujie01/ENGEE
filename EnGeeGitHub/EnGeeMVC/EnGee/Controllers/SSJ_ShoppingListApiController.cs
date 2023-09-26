using EnGee.Models;
using EnGee.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Linq;
using MailKit.Search;
using Microsoft.CodeAnalysis;
using Org.BouncyCastle.Asn1.X509;
using Microsoft.EntityFrameworkCore;


namespace EnGee.Controllers
{//訂單使用API沒做完
    [Route("api/[controller]")]
    [ApiController]
    public class SSJ_ShoppingListApiController : ControllerBase
    {
        private readonly EngeeContext _db;

        public SSJ_ShoppingListApiController(EngeeContext db)
        {
            _db = db;
        }

        [HttpGet("{id}")]
        public IActionResult GetOrder(int id)
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
                             //BuyerUsername = m.Username,
                             OrderStatus = o.OrderStatus,
                             OrderCatagory = o.OrderCatagory,
                             ConvienenNum = o.ConvienenNum,
                             DeliveryFee = o.DeliveryFee
                         }).FirstOrDefault();
            
            var orderDetails = (from orderDetail in _db.TOrderDetails
                                join Order in _db.TOrders on orderDetail.OrderId equals Order.OrderId
                                join product in _db.TProducts on orderDetail.ProductId equals product.ProductId
                                join member in _db.TMembers on orderDetail.SellerId equals member.MemberId
                                join deliveryType in _db.TDeliveryTypes on orderDetail.DeliveryTypeId equals deliveryType.DeliveryTypeId
                                where orderDetail.OrderId == id
                                select new SSJ_ShoppingListOrderDetailViewModel
                                {
                                    OrderID = Order.OrderId,
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
                                }).ToList();

            if (order == null)
            {
                return NotFound();
            }
            var result = new SSJ_ShoppingListCombinedViewModel
            {
                Orders = new List<SSJ_ShoppingListOrderViewModel> { order },
                OrderDetails = orderDetails
            };
            return Ok(result);
        }

        //[HttpPut]
        //public IActionResult UpdateOrder([FromBody] SSJ_ShoppingListCombinedViewModel model)

        [HttpPut("UpdateOrder/{id}")]
        public IActionResult UpdateOrder(int id, [FromBody] SSJ_ShoppingListCombinedViewModel model)
        {
            if (model == null || model.Orders == null || model.OrderDetails == null)
            {
                return BadRequest("Invalid data.");
            }
            var order = _db.TOrders.FirstOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound("Order not found.");
            }

            order.OrderDate = model.Orders.First().OrderDate;
            order.OrderTotalUsagePoints = model.Orders.First().OrderTotalUsagePoints;
            order.BuyerId = model.Orders.First().BuyerID;
            ////order.BuyerUsername = model.Orders.First().BuyerUsername;
            order.OrderStatus = model.Orders.First().OrderStatus;
            order.OrderCatagory = model.Orders.First().OrderCatagory;
            order.ConvienenNum = model.Orders.First().ConvienenNum;
            order.DeliveryFee = model.Orders.First().DeliveryFee;
            _db.Entry(order).State = EntityState.Modified;
            foreach (var detail in model.OrderDetails)
            {
                var orderDetail = _db.TOrderDetails.FirstOrDefault(od => od.OrderDetailId == detail.OrderDetailID);
                if (orderDetail == null)
                {
                    return NotFound($"Order detail with ID {detail.OrderDetailID} not found.");
                }
                orderDetail.ProductId = detail.ProductID;
                orderDetail.ProductUnitPoint = detail.ProductUnitPoint;
                orderDetail.OrderQuantity = detail.OrderQuantity;
                orderDetail.SellerId = detail.SellerID;
                orderDetail.DeliveryTypeId = detail.DeliveryTypeID;
                orderDetail.DeliveryAddress = detail.DeliveryAddress;
                //orderDetail.ProductName = detail.ProductName;
                //orderDetail.DeliveryType = detail.DeliveryType;
                //orderDetail.SellerUsername = detail.SellerUsername;
                //orderDetail.ProductImagePath = detail.ProductImagePath;
                _db.Entry(orderDetail).State = EntityState.Modified;
            }
            try
            {
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }

            return Ok(new { Message = "Order updated successfully." });
        }
    }
}

