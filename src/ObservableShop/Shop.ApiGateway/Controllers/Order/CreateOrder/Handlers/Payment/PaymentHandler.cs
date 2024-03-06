// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MediatR;
using Shop.ApiGateway.Clients.Payment;
using Shop.ApiGateway.Controllers.Order.CreateOrder.Events;

namespace Shop.ApiGateway.Controllers.Order.CreateOrder.Handlers.Payment
{
    public class PaymentHandler(IPaymentClient client) : INotificationHandler<OrderCreatedEvent>
    {
        private readonly IPaymentClient _client = client;

        public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            notification.Data.Payment.PaymentSucceeded = await _client.TakePayment(cancellationToken);
        }
    }
}
