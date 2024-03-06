// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace EmailService.Services.Eventing
{
    public interface IRabbitMqService : IDisposable
    {
        void SetupFanoutExchangeAndQueue(string exchangeName, string queueName);
        void StartConsumer(string queueName, Action<string> handleMessage);
        void StopConsumer();
    }
}
