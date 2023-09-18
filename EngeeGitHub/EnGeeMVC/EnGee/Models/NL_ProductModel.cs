using EnGee.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace prjEnGeeDemo.Models
{
    public class NL_ProductModel
    {
        private TProduct _product;

        public TBrand Brand { get; set; }

        public TCosmeticMainCategory MainCategory { get; set; }

        public TCosmeticSubcategory Subcategory { get; set; }

        public TMember Seller { get; set; }

        public TProduct Product
        {
            get { return _product; }
            set { _product = value; }
        }               

        public NL_ProductModel()
        {
            _product = new TProduct();
            Brand = new TBrand();
            MainCategory = new TCosmeticMainCategory();            
            Subcategory = new TCosmeticSubcategory();
            Seller = new TMember();
        }

        [DisplayName("ID")]
        public int ProductId
        {
            get { return _product.ProductId; }
            set { _product.ProductId = value; }
        }
        [DisplayName("名稱")]
        public string ProductName
        {
            get { return _product.ProductName; }
            set { _product.ProductName = value; }
        }
        [DisplayName("品牌")]
        public int BrandId
        {
            get { return _product.BrandId; }
            set { _product.BrandId = value; }
        }
        [DisplayName("彩妝品分類")]
        public int MainCategoryId
        {
            get { return _product.MainCategoryId; }
            set { _product.MainCategoryId = value; }
        }
        [DisplayName("子分類")]
        public int SubcategoryId
        {
            get { return _product.SubcategoryId; }
            set { _product.SubcategoryId = value; }
        }
        [DisplayName("敘述")]
        public string ProductDescribe
        {
            get { return _product.ProductDescribe; }
            set { _product.ProductDescribe = value; }
        } 
        [DisplayName("照片")]
        public string ProductImagePath
        {
            get { return _product.ProductImagePath; }
            set { _product.ProductImagePath = value; }
        }
        [DisplayName("點數")]
        public int ProductUnitPoint
        {
            get { return _product.ProductUnitPoint; }
            set { _product.ProductUnitPoint = value; }
        }
        [DisplayName("數量")]
        public int ProductRemainingQuantity
        {
            get { return _product.ProductRemainingQuantity; }
            set { _product.ProductRemainingQuantity = value; }
        }
        [DisplayName("有效期限")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ProductExpirationDate
        {
            get { return _product.ProductExpirationDate; }
            set { _product.ProductExpirationDate = value; }
        }
        [DisplayName("使用狀態")]
        public string ProductUsageStatus
        {
            get { return _product.ProductUsageStatus; }
            set { _product.ProductUsageStatus = value; }
        } 
        [DisplayName("捐贈")]
        public int DonationStatus
        {
            get { return _product.DonationStatus; }
            set { _product.DonationStatus = value; }
        }
        [DisplayName("銷售日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfSale
        {
            get { return _product.DateOfSale; }
            set { _product.DateOfSale = value; }
        }
        [DisplayName("運送方式")]
        public int DeliveryTypeId
        {
            get { return _product.DeliveryTypeId; }
            set { _product.DeliveryTypeId = value; }
        }
        [DisplayName("賣家ID")]
        public int SellerId
        {
            get { return _product.SellerId; }
            set { _product.SellerId = value; }
        }
        [DisplayName("銷售狀況")]
        public int ProductSaleStatus
        {
            get { return _product.ProductSaleStatus; }
            set { _product.ProductSaleStatus = value; }
        }

        public IFormFile photo { get; set; }
    }
}
