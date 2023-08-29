using System;
using System.Collections.Generic;

namespace EnGee.Models;

public partial class TCase
{
    public int CaseId { get; set; }

    public string CaseCategory { get; set; } = null!;

    public string CaseTitle { get; set; } = null!;

    public string CaseImagePath { get; set; } = null!;

    public string CaseContent { get; set; } = null!;

    public DateTime CaseStartDate { get; set; }

    public DateTime CaseEndDate { get; set; }

    public string DisplayStatus { get; set; } = null!;

    public int MemberId { get; set; }

    public virtual TMember Member { get; set; } = null!;
}
