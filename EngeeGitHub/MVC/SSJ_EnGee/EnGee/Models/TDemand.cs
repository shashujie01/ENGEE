using System;
using System.Collections.Generic;

namespace EnGee.Models;

public partial class TDemand
{
    public int DemandId { get; set; }

    public int GiverId { get; set; }

    public int ProductId { get; set; }

    public int DemanderId { get; set; }

    public string DemandMessage { get; set; } = null!;

    public int DemandQuantity { get; set; }

    public DateTime DemandDate { get; set; }

    public int DeliveryTypeId { get; set; }

    public string ReceiverName { get; set; } = null!;

    public string ReceiverPhone { get; set; } = null!;

    public string ConvienenNum { get; set; } = null!;

    public string DeliveryAddress { get; set; } = null!;

    public virtual TDeliveryType DeliveryType { get; set; } = null!;

    public virtual TMember Demander { get; set; } = null!;

    public virtual TMember Giver { get; set; } = null!;

    public virtual TProduct Product { get; set; } = null!;
}
