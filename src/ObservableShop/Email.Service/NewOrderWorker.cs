
using EmailService.Services.Eventing;

namespace EmailService
{
    public class NewOrderWorker : BackgroundService
    {
        private readonly ILogger<NewOrderWorker> _logger;
        private readonly IRabbitMqService _rabbitMqService;

        public NewOrderWorker(ILogger<NewOrderWorker> logger, IRabbitMqService rabbitMqService)
        {
            _logger = logger;
            _rabbitMqService = rabbitMqService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var exchangeName = "new.order.products.reserved";
            var queueName = "new.order.products.reserved.send.email";

            _rabbitMqService.SetupFanoutExchangeAndQueue(exchangeName, queueName);

            _rabbitMqService.StartConsumer(queueName, message =>
            {
                _logger.LogInformation($"------SENDING EMAIL------");
                _logger.LogInformation($"Received message: {message}");
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            _rabbitMqService.StopConsumer();
            _rabbitMqService.Dispose();
        }
    }

}
