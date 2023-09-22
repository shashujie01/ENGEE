using EnGee.ViewModel;
using System.ComponentModel.DataAnnotations;

namespace EnGee.Models;

public class SSJ_CShoppingCarItem
{
    public SSJ_ProductDetailsViewModel ProductDetails { get; set; }
    public int ProductId { get; set; }
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
    public TProduct tproduct { get; set; }
    public int 小計
    {
        get
        {
            return this.count * this.point;
        }
    }
    public bool IsSelected { get; set; }
}
