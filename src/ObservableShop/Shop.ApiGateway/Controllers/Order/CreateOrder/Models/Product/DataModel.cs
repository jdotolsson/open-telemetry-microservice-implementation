﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Shop.ApiGateway.Controllers.Order.CreateOrder.Models.Product
{
    public class DataModel
    {
        public List<Article> OrderedProducts { get; set; } = new();
        public List<Article> MissingInventory { get; set; } = new();
    }

    public class Article
    {
        public string Id { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
