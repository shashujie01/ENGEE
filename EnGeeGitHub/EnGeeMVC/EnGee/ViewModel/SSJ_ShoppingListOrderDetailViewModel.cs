namespace EnGee.ViewModel
{
    public class SSJ_ShoppingListOrderDetailViewModel
    {
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public int ProductID { get; set; }
        public int ProductUnitPoint { get; set; }
        public int OrderQuantity { get; set; }
        public int SellerID { get; set; }
        public int DeliveryTypeID { get; set; }
        public string DeliveryAddress { get; set; }
    }
}
