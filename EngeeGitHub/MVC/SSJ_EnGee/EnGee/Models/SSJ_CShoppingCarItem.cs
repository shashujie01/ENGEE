using System.ComponentModel.DataAnnotations;

namespace EnGee.Models;

public class SSJ_CShoppingCarItem
{
    public int ProductId { get; set; }
    
    public int count { get; set; }
    public int point { get; set; }

    public string ProductImagePath { get; set; } = null!;

    public TProduct tproduct { get; set; }
    public decimal 小計
    {
        get
        {
            return this.count * this.point;
        }
    }
}
