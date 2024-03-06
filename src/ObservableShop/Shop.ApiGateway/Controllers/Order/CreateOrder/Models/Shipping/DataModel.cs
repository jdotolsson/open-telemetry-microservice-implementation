// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Shop.ApiGateway.Controllers.Order.CreateOrder.Models.Shipping
{
    public class DataModel
    {
        public string TrackingNumber { get; set; } = string.Empty;
        public DateTime EstimatedDelivery { get; set; }
    }
}
