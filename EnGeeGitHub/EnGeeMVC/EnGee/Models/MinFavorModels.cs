namespace EnGee.Models
{
    public class MinFavorModel : TProduct
    {
        private readonly TProduct _product;
        public MinFavorModel(TProduct product)
        {
            _product = product;
        }
        public int ProductId                //Min
        {
            get { return _product.ProductId; }
            set { _product.ProductId = value; }
        }


        public string ProductName          //Min
        {
            get { return _product.ProductName; }
            set { _product.ProductName = value; }
        }

        public int BrandId
        {
            get { return _product.BrandId; }
            set { _product.BrandId = value; }
        }

        public int MainCategoryId
        {
            get { return _product.MainCategoryId; }
            set { _product.MainCategoryId = value; }
        }

        public int SubcategoryId
        {
            get { return _product.SubcategoryId; }
            set { _product.SubcategoryId = value; }
        }

        public string ProductDescribe  //Min
        {
            get { return _product.ProductDescribe; }
            set { _product.ProductDescribe = value; }
        }

        public string ProductImagePath
        {
            get { return _product.ProductImagePath; }
            set { _product.ProductImagePath = value; }
        }

        public int ProductUnitPoint
        {
            get { return _product.ProductUnitPoint; }
            set { _product.ProductUnitPoint = value; }
        }

        public int ProductRemainingQuantity
        {
            get { return _product.ProductRemainingQuantity; }
            set { _product.ProductRemainingQuantity = value; }
        }

        public DateTime ProductExpirationDate
        {
            get { return _product.ProductExpirationDate; }
            set { _product.ProductExpirationDate = value; }
        }

        public string ProductUsageStatus
        {
            get { return _product.ProductUsageStatus; }
            set { _product.ProductUsageStatus = value; }
        }

        public int DonationStatus      //Min
        {
            get { return _product.DonationStatus; }
            set { _product.DonationStatus = value; }
        }

        public DateTime DateOfSale
        {
            get { return _product.DateOfSale; }
            set { _product.DateOfSale = value; }
        }
        public int DeliveryTypeId
        {
            get { return _product.DeliveryTypeId; }
            set { _product.DeliveryTypeId = value; }
        }

        public int SellerId
        {
            get { return _product.SellerId; }
            set { _product.SellerId = value; }
        }

        public int ProductSaleStatus
        {
            get { return _product.ProductSaleStatus; }
            set { _product.ProductSaleStatus = value; }
        }

        public int? boolFavor { get; set; }  //Min 新增 關聯至MinFavorController



    }
}
