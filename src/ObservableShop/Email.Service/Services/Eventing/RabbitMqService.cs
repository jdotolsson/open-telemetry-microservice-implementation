// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace EmailService.Services.Eventing
{
    public class RabbitMqService : IRabbitMqService
    {
        private static readonly ActivitySource Activity = new(nameof(RabbitMqService));
        private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
        private string QueueName = string.Empty;
        private string ExchangeName = string.Empty;

        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMqService> _logger;
        private EventingBasicConsumer? _consumer;

        public RabbitMqService(IConfiguration configuration, ILogger<RabbitMqService> logger)
        {
            var factory = new ConnectionFactory
            {
                UserName = configuration.GetValue<string>("RABBITMQ_USERNAME"),
                Password = configuration.GetValue<string>("RABBITMQ_PASSWORD"),
                HostName = configuration.GetValue<string>("RABBITMQ_HOST")
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _logger = logger;
        }

        public void SetupFanoutExchangeAndQueue(string exchangeName, string queueName)
        {
            QueueName = queueName;
            ExchangeName = exchangeName;
            _channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout);
            _channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(queueName, exchangeName, string.Empty);
        }

        public void StartConsumer(string queueName, Action<string> handleMessage)
        {
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += (sender, eventArgs) =>
            {
                ProcessMessage(eventArgs, handleMessage);
            };

            _channel.BasicConsume(queueName, autoAck: false, consumer: _consumer);
        }

        private void ProcessMessage(BasicDeliverEventArgs ea, Action<string> handleMessage)
        {
            try
            {
                //Extract the activity and set it into the current one
                var parentContext = Propagator.Extract(default, ea.BasicProperties, ExtractTraceContextFromBasicProperties);
                Baggage.Current = parentContext.Baggage;

                //Start a new Activity
                using (var activity = Activity.StartActivity("Process Message", ActivityKind.Consumer, parentContext.ActivityContext))
                {
                    ArgumentNullException.ThrowIfNull(activity, nameof(activity));

                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    //Add Tags to the Activity
                    AddActivityTags(activity);

                    handleMessage?.Invoke(message);
                   
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"There was an error processing the message: {ex} ");
            }
        }

        //Extract the Activity from the message header
        private IEnumerable<string> ExtractTraceContextFromBasicProperties(IBasicProperties props, string key)
        {
            try
            {
                if (props.Headers.TryGetValue(key, out var value))
                {
                    var bytes = value as byte[];
                    ArgumentNullException.ThrowIfNull(bytes, nameof(bytes));
                    return new[] { Encoding.UTF8.GetString(bytes) };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to extract trace context: {ex}");
            }

            return Enumerable.Empty<string>();
        }

        //Add Tags to the Activity
        private void AddActivityTags(Activity activity)
        {
            activity?.SetTag("messaging.system", "rabbitmq");
            activity?.SetTag("messaging.destination_kind", "queue");
            activity?.SetTag("messaging.rabbitmq.exchange", ExchangeName);
            activity?.SetTag("messaging.rabbitmq.queue", QueueName);
        }

        public void StopConsumer()
        {
            _consumer?.OnCancel();
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
