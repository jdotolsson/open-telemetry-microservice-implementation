// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Shop.ApiGateway.HttpClients.Order.DataContracts;

namespace Shop.ApiGateway.HttpClients.Order
{
    public class OrderClient(HttpClient client, ILogger<OrderClient> logger) : IOrderClient
    {
        private readonly HttpClient _client = client;
        private readonly ILogger<OrderClient> _logger = logger;

        public async Task<OrderDto> CreateOrder(CancellationToken cancellationToken = default)
        {
            var response = await _client.PostAsync("/order", null, cancellationToken);
            var responseMessage = response.EnsureSuccessStatusCode();
            var result = await responseMessage.Content.ReadFromJsonAsync<OrderDto>(cancellationToken);
            return result ?? throw new ArgumentException("Could not create Order in api gateway");
        }
    }
}
