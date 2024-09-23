using System;
using System.Collections.Generic;

namespace BikeStores.Models;

public partial class VwProductionByQuantity
{
    public string CategoryName { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public short ModelYear { get; set; }

    public decimal ListPrice { get; set; }

    public int? Quantity { get; set; }
}
