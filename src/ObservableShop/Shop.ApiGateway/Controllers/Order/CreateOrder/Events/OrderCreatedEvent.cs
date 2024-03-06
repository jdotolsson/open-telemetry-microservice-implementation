// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MediatR;
using Shop.ApiGateway.Controllers.Order.CreateOrder.Models;

namespace Shop.ApiGateway.Controllers.Order.CreateOrder.Events
{
    public class OrderCreatedEvent(string orderId, OrderHandler.RequestDto request, DataModel dataModel) : INotification
    {
        public string OrderId { get; set; } = orderId;
        public OrderHandler.RequestDto Request { get; set; } = request;

        public DataModel Data { get; set; } = dataModel;
    }

    public class CreateOrderEvent(OrderHandler.RequestDto request, DataModel dataModel) : INotification
    {
        public OrderHandler.RequestDto Request { get; set; } = request;
        public DataModel Data { get; set; } = dataModel;
    }


    public class ProductReservedEvent(string orderId, OrderHandler.RequestDto request, DataModel dataModel) : INotification
    {
        public string OrderId { get; set; } = orderId;
        public OrderHandler.RequestDto Request { get; set; } = request;

        public DataModel Data { get; set; } = dataModel;
    }
}
