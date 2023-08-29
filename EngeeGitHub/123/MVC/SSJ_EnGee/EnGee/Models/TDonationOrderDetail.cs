using System;
using System.Collections.Generic;

namespace EnGee.Models;

public partial class TDonationOrderDetail
{
    public int DonationOrderDetailId { get; set; }

    public int DonationOrderId { get; set; }

    public int ProductId { get; set; }

    public int DonateAmount { get; set; }

    public bool DonateItemStatus { get; set; }

    public virtual TDonationOrder DonationOrder { get; set; } = null!;

    public virtual TProduct Product { get; set; } = null!;
}
