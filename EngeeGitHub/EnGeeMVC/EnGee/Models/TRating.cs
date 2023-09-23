using System;
using System.Collections.Generic;

namespace EnGee.Models;

public partial class TRating
{
    public int RatingId { get; set; }

    public int OrderId { get; set; }

    public int BuyerId { get; set; }

    public int SellerId { get; set; }

    public int BuyerRating { get; set; }

    public int SellerRating { get; set; }

    public string BuyerComment { get; set; } = null!;

    public string SellerComment { get; set; } = null!;

    public DateTime BuyerRatingDate { get; set; }

    public DateTime SellerRatingDate { get; set; }

    public virtual TMember Buyer { get; set; } = null!;

    public virtual TMember Seller { get; set; } = null!;
}
