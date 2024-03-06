// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using UpdateReceptionist.Service.Services.Eventing;

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace UpdateReceptionist.Service.Services.Eventing
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventing(this IServiceCollection services)
        {
            services.AddSingleton<IEventBus, RabbitMQEventBus>();
            return services;
        }
    }
}
