// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MediatR;
using Shop.ApiGateway.Controllers.Order.CreateOrder.DataContracts;
using Shop.ApiGateway.Controllers.Order.CreateOrder.Events;
using Shop.ApiGateway.Controllers.Order.CreateOrder.Models;

namespace Shop.ApiGateway.Controllers.Order.CreateOrder
{
    public class OrderHandler(IMediator mediator) : IRequestHandler<OrderHandler.RequestDto, OrderHandler.ResponseDto>
    {
        private readonly IMediator _mediator = mediator;
        public async Task<ResponseDto> Handle(RequestDto request, CancellationToken cancellationToken)
        {
            var orderContainer = new DataModel();
            var @event = new CreateOrderEvent(request, orderContainer);
            await _mediator.Publish(@event, cancellationToken);

            return new ResponseDto(orderContainer);
        }

        public class RequestDto(OrderDto orderDto) : IRequest<ResponseDto>
        {
            public OrderDto Order { get; set; } = orderDto;
        }

        public class ResponseDto(DataModel order)
        {
            public DataModel Order { get; set; } = order;
        }
    }
}
