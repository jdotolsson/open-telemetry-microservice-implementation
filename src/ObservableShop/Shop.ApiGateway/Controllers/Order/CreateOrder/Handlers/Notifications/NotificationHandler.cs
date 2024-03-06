// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MediatR;
using Shop.ApiGateway.Controllers.Order.CreateOrder.Events;
using Shop.ApiGateway.Services.Eventing;

namespace Shop.ApiGateway.Controllers.Order.CreateOrder.Handlers.Notifications
{
    public class NotificationHandler(IEventBus eventBus) : INotificationHandler<ProductReservedEvent>
    {
        private readonly IEventBus _eventBus = eventBus;

        public Task Handle(ProductReservedEvent notification, CancellationToken cancellationToken)
        {
            _eventBus.Publish("Send Customer Notifications", notification, "new.order.products.reserved");
            notification.Data.Notification.NotificationsSent = true;
            return Task.CompletedTask;
        }
    }
}
