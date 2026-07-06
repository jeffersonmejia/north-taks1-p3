using System;
using System.Collections.Generic;

namespace NorthwindStore.Models.Northwind;

public partial class UsState
{
    public int StateId { get; set; }

    public string? StateName { get; set; }

    public string? StateAbbr { get; set; }

    public string? StateRegion { get; set; }
}
