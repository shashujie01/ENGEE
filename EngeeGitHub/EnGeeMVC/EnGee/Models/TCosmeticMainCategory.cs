using System;
using System.Collections.Generic;

namespace EnGee.Models;

public partial class TCosmeticMainCategory
{
    public int MainCategoryId { get; set; }

    public string MainCategory { get; set; } = null!;

    public virtual ICollection<TCosmeticSubcategory> TCosmeticSubcategories { get; set; } = new List<TCosmeticSubcategory>();

    public virtual ICollection<TProduct> TProducts { get; set; } = new List<TProduct>();
}
