using EnGee.ViewModel;
using System.ComponentModel.DataAnnotations;

namespace EnGee.Models;

public class SSJ_CShoppingCarItem
{
    public SSJ_ProductDetailsViewModel ProductDetails { get; set; }
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
    public string DeliveryAddress
    {
        get
        {
            if (DeliveryTypeID == 1)
            {
                return DeliveryAddress_homeDelivery;
            }
            else if (DeliveryTypeID == 2)
            {
                return DeliveryAddress_storePickup;
            }
            else
            {
                return "未知"; // 或其他預設值
            }
        }
    }
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
    //下面是給超商JSON使用
    public string StoreID { get; set; }
    public string StoreName { get; set; }
    public string Address { get; set; }
}
