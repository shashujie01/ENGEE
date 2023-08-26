using System;
using System.Collections.Generic;

namespace EnGee.Models;

public partial class TCartsItem
{
    public int CartsItemId { get; set; }

    public int ShoppingCartId { get; set; }

    public int ProductId { get; set; }

    public int CartOrderQuantity { get; set; }

    public virtual TProduct Product { get; set; } = null!;

    public virtual TShoppingCart ShoppingCart { get; set; } = null!;
}
