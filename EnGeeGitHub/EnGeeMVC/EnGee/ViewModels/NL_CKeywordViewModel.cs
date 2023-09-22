namespace EnGee.ViewModels
{
    public class NL_CKeywordViewModel
    {
        public string txtKeyword { get; set; }
        public string searchBy { get; set; }
        public string BrandId { get; set; }
        public string MainCategoryId { get; set; }
        public string DeliveryTypeId { get; set; }
        public int? DonationStatus { get; set; } // 0:非捐贈 1:捐贈
        public int? ProductSaleStatus { get; set; } // 1:下架 2:上架 3:訂單成立
    }
}
