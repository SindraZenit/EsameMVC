using System;
using System.Collections.Generic;

namespace BikeStores.Models;

public partial class VwProductsByCustomer
{
    public string ProductName { get; set; } = null!;

    public string BrandName { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public int PurchaseQuantity { get; set; }

    public decimal? DiscountedPrice { get; set; }

    public decimal Discount { get; set; }

    public DateOnly OrderDate { get; set; }

    public string CustomereName { get; set; } = null!;

    public string CustomereLastName { get; set; } = null!;

    public string? City { get; set; }
}
