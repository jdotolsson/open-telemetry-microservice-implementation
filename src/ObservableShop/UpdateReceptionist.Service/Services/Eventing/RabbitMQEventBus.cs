using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry;
using System.Diagnostics;

namespace UpdateReceptionist.Service.Services.Eventing
{
    public class RabbitMQEventBus : IEventBus
    {
        private static readonly ActivitySource Activity = new(nameof(RabbitMQEventBus));
        private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RabbitMQEventBus> _logger;
        private IConnection? _connection;

        public RabbitMQEventBus(IConfiguration configuration, ILogger<RabbitMQEventBus> logger)
        {
            _configuration = configuration;
            _logger = logger;
            ConnectRabbitMq();
        }

        private void ConnectRabbitMq()
        {
            var factory = new ConnectionFactory()
            {
                UserName = _configuration.GetValue<string>("RABBITMQ_USERNAME"),
                Password = _configuration.GetValue<string>("RABBITMQ_PASSWORD"),
                HostName = _configuration.GetValue<string>("RABBITMQ_HOST")
            };

            _connection = TryConnectWithRetry(factory, maxAttempts: 5, retryDelaySeconds: 5);

            if (_connection == null)
            {
                _logger.LogError("Failed to establish RabbitMQ connection.");
            }
        }

        private IConnection? TryConnectWithRetry(ConnectionFactory factory, int maxAttempts, int retryDelaySeconds)
        {
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    var connection = factory.CreateConnection();
                    return connection;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"RabbitMQ connection attempt {attempt}/{maxAttempts} failed. Retrying in {retryDelaySeconds} seconds. Error: {ex.Message}");
                    System.Threading.Thread.Sleep(retryDelaySeconds * 1000);
                }
            }

            return null;
        }

        public void Queue<T>(string activity, T message, string queueName) where T : class
        {
            try
            {
                using (var activityObj = Activity.StartActivity(activity, ActivityKind.Producer))
                using (var channel = _connection?.CreateModel())
                {
                    ArgumentNullException.ThrowIfNull(activityObj, nameof(activityObj));

                    var props = channel?.CreateBasicProperties();

                    if (props != null)
                    {
                        AddActivityToHeader(activityObj, queueName, props);
                        var bytes = JsonSerializer.SerializeToUtf8Bytes(message);
                        channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: props, body: bytes);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error trying to queue a message");
                throw;
            }
        }

        private void AddActivityToHeader(Activity activity, string queueName, IBasicProperties props)
        {
            Propagator.Inject(new PropagationContext(activity.Context, Baggage.Current), props, InjectContextIntoHeader);
            activity?.SetTag("messaging.system", "rabbitmq");
            activity?.SetTag("messaging.destination_kind", "queue");
            activity?.SetTag("messaging.rabbitmq.queue", queueName);
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

        public void Dispose()
        {
            _connection?.Close();
        }
    }
}
