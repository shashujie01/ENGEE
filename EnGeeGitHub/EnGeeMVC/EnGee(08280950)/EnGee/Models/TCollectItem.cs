using System;
using System.Collections.Generic;

namespace EnGee.Models;

public partial class TCollectItem
{
    public int CollectItemsId { get; set; }

    public int CollectId { get; set; }

    public string CollectItemName { get; set; } = null!;

    public int MainCategoryId { get; set; }

    public int SubcategoryId { get; set; }

    public int CollectAmount { get; set; }

    public virtual TCollect Collect { get; set; } = null!;

    public virtual TCosmeticMainCategory MainCategory { get; set; } = null!;

    public virtual TCosmeticSubcategory Subcategory { get; set; } = null!;
}
