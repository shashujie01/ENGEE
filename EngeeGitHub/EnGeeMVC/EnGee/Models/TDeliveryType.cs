using System;
using System.Collections.Generic;

namespace EnGee.Models;

public partial class TDeliveryType
{
    public int DeliveryTypeId { get; set; }

    public string DeliveryType { get; set; } = null!;

    public int? DeliveryFee { get; set; }

    public virtual ICollection<TCollect> TCollects { get; set; } = new List<TCollect>();

    public virtual ICollection<TDemand> TDemands { get; set; } = new List<TDemand>();

    public virtual ICollection<TDonationOrder> TDonationOrders { get; set; } = new List<TDonationOrder>();

    public virtual ICollection<TProduct> TProducts { get; set; } = new List<TProduct>();
}
