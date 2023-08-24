﻿using System;
using System.Collections.Generic;

namespace Engee.Models;

public partial class TCosmeticSubcategory
{
    public int SubcategoryId { get; set; }

    public string Subcategory { get; set; } = null!;

    public int MainCategoryId { get; set; }

    public virtual TCosmeticMainCategory MainCategory { get; set; } = null!;

    public virtual ICollection<TCollectItem> TCollectItems { get; set; } = new List<TCollectItem>();

    public virtual ICollection<TProduct> TProducts { get; set; } = new List<TProduct>();
}
