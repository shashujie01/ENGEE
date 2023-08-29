using System;
using System.Collections.Generic;

namespace EnGee.Models;

public partial class TBrand
{
    public int BrandId { get; set; }

    public string BrandCategory { get; set; } = null!;

    public string? BrandName { get; set; }

    public virtual ICollection<TProduct> TProducts { get; set; } = new List<TProduct>();
}
