using System;
using System.Collections.Generic;

namespace EnGee.Models;

public partial class TMessage
{
    public int MessageId { get; set; }

    public int MemberId { get; set; }

    public string MessageArea { get; set; } = null!;

    public string MessageContent { get; set; } = null!;

    public int ThumpUp { get; set; }

    public int ProductId { get; set; }

    public DateTime? MessageDate { get; set; }

    public virtual TMember Member { get; set; } = null!;

    public virtual TProduct Product { get; set; } = null!;
}
