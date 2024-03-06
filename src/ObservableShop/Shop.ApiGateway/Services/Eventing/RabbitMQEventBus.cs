// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using OpenTelemetry.Context.Propagation;
using OpenTelemetry;
using RabbitMQ.Client;
using System.Diagnostics;
using System.Text.Json;

namespace Shop.ApiGateway.Services.Eventing
{
    public class RabbitMQEventBus : IEventBus
    {
        private static readonly ActivitySource Activity = new(nameof(RabbitMQEventBus));
        private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
        private readonly IConnection _connection;
        private readonly ILogger<RabbitMQEventBus> _logger;

        public RabbitMQEventBus(IConfiguration configuration, ILogger<RabbitMQEventBus> logger)
        {
            var factory = new ConnectionFactory()
            {
                UserName = configuration.GetValue<string>("RABBITMQ_USERNAME"),
                Password = configuration.GetValue<string>("RABBITMQ_PASSWORD"),
                HostName = configuration.GetValue<string>("RABBITMQ_HOST")
            };

            _connection = factory.CreateConnection();
            _logger = logger;
        }

        public void Publish<T>(string activiy, T message, string exchangeName) where T : class
        {
            try
            {
                using (var activity = Activity.StartActivity(activiy, ActivityKind.Producer))
                using (var channel = _connection.CreateModel())
                {
                    ArgumentNullException.ThrowIfNull(activity, nameof(activity));

                    var props = channel.CreateBasicProperties();

                    AddActivityToHeader(activity, props);

                    channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

                    var bytes = JsonSerializer.SerializeToUtf8Bytes(message);
                    channel.BasicPublish(exchange: exchangeName, routingKey: "", basicProperties: props, body: bytes);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error trying to publish a message");
                throw;
            }
        }

        private void AddActivityToHeader(Activity activity, IBasicProperties props)
        {
            Propagator.Inject(new PropagationContext(activity.Context, Baggage.Current), props, InjectContextIntoHeader);
            activity?.SetTag("messaging.system", "rabbitmq");
            activity?.SetTag("messaging.destination_kind", "queue");
            activity?.SetTag("messaging.rabbitmq.queue", "sample");
        }

        private void InjectContextIntoHeader(IBasicProperties props, string key, string value)
        {
            try
            {
                props.Headers ??= new Dictionary<string, object>();
                props.Headers[key] = value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to inject trace context.");
            }
        }
    }

}
