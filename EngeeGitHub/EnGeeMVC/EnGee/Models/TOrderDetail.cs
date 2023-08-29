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

    public int BuyerId { get; set; }

    public int SellerId { get; set; }

    public virtual TMember Buyer { get; set; } = null!;

    public virtual TOrder Order { get; set; } = null!;

    public virtual TProduct Product { get; set; } = null!;

    public virtual TMember Seller { get; set; } = null!;
}
