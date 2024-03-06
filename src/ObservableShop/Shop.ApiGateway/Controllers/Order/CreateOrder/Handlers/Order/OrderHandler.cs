// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text;
using MediatR;
using Shop.ApiGateway.Controllers.Order.CreateOrder.Events;
using Shop.ApiGateway.HttpClients.Order;

namespace Shop.ApiGateway.Controllers.Order.CreateOrder.Handlers.Order
{
    public class OrderHandler(
        IMediator mediator,
        IOrderClient client) : INotificationHandler<CreateOrderEvent>
    {
        private readonly IMediator _mediator = mediator;
        private readonly IOrderClient _client = client;

        public async Task Handle(CreateOrderEvent notification, CancellationToken cancellationToken)
        {
            var order = await _client.CreateOrder(cancellationToken);
            notification.Data.Order.Id = order.Id;            
            await _mediator.Publish(new OrderCreatedEvent(order.Id, notification.Request, notification.Data), cancellationToken);
        }
    }
}
