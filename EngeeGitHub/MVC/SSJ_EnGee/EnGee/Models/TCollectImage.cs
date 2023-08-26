using System;
using System.Collections.Generic;

namespace EnGee.Models;

public partial class TCollectImage
{
    public int CollectImageId { get; set; }

    public int CollectId { get; set; }

    public string CollectImagePath { get; set; } = null!;

    public virtual TCollect Collect { get; set; } = null!;
}
