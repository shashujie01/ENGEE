using System;
using System.Collections.Generic;

namespace Engee.Models;

public partial class TCosmeticMainCategory
{
    public int MainCategoryId { get; set; }

    public string MainCategory { get; set; } = null!;

    public virtual ICollection<TCollectItem> TCollectItems { get; set; } = new List<TCollectItem>();

    public virtual ICollection<TCosmeticSubcategory> TCosmeticSubcategories { get; set; } = new List<TCosmeticSubcategory>();

    public virtual ICollection<TProduct> TProducts { get; set; } = new List<TProduct>();
}
