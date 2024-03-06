// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Shop.ApiGateway.Services.Eventing
{
    public interface IEventBus
    {
        void Publish<T>(string activiy, T message, string exchangeName) where T : class;
    }
}
