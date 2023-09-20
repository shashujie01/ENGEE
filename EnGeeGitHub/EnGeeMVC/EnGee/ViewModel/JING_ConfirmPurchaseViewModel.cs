using EnGee.Models;

namespace EnGee.ViewModel
{
    public class JING_ConfirmPurchaseViewModel
    {
        public List<JING_CDemandCartItem> Items { get; }

        public string DeliveryAddress { get; set; }
        public int DeliveryTypeID { get; set; }
        public int OrderTotalUsagePoints { get; set; }
        public int BuyerId { get; set; }
        public int SellerId { get; set; }
        public int DeliveryFee { get; set; }
        public int DeliveryType { get; set; }
    }
}
