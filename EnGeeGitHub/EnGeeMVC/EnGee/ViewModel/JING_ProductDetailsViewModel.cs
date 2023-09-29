using EnGee.Models;

namespace EnGee.ViewModel
{
    public class JING_ProductDetailsViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string BrandName { get; set; }
        public string MainCategoryName { get; set; }
        public string SubcategoryName { get; set; }
        public string ProductDescribe { get; set; }
        public string ProductImagePath { get; set; }
        public int ProductUnitPoint { get; set; }
        public int ProductRemainingQuantity { get; set; }
        public DateTime ProductExpirationDate { get; set; }
        public string ProductUsageStatus { get; set; }
        public int DonationStatus { get; set; }
        public DateTime DateOfSale { get; set; }
        public string DeliveryTypeName { get; set; }
        public decimal DeliveryFee { get; set; }
        public string SellerName { get; set; }
        public int ProductSaleStatus { get; set; }

        public List<TMessage> Messages { get; set; }

        public bool IsFavorite { get; set; }
        public int? LoggedInUserId { get; set; }
    }
}
