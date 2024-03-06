// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ProductClient;
using Shop.ApiGateway.Clients.Payment;
using Shop.ApiGateway.HttpClients.Order;

namespace Shop.ApiGateway.HttpClients
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient<IOrderClient, OrderClient>(options =>
            {
                options.BaseAddress = new Uri("http://order_service:8080");
            });

            services.AddHttpClient<IPaymentClient, PaymentClient>(options =>
            {
                options.BaseAddress = new Uri("http://payment_service:8080");
            });

            services.AddGrpcClient<ProductService.ProductServiceClient>(o =>
            {
                o.Address = new Uri("http://product_service:50051");
            });

            return services;
        }
    }
}
