using System;
using System.Collections.Generic;

namespace EnGee.Models;

public partial class TOrder
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public int OrderTotalUsagePoints { get; set; }

    public int BuyerId { get; set; }

    public string OrderStatus { get; set; } = null!;

    public int OrderCatagory { get; set; }

    public string ConvienenNum { get; set; } = null!;

    public int DeliveryFee { get; set; }

    public string? ReceiverName { get; set; }

    public string? ReceiverTel { get; set; }
}
