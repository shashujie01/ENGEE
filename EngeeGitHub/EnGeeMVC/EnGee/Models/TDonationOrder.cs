using System;
using System.Collections.Generic;

namespace EnGee.Models;

public partial class TDonationOrder
{
    public int DonationOrderId { get; set; }

    public int MemberId { get; set; }

    public int CollectId { get; set; }

    public DateTime OrderDate { get; set; }

    public int DeliveryTypeId { get; set; }

    public string DonarName { get; set; } = null!;

    public string DonarPhone { get; set; } = null!;

    public string DonationStatus { get; set; } = null!;

    public int DonationAmount { get; set; }

    public virtual TCollect Collect { get; set; } = null!;

    public virtual TDeliveryType DeliveryType { get; set; } = null!;

    public virtual TMember Member { get; set; } = null!;
}
