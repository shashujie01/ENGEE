namespace EnGee.Models
{
    public class SSJ_CProductWrap
    {
        private TProduct _product;
        public TProduct product
        {
            get { return _product; }
            set { _product = value; }
        }
        public SSJ_CProductWrap()
        {
            _product = new TProduct();
        }
        public int ProductId
        {
            get { return _product.ProductId; }
            set { _product.ProductId = value; }
        }

        public string? ProductName
        {
            get { return _product.ProductName; }
            set { _product.ProductName = value; }
        }

        public int ProductRemainingQuantity
        {
            get { return _product.ProductRemainingQuantity; }
            set { _product.ProductRemainingQuantity = value; }
        }

        public int ProductUnitPoint
        {
            get { return _product.ProductUnitPoint; }
            set { _product.ProductUnitPoint = value; }
        }

        public string? ProductImagePath
        {
            get { return _product.ProductImagePath; }
            set { _product.ProductImagePath = value; }
        }
        public IFormFile photo { get; set; }
    }
}
