using Microsoft.Extensions.Configuration;
using System;
using System.Text;
using System.Threading.Tasks;

namespace PrinterSample.Notifiers
{
    using Printer.Notifiers;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Exceptions;
    using RabbitMQ.Client.Events;
    using Exceptions;

    internal class RabbitMq : IPrinterNotifier
    {
        private ConnectionFactory _factory;

        private Action _disposeConnections;

        public RabbitMq(IConfigurationRoot configuration)
        {
            _factory = new ConnectionFactory() { HostName = configuration["RabbitMq:HostName"] };
        }

        async Task IPrinterNotifier.Notify(string message, PrinterNotificationTypes notificationType, Guid correlationId)
        {
            var channel = correlationId == Guid.Empty ? $"Printer.{notificationType.ToString()}" : $"Printer.{notificationType.ToString()}.{correlationId}";
            await Notify(message, channel);
        }

        async Task IPrinterNotifier.Subscribe(Action<string> callback, PrinterNotificationTypes notificationType, Guid correlationId)
        {
            var channel = correlationId == Guid.Empty ? $"Printer.{notificationType.ToString()}" : $"Printer.{notificationType.ToString()}.{correlationId}";
            await Subscribe(callback, channel);
        }

        public Task Notify(string message, string channel)
        {
            var (exchange, routingKey, correlationId) = SplitChannel(channel);

            try
            {
                using (var connection = _factory.CreateConnection())
                using (var model = connection.CreateModel())
                {
                    model.ExchangeDeclare(exchange: exchange, type: ExchangeType.Direct);

                    var body = Encoding.UTF8.GetBytes(message);
                    var props = model.CreateBasicProperties();
                    props.CorrelationId = correlationId;

                    model.BasicPublish(exchange: exchange,
                                        routingKey: routingKey,
                                        basicProperties: props,
                                        body: body);
                }

                return Task.CompletedTask;
            }
            catch (BrokerUnreachableException)
            {
                throw new CannotConnectToNotificationServiceException();
            }
        }

        public Task Subscribe(Action<string> callback, string channel)
        {
            var (exchange, routingKey, correlationId) = SplitChannel(channel);

            try
            {
                var connection = _factory.CreateConnection();
                var model = connection.CreateModel();

                _disposeConnections += () =>
                {
                    model.Close();
                    connection.Close();
                };

                model.ExchangeDeclare(exchange: exchange, type: ExchangeType.Direct);

                var queueName = model.QueueDeclare(durable: true, autoDelete: false).QueueName;
                model.QueueBind(queue: queueName,
                                exchange: exchange,
                                routingKey: routingKey);

                var consumer = new EventingBasicConsumer(model);
                consumer.Received += (m, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    if (ea.BasicProperties.CorrelationId == correlationId)
                        callback(message);
                    else
                        model.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);

                };

                model.BasicConsume(queue: queueName,
                                    autoAck: false,
                                    consumer: consumer);

                return Task.CompletedTask;
            }
            catch (BrokerUnreachableException)
            {
                throw new CannotConnectToNotificationServiceException();
            }
        }

        public void Dispose()
        {
            if (_disposeConnections != null)
                _disposeConnections.Invoke();
        }

        private (string exchange, string routingKey, string correlationId) SplitChannel(string channel)
        {
            string exchange = String.Empty,
                routingKey = String.Empty,
                correlationId = String.Empty;

            if (String.IsNullOrWhiteSpace(channel))
                return (exchange, routingKey, correlationId);

            var channelParts = channel.Split('.');
            int elements = channelParts.Length;

            if (elements == 3)
                correlationId = channelParts[2];
            if (elements >= 2)
                routingKey = channelParts[1];
            if (elements >= 1)
                exchange = channelParts[0];

            return (exchange, routingKey, correlationId);
        }

    }
}
