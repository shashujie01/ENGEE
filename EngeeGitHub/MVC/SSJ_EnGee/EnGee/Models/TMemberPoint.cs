using System;
using System.Collections.Generic;

namespace EnGee.Models;

public partial class TMemberPoint
{
    public int PointId { get; set; }

    public int MemberId { get; set; }

    public int Point { get; set; }

    public DateTime TransactionDate { get; set; }

    public bool TransactionType { get; set; }

    public string Memo { get; set; } = null!;

    public virtual TMember Member { get; set; } = null!;
}
