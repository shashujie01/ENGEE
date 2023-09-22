using System;
using System.Collections.Generic;

namespace EnGee.Models;

public partial class TProduct
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public int BrandId { get; set; }

    public int MainCategoryId { get; set; }

    public int SubcategoryId { get; set; }

    public string ProductDescribe { get; set; } = null!;

    public string ProductImagePath { get; set; } = null!;

    public int ProductUnitPoint { get; set; }

    public int ProductRemainingQuantity { get; set; }

    public DateTime ProductExpirationDate { get; set; }

    public string ProductUsageStatus { get; set; } = null!;

    public int DonationStatus { get; set; }

    public DateTime DateOfSale { get; set; }

    public int DeliveryTypeId { get; set; }

    public int SellerId { get; set; }

    public int ProductSaleStatus { get; set; }

    public int? BoolFavor { get; set; }

    public virtual TBrand Brand { get; set; } = null!;

    public virtual TDeliveryType DeliveryType { get; set; } = null!;

    public virtual TCosmeticMainCategory MainCategory { get; set; } = null!;

    public virtual TMember Seller { get; set; } = null!;

    public virtual TCosmeticSubcategory Subcategory { get; set; } = null!;

    public virtual ICollection<TCartsItem> TCartsItems { get; set; } = new List<TCartsItem>();

    public virtual ICollection<TDemand> TDemands { get; set; } = new List<TDemand>();

    public virtual ICollection<TMemberFavorite> TMemberFavorites { get; set; } = new List<TMemberFavorite>();

    public virtual ICollection<TMessage> TMessages { get; set; } = new List<TMessage>();
}
