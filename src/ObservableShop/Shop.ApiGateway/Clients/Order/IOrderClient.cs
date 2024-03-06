// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Shop.ApiGateway.HttpClients.Order.DataContracts;

namespace Shop.ApiGateway.HttpClients.Order
{
    public interface IOrderClient
    {
        Task<OrderDto> CreateOrder(CancellationToken cancellationToken = default);
    }
}
