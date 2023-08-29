using System;
using System.Collections.Generic;

namespace EnGee.Models;

public partial class TShoppingCart
{
    public int ShoppingCartId { get; set; }

    public int MemberId { get; set; }

    public DateTime CartCreatDate { get; set; }

    public DateTime UpdateDate { get; set; }

    public virtual TMember Member { get; set; } = null!;

    public virtual ICollection<TCartsItem> TCartsItems { get; set; } = new List<TCartsItem>();
}
