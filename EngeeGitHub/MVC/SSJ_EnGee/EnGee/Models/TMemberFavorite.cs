using System;
using System.Collections.Generic;

namespace EnGee.Models;

public partial class TMemberFavorite
{
    public int FavoriteId { get; set; }

    public int MemberId { get; set; }

    public int ProductId { get; set; }

    public DateTime AddFavoriteDate { get; set; }

    public bool AddFavoriteType { get; set; }

    public virtual TMember Member { get; set; } = null!;

    public virtual TProduct Product { get; set; } = null!;
}
