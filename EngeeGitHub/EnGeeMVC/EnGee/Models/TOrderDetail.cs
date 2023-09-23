using System;
using System.Collections.Generic;

namespace EnGee.Models;

public partial class TOrderDetail
{
    public int OrderDetailId { get; set; }

    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public int ProductId { get; set; }

    public int ProductUnitPoint { get; set; }

    public int OrderQuantity { get; set; }

    public int SellerId { get; set; }

    public int DeliveryTypeId { get; set; }

    public string DeliveryAddress { get; set; } = null!;
}
