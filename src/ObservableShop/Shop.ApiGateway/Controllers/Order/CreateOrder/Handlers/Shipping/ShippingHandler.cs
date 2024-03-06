// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MediatR;
using Shop.ApiGateway.Controllers.Order.CreateOrder.Events;

namespace Shop.ApiGateway.Controllers.Order.CreateOrder.Handlers.Shipping
{
    public class ShippingHandler : INotificationHandler<ProductReservedEvent>
    {
        private static Random _random = new();
        public async Task Handle(ProductReservedEvent notification, CancellationToken cancellationToken)
        {
            //Simulate Shipping
            notification.Data.Shipping.EstimatedDelivery = DateTime.UtcNow.AddDays(3);
            notification.Data.Shipping.TrackingNumber = $"TR{_random.Next(100000000, 999999999)}SE";
        }
    }
}
