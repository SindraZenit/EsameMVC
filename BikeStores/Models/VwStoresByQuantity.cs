using System;
using System.Collections.Generic;

namespace BikeStores.Models;

public partial class VwStoresByQuantity
{
    public string BrandName { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public string StoreName { get; set; } = null!;

    public string? City { get; set; }

    public int? Quantity { get; set; }
}
