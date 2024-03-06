// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace UpdateReceptionist.Service
{
    public class ProductPriceUpdateModel(string id, decimal newPrice)
    {
        public string Id { get; set; } = id;
        public decimal NewPrice { get; set; } = newPrice;
    }
}
