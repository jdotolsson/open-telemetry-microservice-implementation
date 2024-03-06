// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Shop.ApiGateway.Controllers.Order.CreateOrder.Models
{
    public class DataModel
    {
        public Notification.DataModel Notification { get; set; } = new();
        public Order.DataModel Order { get; set; } = new();
        public Payment.DataModel Payment { get; set; } = new();
        public Product.DataModel Product { get; set; } = new();
        public Shipping.DataModel Shipping { get; set; } = new();
    }
}
