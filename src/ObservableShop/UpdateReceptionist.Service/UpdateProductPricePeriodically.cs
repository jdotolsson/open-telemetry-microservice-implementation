// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using UpdateReceptionist.Service.Services.Eventing;

namespace UpdateReceptionist.Service
{
    public class UpdateProductPricePeriodically(ILogger<UpdateProductPricePeriodically> logger, IEventBus eventBus) : BackgroundService
    {
        private readonly ILogger<UpdateProductPricePeriodically> _logger = logger;
        private readonly IEventBus _eventBus = eventBus;
        private static Random _random = new();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    var productId = $"P{_random.Next(1, 21):000}";
                    var newPrice = GenerateRandomPrice();
                    _logger.LogInformation("Receptionist is updating Product {id} with price {price}. running at: {time}", productId, newPrice, DateTimeOffset.Now);
                    _eventBus.Queue("Updating Product Price", new ProductPriceUpdateModel(productId, newPrice), "product.price.update");
                }
                var secondsToWait = _random.Next(10, 30);
                await Task.Delay(TimeSpan.FromSeconds(secondsToWait), stoppingToken);
            }
        }

        public static decimal GenerateRandomPrice()
        {
            var randomDouble = _random.NextDouble() * (9999.99 - 5.99) + 5.99;
            var randomPrice = decimal.Round((decimal)randomDouble, 2);
            return randomPrice;
        }
    }
}
