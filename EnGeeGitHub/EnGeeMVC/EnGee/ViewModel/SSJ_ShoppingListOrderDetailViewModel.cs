namespace EnGee.ViewModel
{
    public class SSJ_ShoppingListOrderDetailViewModel
    {
        public int OrderID { get; set; }
        public string ProductName { get; set; }
        public string ProductImagePath { get; set; }
        public int OrderDetailID { get; set; }
        public int ProductID { get; set; }
        public int ProductUnitPoint { get; set; }
        public int OrderQuantity { get; set; }
        public int SellerID { get; set; }
        public string SellerUsername { get; set; }
        public int DeliveryTypeID { get; set; }
        public string DeliveryType { get; set; }
        public string DeliveryAddress { get; set; }
    }
}
