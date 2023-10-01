namespace EnGee.ViewModel
{
    public class SSJ_ShoppingListOrderViewModel
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public string OrderStatusDescription
        {
            get
            {
                return OrderStatus switch
                {
                    "1" => "待出貨",
                    "2" => "待收貨",
                    "3" => "訂單已完成",
                    "4" => "不成立",
                    "5" => "退貨/退款",
                    _ => "未知",
                };
            }
        }
        public int OrderCatagory { get; set; }
        public string OrderCatagoryDescription
        {
            get
            {
                return OrderCatagory switch
                {
                    1 => "購買",
                    2 => "一點送",
                };
            }
        }
        public int BuyerID { get; set; }

        //public string ConvienenNum { get; set; }
        public int DeliveryFee { get; set; }
        public int OrderTotalUsagePoints { get; set; }

        public string ReceiverName { get; set; }
        public string ReceiverTEL { get; set; }
    }
}
