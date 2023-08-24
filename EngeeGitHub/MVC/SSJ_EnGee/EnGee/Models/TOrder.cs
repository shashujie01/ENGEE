using System;
using System.Collections.Generic;

namespace Engee.Models;

public partial class TOrder
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public int DeliveryTypeId { get; set; }

    public string DeliveryAddress { get; set; } = null!;

    public int OrderTotalUsagePoints { get; set; }

    public int BuyerId { get; set; }

    public int SellerId { get; set; }

    public string OrderStatus { get; set; } = null!;

    public int OrderCatagory { get; set; }

    public string ConvienenNum { get; set; } = null!;

    public int DeliveryFee { get; set; }

    public virtual TMember Buyer { get; set; } = null!;

    public virtual TDeliveryType DeliveryType { get; set; } = null!;

    public virtual TMember Seller { get; set; } = null!;

    public virtual ICollection<TOrderDetail> TOrderDetails { get; set; } = new List<TOrderDetail>();

    public virtual ICollection<TRating> TRatings { get; set; } = new List<TRating>();
}
