using System;
using System.Collections.Generic;

namespace Engee.Models;

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

    public virtual TDeliveryType DeliveryType { get; set; } = null!;

    public virtual TMember Member { get; set; } = null!;

    public virtual ICollection<TCollectImage> TCollectImages { get; set; } = new List<TCollectImage>();

    public virtual ICollection<TCollectItem> TCollectItems { get; set; } = new List<TCollectItem>();
}
