// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Shop.ApiGateway.Clients.Payment
{
    public interface IPaymentClient
    {
        Task<bool> TakePayment(CancellationToken cancellationToken = default);
    }

    public class PaymentClient(HttpClient client) : IPaymentClient
    {
        private readonly HttpClient _client = client;

        public async Task<bool> TakePayment(CancellationToken cancellationToken = default)
        {
            var response = await _client.PostAsync("/payment", null, cancellationToken);
            var responseMessage = response.EnsureSuccessStatusCode();
            return responseMessage.StatusCode == System.Net.HttpStatusCode.Created;
        }
    }
}
