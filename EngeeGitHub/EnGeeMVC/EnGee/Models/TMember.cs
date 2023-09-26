using System;
using System.Collections.Generic;

namespace EnGee.Models;

public partial class TMember
{
    public int MemberId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Fullname { get; set; } = null!;

    public int Gender { get; set; }

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public DateTime? RegistrationDate { get; set; }

    public DateTime Birth { get; set; }

    public int? Access { get; set; }

    public double? Point { get; set; }

    public string? PhotoPath { get; set; }

    public string? Introduction { get; set; }

    public string? CharityProof { get; set; }

    public bool? IsValidCharity { get; set; }

    public virtual ICollection<TCase> TCases { get; set; } = new List<TCase>();

    public virtual ICollection<TCollect> TCollects { get; set; } = new List<TCollect>();

    public virtual ICollection<TDemand> TDemandDemanders { get; set; } = new List<TDemand>();

    public virtual ICollection<TDemand> TDemandGivers { get; set; } = new List<TDemand>();

    public virtual ICollection<TDonationOrder> TDonationOrders { get; set; } = new List<TDonationOrder>();

    public virtual ICollection<TMemberFavorite> TMemberFavorites { get; set; } = new List<TMemberFavorite>();

    public virtual ICollection<TMemberPoint> TMemberPoints { get; set; } = new List<TMemberPoint>();

    public virtual ICollection<TMessage> TMessages { get; set; } = new List<TMessage>();

    public virtual ICollection<TProduct> TProducts { get; set; } = new List<TProduct>();

    public virtual ICollection<TRating> TRatingBuyers { get; set; } = new List<TRating>();

    public virtual ICollection<TRating> TRatingSellers { get; set; } = new List<TRating>();

    public virtual ICollection<TShoppingCart> TShoppingCarts { get; set; } = new List<TShoppingCart>();
}
