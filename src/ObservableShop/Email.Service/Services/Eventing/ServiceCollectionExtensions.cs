// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace EmailService.Services.Eventing
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventing(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMqService, RabbitMqService2>();
            return services;
        }
    }
}
