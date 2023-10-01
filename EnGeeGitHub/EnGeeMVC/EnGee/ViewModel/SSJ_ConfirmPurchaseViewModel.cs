using EnGee.Models;

namespace EnGee.ViewModel
{
    public class SSJ_ConfirmPurchaseViewModel
    {
        //public List<SSJ_CShoppingCarItem> Items { get; }

        public int ProductId { get; set; }
        public int BuyerId { get; set; }
        public int SellerId { get; set; }
        public int count { get; set; }
        public int point { get; set; }
        public string ProductImagePath { get; set; } = null!;
        public int DeliveryTypeID { get; set; }
        public string DeliveryType
        {
            get
            {
                if (DeliveryTypeID == 1)
                {
                    return "宅配";
                }
                else if (DeliveryTypeID == 2)
                {
                    return "超商門市取貨";
                }
                else
                {
                    return "未知"; // 或其他預設值
                }
            }
        }
        public string DeliveryAddress { get; set; }
        public string DeliveryAddress_homeDelivery { get; set; }
        public string DeliveryAddress_storePickup { get; set; }
        public TProduct tproduct { get; set; }
        public int 小計
        {
            get
            {
                return this.count * this.point;
            }
        }
        public bool IsSelected { get; set; }
        public int OrderTotalUsagePoints { get; set; }
        public int DeliveryFee { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverTEL { get; set; }


    }
}
