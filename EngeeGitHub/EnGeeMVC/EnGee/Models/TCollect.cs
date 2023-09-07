using System;
using System.Collections.Generic;

namespace EnGee.Models;

public partial class TCollect
{
    public int CollectId { get; set; }

    public int MemberId { get; set; }

    public string CollectTitle { get; set; } = null!;

    public string CollectCaption { get; set; } = null!;

    public DateTime CollectStartDate { get; set; }

    public DateTime CollectEndDate { get; set; }

    public int DeliveryTypeId { get; set; }

    public string? DeliveryAddress { get; set; }

    public string? ConvenienNum { get; set; }

    public bool CollectStatus { get; set; }

    public string? CollectImagePath { get; set; }

    public string CollectItemName { get; set; } = null!;

    public int MainCategoryId { get; set; }

    public int SubcategoryId { get; set; }

    public int CollectAmount { get; set; }

    public virtual TDeliveryType DeliveryType { get; set; } = null!;

    public virtual TMember Member { get; set; } = null!;

    public virtual ICollection<TDonationOrder> TDonationOrders { get; set; } = new List<TDonationOrder>();
}
